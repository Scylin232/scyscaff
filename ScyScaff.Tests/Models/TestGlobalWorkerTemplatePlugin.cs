using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Tests.Models;

public class TestGlobalWorkerTemplatePlugin : IGlobalWorkerTemplatePlugin
{
    public string GlobalWorkerName { get; } = "TestGlobalWorker";

    public string GetTemplateTreePath() => Constants.TemplateTreePath;
}