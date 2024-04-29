using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Tests.Models;

public class TestGlobalWorkerTemplatePlugin : IGlobalWorkerTemplatePlugin
{
    public string Name { get; } = "TestGlobalWorker";

    public Dictionary<string, string[]> SupportedFlags { get; } = new()
    {
        { "TestFlagKey", new[] { "TestFlagValue" } }
    };
}