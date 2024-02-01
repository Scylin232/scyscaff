using System.Reflection;
using ScyScaff.Core.Utils.Plugins;

namespace ScyScaff.Core.Services.Plugins;

internal static class PluginLoader<T>
    where T : class
{
    public static List<T> ConstructPlugins(string[] pluginsAbsolutePath)
    {
        return pluginsAbsolutePath.SelectMany(pluginPath =>
        {
            Assembly pluginAssembly = LoadPlugin(pluginPath);
        
            return CreatePlugin(pluginAssembly);
        }).ToList();
    }
    
    private static Assembly LoadPlugin(string relativePath)
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

    private static IEnumerable<T> CreatePlugin(Assembly assembly)
    {
        int count = 0;
        
        foreach (Type type in assembly.GetTypes())
        {
            if (!typeof(T).IsAssignableFrom(type)) continue;
            
            T? result = Activator.CreateInstance(type) as T;
            
            if (result == null) continue;
            
            count++;
                
            yield return result;
        }

        if (count != 0) yield break;
        
        string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        
        throw new ApplicationException(
            $"Can't find any type which implements {nameof(T)} in {assembly} from {assembly.Location}.\n" +
            $"Available types: {availableTypes}");
    }
}