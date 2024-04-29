using System.Reflection;
using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Utils.Application;

// Represents a class for handling application exit.
public class Application : IApplication
{
    // Exits the application with the exit code -1.
    public void ExitErrorCodeMinusOne() => Environment.Exit(-1);
    
    // Receives plugin template tree path from its assembly.
    public string GetPluginTemplateTreePath(ITemplatePlugin plugin)
    {
        Assembly pluginAssembly = plugin.GetType().Assembly;
        
        return Path.Combine(Path.GetDirectoryName(pluginAssembly.Location)!, "TemplateTree\\");
    }
}