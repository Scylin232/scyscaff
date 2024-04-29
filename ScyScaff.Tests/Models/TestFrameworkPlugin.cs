using System.IO.Abstractions;
using ScyScaff.Core.Models.Events;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Tests.Models;

public class TestFrameworkPlugin : IFrameworkTemplatePlugin, IGenerationEvents
{
    public string Name { get; } = "TestFramework";
    
    public string[] SupportedAuth { get; } = { "TestAuth" };
    public string[] SupportedDatabases { get; } = { "TestDatabase" };

    public Dictionary<string, string[]> SupportedFlags { get; } = new()
    {
        { "TestFlagKey", new[] { "TestFlagValue" } }
    };
    
    public Task OnGenerationStarted(IDirectoryInfo serviceDirectory, IScaffolderEntity? entity) => Task.CompletedTask;
    public Task OnGenerationEnded(IDirectoryInfo serviceDirectory, IScaffolderEntity? entity) => Task.CompletedTask;
}