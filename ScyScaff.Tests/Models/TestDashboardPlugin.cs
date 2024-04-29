using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Tests.Models;

public class TestDashboardPlugin : IDashboardTemplatePlugin
{
    public string Name { get; } = "TestDashboard";

    public Dictionary<string, string[]> SupportedFlags { get; } = new()
    {
        { "TestFlagKey", new[] { "TestFlagValue" } }
    };
}