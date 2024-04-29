namespace ScyScaff.Core.Models.Plugins;

// Basic interface of all plugins.
public interface ITemplatePlugin
{
    public string Name { get; }
    
    public Dictionary<string, string[]> SupportedFlags { get; }
}