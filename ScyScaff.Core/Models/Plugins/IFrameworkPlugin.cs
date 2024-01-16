namespace ScyScaff.Core.Models.Plugins;

public interface IFrameworkPlugin
{
    string FrameworkName { get; }
    
    string[] SupportedAuth { get; }
    string[] SupportedDatabases { get; }
    
    string GetTemplateTreePath();
}

public interface IServiceGenerationEvents
{
    Task OnServiceGenerationStarted(DirectoryInfo serviceDirectory);
    Task OnServiceGenerationEnded(DirectoryInfo serviceDirectory);
}