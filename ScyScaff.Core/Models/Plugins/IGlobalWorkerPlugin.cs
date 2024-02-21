namespace ScyScaff.Core.Models.Plugins;

public interface IGlobalWorkerPlugin : ITemplatePlugin
{
    string GlobalWorkerName { get; }
}
