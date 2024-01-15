using Scriban;
using ScyScaff.Core.Enums.Parser;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Builder;
using ScyScaff.Core.Utils.Builder;

namespace ScyScaff.Core.Services.Builder;

internal static class TemplateTreeGenerator
{
    public static void GenerateServicesFiles(ScaffolderConfig config, string workingDirectory)
    {
        foreach (KeyValuePair<string, ScaffolderService> service in config.Services)
        {
            DirectoryInfo serviceDirectory = Directory.CreateDirectory(Path.Combine(workingDirectory, $"{config.ProjectName}.{service.Key}"));
            
            string templateTreePath = service.Value.AssignedFrameworkPlugin!.GetTemplateTreePath();
            DirectoryTreeNode rootTemplateTreeNode = DirectoryTree.GetDirectoryTree(templateTreePath);
            
            GenerateFilesFromTree(rootTemplateTreeNode, new GenerationContext(config, service.Value, templateTreePath.Length, serviceDirectory.FullName));
        }
    }
    
    private static void GenerateFilesFromTree(DirectoryTreeNode treeNode, GenerationContext context)
    {
        if (treeNode.Path.Contains("[...modelIterator...]"))
        {
            IEnumerable<string> modelTemplateDirectories = Directory
                .GetDirectories(Path.GetDirectoryName(treeNode.Path)!, "*.*", SearchOption.AllDirectories)
                .ToList();
            
            IEnumerable<string> modelTemplateFiles = Directory
                .GetFiles(Path.GetDirectoryName(treeNode.Path)!, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".liquid"))
                .ToList();

            foreach (KeyValuePair<string, Dictionary<string, FieldTypeProvider>> model in context.Service.Models)
            {
                foreach (string modelTemplateDirectory in modelTemplateDirectories)
                    GenerateTemplateFile(modelTemplateDirectory, context, model);
                
                foreach (string modelTemplateFile in modelTemplateFiles)
                    GenerateTemplateFile(modelTemplateFile, context, model);
            }
        }
        
        if (treeNode.Path.EndsWith(".liquid"))
        {
            bool isInIterableDirectory = Directory
                .GetFiles(Path.GetDirectoryName(treeNode.Path)!)
                .Any(directory => directory.Contains("[..."));
            
            if (!isInIterableDirectory)
                GenerateTemplateFile(treeNode.Path, context);
        }
        
        if (File.GetAttributes(treeNode.Path).HasFlag(FileAttributes.Directory) && !treeNode.Path.Contains("{{"))
            Directory.CreateDirectory(Path.Combine(context.ServiceDirectory, treeNode.Path[context.TemplateTreePathLength..]));

        foreach (DirectoryTreeNode treeChild in treeNode.Children)
        {
            if (treeChild.Path.Contains("{{")) continue;
            
            GenerateFilesFromTree(treeChild, context);
        }
    }
    
    private static void GenerateTemplateFile(string filePath, GenerationContext context, KeyValuePair<string, Dictionary<string, FieldTypeProvider>>? model = null)
    {
        Console.WriteLine(filePath);
        
        Template fileNameTemplate = Template.Parse(filePath);
        string? fileNameResult = fileNameTemplate.Render(new
        {
            context.Config,
            context.Service,
            Model = model
        });
        
        if (fileNameResult is null) return;

        string newFilePath = Path.Combine(context.ServiceDirectory, fileNameResult[context.TemplateTreePathLength..]);

        if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
        {
            Directory.CreateDirectory(newFilePath);
            
            return;
        }
        
        Template fileContentTemplate = Template.Parse(File.ReadAllText(filePath));
        string? fileContentResult = fileContentTemplate.Render(new
        {
            context.Config,
            context.Service,
            Model = model
        });
        
        if (fileContentResult is null) return;
        
        // Trim ".liquid" from file name.
        File.WriteAllText(newFilePath[..^7], fileContentResult);
    }
}