using ScyScaff.Core.Models.Builder;

namespace ScyScaff.Core.Utils.Builder;

public static class DirectoryTree
{
    public static DirectoryTreeNode GetDirectoryTree(string directoryPath)
    {
        var root = new DirectoryTreeNode(directoryPath);

        foreach (string subDirectory in Directory.GetDirectories(directoryPath))
        {
            DirectoryTreeNode subDirectoryNode = GetDirectoryTree(subDirectory);
            root.AddChild(subDirectoryNode);
        }

        foreach (string file in Directory.GetFiles(directoryPath))
        {
            root.AddChild(new DirectoryTreeNode(file));
        }

        return root;
    }
}