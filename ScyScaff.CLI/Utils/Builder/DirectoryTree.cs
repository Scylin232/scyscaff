using System.IO.Abstractions;
using ScyScaff.Core.Models.Builder;

namespace ScyScaff.Core.Utils.Builder;

public static class DirectoryTree
{
    public static DirectoryTreeNode GetDirectoryTree(IFileSystem fileSystem, string directoryPath)
    {
        DirectoryTreeNode root = new(directoryPath);
        
        foreach (string subDirectory in fileSystem.Directory.GetDirectories(directoryPath))
        {
            DirectoryTreeNode subDirectoryNode = GetDirectoryTree(fileSystem, subDirectory);
            root.AddChild(subDirectoryNode);
        }
        
        foreach (string file in fileSystem.Directory.GetFiles(directoryPath))
            root.AddChild(new DirectoryTreeNode(file));
        
        return root;
    }
}