using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Application;

public interface IApplication
{
    void ExitErrorCodeMinusOne();
    string GetPluginTemplateTreePath(ITemplatePlugin plugin);
}