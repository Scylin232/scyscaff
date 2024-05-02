using System.IO.Abstractions;
using ScyScaff.CLI.Models.Builder;

namespace ScyScaff.CLI.Utils.Builder;

// Represents a utility class for constructing directory trees
public static class DirectoryTree
{
    // Constructs a directory tree rooted at the specified directory path
    public static DirectoryTreeNode GetDirectoryTree(IFileSystem fileSystem, string directoryPath)
    {
        // Creates the root node of the directory tree
        DirectoryTreeNode root = new DirectoryTreeNode(directoryPath);
        
        // Iterates through each subdirectory in the specified directory
        foreach (string subDirectory in fileSystem.Directory.GetDirectories(directoryPath))
        {
            // Recursively constructs the directory tree for each subdirectory
            DirectoryTreeNode subDirectoryNode = GetDirectoryTree(fileSystem, subDirectory);
            
            // Adds the subdirectory node as a child of the root node
            root.AddChild(subDirectoryNode);
        }
        
        // Iterates through each file in the specified directory
        foreach (string file in fileSystem.Directory.GetFiles(directoryPath))
        {
            // Creates a leaf node for each file and adds it as a child of the root node
            root.AddChild(new DirectoryTreeNode(file));
        }
        
        // Returns the root node of the constructed directory tree
        return root;
    }
}