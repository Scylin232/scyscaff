namespace ScyScaff.Core.Models.Builder;

public class DirectoryTreeNode(string path)
{
    public string Path { get; } = path;
    public List<DirectoryTreeNode> Children { get; } = new();
    
    public void AddChild(DirectoryTreeNode child)
    {
        Children.Add(child);
    }
}