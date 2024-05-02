using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Application;

public interface IPathGatherer
{
    string GetPluginTemplateTreePath(ITemplatePlugin plugin);
}