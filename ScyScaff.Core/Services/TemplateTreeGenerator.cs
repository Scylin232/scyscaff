using Scriban;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Services;

public static class TemplateTreeGenerator
{
    public static void GenerateFilesFromTree(DirectoryTreeNode rootTree, string directory, int rootPathLength)
    {
        if (rootTree.Name.EndsWith(".liquid"))
        {
            Template template = Template.Parse(File.ReadAllText(rootTree.Name));
            string? result = template.Render();

            if (result is not null)
            {
                string newFilePath = Path.Combine(directory, rootTree.Name[rootPathLength..]);

                // Trim ".liquid" from file name.
                File.WriteAllText(newFilePath[..^7], result);
            }
        }
        else
        {
            Directory.CreateDirectory(Path.Combine(directory, rootTree.Name[rootPathLength..]));
        }
        
        foreach (DirectoryTreeNode treeChild in rootTree.Children)
        {
            GenerateFilesFromTree(treeChild, directory, rootPathLength);
        }
    }
}