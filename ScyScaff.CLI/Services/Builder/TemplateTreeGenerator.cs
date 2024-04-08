using System.Text;
using Scriban;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
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

        // Determine target file path, trimming ".liquid" from the file name.
        string targetFilePath = newFilePath[..^7];
        
        // If the file exists, we should start the process of adding new lines rather than replacing them all.
        if (File.Exists(targetFilePath))
        {
            // Check if add-mode is enabled, if not, stop the program.
            if (context.IsAddModeEnabled is null or false)
            {
                Console.WriteLine("Warning! You executed the scaffolder in a directory where there are already files with the same name. Add the \"--add true\" option to continue. This will only add new lines and will not remove any code you've written, but you will likely need to rework lines you've already modified. The Docker-Compose file will be overwritten completely.");
                Environment.Exit(-1);
            }

            // Call the function to add new lines if everything is good to go.
            AddNewLines(targetFilePath, fileContentResult);

            // Stop the function.
            return;
        }
        
        // Write the processed content to the new file.
        File.WriteAllText(targetFilePath, fileContentResult);
    }

    // Only add new lines to the target file based on the content we generated.
    private static void AddNewLines(string targetFilePath, string fileContentResult)
    {
        // Read the entire content of the file and compare it with the content we processed. If it is the same, we should exit the function.
        string targetFileContent = File.ReadAllText(targetFilePath);

        if (targetFileContent == fileContentResult) return;

        // Create textual diff model between existing file content and new one.
        DiffPaneModel diff = InlineDiffBuilder.Diff(fileContentResult, targetFileContent);
        // Create a StringBuilder, so that we will write string-lines in a loop.
        StringBuilder outputBuilder = new();

        // Add a line to StringBuilder if it was: Deleted (Added by a new version, since it is taken as the original), unchanged, inserted (custom user code).
        foreach (DiffPiece? line in diff.Lines.Where(line => line.Type == ChangeType.Deleted || line.Type == ChangeType.Unchanged || line.Type == ChangeType.Inserted))
            outputBuilder.Append($"{line.Text}\n");

        // Write StringBuilder to file.
        File.WriteAllText(targetFilePath, outputBuilder.ToString());
    }
}