using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Tests.Models;

public class TestDashboardPlugin : IDashboardTemplatePlugin
{
    public string DashboardName { get; } = "TestDashboard";

    public string GetTemplateTreePath() => Constants.TemplateTreePath;
}