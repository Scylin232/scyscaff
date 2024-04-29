namespace ScyScaff.Core.Models.Plugins;

public interface IFrameworkTemplatePlugin : ITemplatePlugin
{
    string[] SupportedAuth { get; }
    string[] SupportedDatabases { get; }
}
