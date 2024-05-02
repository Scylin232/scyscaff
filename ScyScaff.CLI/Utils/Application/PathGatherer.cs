using System.Reflection;
using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.CLI.Utils.Application;

// Represents a class for handling application exit.
public class PathGatherer : IPathGatherer
{
    // Receives plugin template tree path from its assembly.
    public string GetPluginTemplateTreePath(ITemplatePlugin plugin)
    {
        Assembly pluginAssembly = plugin.GetType().Assembly;
        
        return Path.Combine(Path.GetDirectoryName(pluginAssembly.Location)!, "TemplateTree/");
    }
}