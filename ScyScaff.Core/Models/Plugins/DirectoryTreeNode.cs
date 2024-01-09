namespace ScyScaff.Core.Models.Plugins;

public class DirectoryTreeNode(string name)
{
    public string Name { get; } = name;
    public List<DirectoryTreeNode> Children { get; } = new();
    
    public void AddChild(DirectoryTreeNode child)
    {
        Children.Add(child);
    }
}