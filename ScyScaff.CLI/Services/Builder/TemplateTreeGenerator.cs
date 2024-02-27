using Scriban;
using ScyScaff.Core.Models.Builder;
using ScyScaff.Core.Models.Events;
using ScyScaff.Core.Utils.Builder;

namespace ScyScaff.Core.Services.Builder;

// This class is responsible for generating files and directories based on a template tree.
internal static class TemplateTreeGenerator
{
    // Generates files and directories based on the provided template tree.
    public static async Task GenerateFromTree(TreeGenerationContext generationContext, string workingDirectory)
    {
        // Create a directory for the entity based on project name and entity name.
        DirectoryInfo entityDirectory = Directory.CreateDirectory(Path.Combine(workingDirectory, $"{generationContext.Config.ProjectName}.{generationContext.EntityName}"));

        // Check if the template plugin implements the ITemplateGenerationEvents interface.
        ITemplateGenerationEvents? generationEvents = generationContext.TemplatePlugin as ITemplateGenerationEvents;

        // Invoke OnServiceGenerationStarted event if supported by the template plugin.
        if (generationEvents is not null)
            await generationEvents.OnServiceGenerationStarted(entityDirectory);

        // Get the path to the template tree.
        string templateTreePath = generationContext.TemplatePlugin.GetTemplateTreePath();
        DirectoryTreeNode rootTemplateTreeNode = DirectoryTree.GetDirectoryTree(templateTreePath);

        // Set some properties in the generation context for use in template processing.
        generationContext.EntityDirectory = entityDirectory.FullName;
        generationContext.TemplateTreePathLength = templateTreePath.Length;

        // Recursively parse and generate files based on the template tree.
        ParseServiceTree(rootTemplateTreeNode, generationContext);

        // Invoke OnServiceGenerationEnded event if supported by the template plugin.
        if (generationEvents is not null)
            await generationEvents.OnServiceGenerationEnded(entityDirectory);
    }

    // Recursively parses the template tree and generates files and directories accordingly.
    private static void ParseServiceTree(DirectoryTreeNode treeNode, TreeGenerationContext context)
    {
        // If the path contains a model iterator placeholder, generate files for each model.
        if (treeNode.Path.Contains("[...modelIterator...]"))
        {
            // Get model template directories and files.
            IEnumerable<string> modelTemplateDirectories = Directory
                .GetDirectories(Path.GetDirectoryName(treeNode.Path)!, "*.*", SearchOption.AllDirectories)
                .ToList();
            
            IEnumerable<string> modelTemplateFiles = Directory
                .GetFiles(Path.GetDirectoryName(treeNode.Path)!, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".liquid"))
                .ToList();

            // If a service is specified in the context, we will use its models, otherwise we will use the models of all services.
            IEnumerable<KeyValuePair<string, Dictionary<string, string>>> models = 
                context.Service?.Models ?? 
                context.Config.Services.SelectMany(x => x.Value.Models);
            
            foreach (KeyValuePair<string, Dictionary<string, string>> model in models)
            {
                foreach (string modelTemplateDirectory in modelTemplateDirectories)
                    GenerateServiceFile(modelTemplateDirectory, context, model);

                foreach (string modelTemplateFile in modelTemplateFiles)
                    GenerateServiceFile(modelTemplateFile, context, model);
            }
        }

        // If the path ends with ".liquid" and is not in an iterable directory, generate the file.
        if (treeNode.Path.EndsWith(".liquid"))
        {
            bool isInIterableDirectory = Directory
                .GetFiles(Path.GetDirectoryName(treeNode.Path)!)
                .Any(directory => directory.Contains("[..."));

            if (!isInIterableDirectory)
                GenerateServiceFile(treeNode.Path, context);
        }

        // If the node represents a directory and does not have templated name, create it in the entity directory.
        if (File.GetAttributes(treeNode.Path).HasFlag(FileAttributes.Directory) && !treeNode.Path.Contains("{{"))
            Directory.CreateDirectory(Path.Combine(context.EntityDirectory, treeNode.Path[context.TemplateTreePathLength..]));

        // Recursively process child nodes.
        foreach (DirectoryTreeNode treeChild in treeNode.Children)
        {
            if (treeChild.Path.Contains("{{")) continue;

            ParseServiceTree(treeChild, context);
        }
    }

    // Generates a service file based on the provided template file and context.
    private static void GenerateServiceFile(string filePath, TreeGenerationContext context, KeyValuePair<string, Dictionary<string, string>>? model = default)
    {
        // Replace specific patterns to appropriate symbols.
        // NOTE: Used for symbols that can't be used in filenames, but can be used in parser, like: . (Dot), | (Pipe), etc.
        string parsedFilePath = FilePathParser.ReplacePatterns(filePath);

        // Parse the file path as a Scriban template to get the actual file name.
        Template fileNameTemplate = Template.Parse(parsedFilePath);
        string? fileNameResult = fileNameTemplate.Render(new
        {
            context.Config,
            context.Service,
            context.ComposeServices,
            Model = model
        });
        
        // If the file name result is null, skip further processing.
        if (fileNameResult is null) return;

        // Build the new file path in the entity directory.
        string newFilePath = Path.Combine(context.EntityDirectory, fileNameResult[context.TemplateTreePathLength..]);

        // If the original file path represents a directory, create it and stop the function.
        if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
        {
            Directory.CreateDirectory(newFilePath);
            return;
        }

        // Parse the file content using Scriban.
        Template fileContentTemplate = Template.Parse(File.ReadAllText(filePath));
        string? fileContentResult = fileContentTemplate.Render(new
        {
            context.Config,
            context.Service,
            context.ComposeServices,
            Model = model
        });

        // If the file content result is null, skip further processing.
        if (fileContentResult is null) return;

        // Write the processed content to the new file, trimming ".liquid" from the file name.
        File.WriteAllText(newFilePath[..^7], fileContentResult);
    }
}