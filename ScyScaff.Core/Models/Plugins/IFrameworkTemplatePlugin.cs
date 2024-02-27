namespace ScyScaff.Core.Models.Plugins;

public interface IFrameworkTemplatePlugin : ITemplatePlugin
{
    string FrameworkName { get; }
    
    string[] SupportedAuth { get; }
    string[] SupportedDatabases { get; }
    Dictionary<string, string[]> SupportedFlags { get; }
}
