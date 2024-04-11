using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Tests.Models;

public class TestGlobalWorkerPlugin : IGlobalWorkerPlugin
{
    public string GlobalWorkerName { get; } = "TestGlobalWorker";

    public string GetTemplateTreePath() => Constants.TemplateTreePath;
}