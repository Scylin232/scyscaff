namespace ScyScaff.Core.Models.Plugins;

public interface IFrameworkTemplatePlugin : ITemplatePlugin
{
    string FrameworkName { get; }
    
    string[] SupportedAuth { get; }
    string[] SupportedDatabases { get; }
}

