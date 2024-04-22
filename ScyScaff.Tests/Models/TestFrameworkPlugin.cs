using System.IO.Abstractions;
using ScyScaff.Core.Models.Events;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Tests.Models;

public class TestFrameworkPlugin : IFrameworkTemplatePlugin, ITemplateGenerationEvents
{
    public string FrameworkName { get; } = "TestFramework";
    public string[] SupportedAuth { get; } = { "TestAuth" };
    public string[] SupportedDatabases { get; } = { "TestDatabase" };

    public Dictionary<string, string[]> SupportedFlags { get; } = new()
    {
        { "TestFlagKey", new[] { "TestFlagValue" } }
    };
    
    public string GetTemplateTreePath() => Constants.TemplateTreePath;
    
    public Task OnServiceGenerationStarted(IDirectoryInfo serviceDirectory, ScaffolderService? scaffolderService) => Task.CompletedTask;
    public Task OnServiceGenerationEnded(IDirectoryInfo serviceDirectory, ScaffolderService? scaffolderService) => Task.CompletedTask;
}