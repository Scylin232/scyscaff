using System.Reflection;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Utils.Plugins;

namespace ScyScaff.Core.Services.Plugins;

internal static class PluginLoader
{
    internal static Assembly LoadPlugin(string relativePath)
    {
        string root = Path.GetFullPath(
            Path.Combine(
                Enumerable.Range(0, 5)
                    .Aggregate(
                        Path.GetDirectoryName(typeof(Program).Assembly.Location),
                        (current, _) => Directory.GetParent(current).FullName)
            )
        );
        
        string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }

    internal static IEnumerable<IFrameworkPlugin> CreatePlugin(Assembly assembly)
    {
        int count = 0;
        
        foreach (Type type in assembly.GetTypes())
        {
            if (!typeof(IFrameworkPlugin).IsAssignableFrom(type)) continue;
            
            IFrameworkPlugin? result = Activator.CreateInstance(type) as IFrameworkPlugin;
            
            if (result == null) continue;
            
            count++;
                
            yield return result;
        }

        if (count != 0) yield break;
        
        string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        
        throw new ApplicationException(
            $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
            $"Available types: {availableTypes}");
    }
}