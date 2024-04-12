using System.IO.Abstractions;
using System.Reflection;
using ScyScaff.Core.Utils.Constants;
using ScyScaff.Core.Utils.Plugins;

namespace ScyScaff.Core.Services.Plugins;

internal static class PluginLoader<T>
    where T : class
{
    public static List<T> ConstructPlugins(IFileSystem fileSystem, IEnumerable<string> pluginsAbsolutePath)
    {
        return pluginsAbsolutePath.SelectMany(pluginPath =>
        {
            Assembly pluginAssembly = LoadPlugin(fileSystem, pluginPath);
        
            return CreatePlugin(pluginAssembly);
        }).ToList();
    }
    
    private static Assembly LoadPlugin(IFileSystem fileSystem, string relativePath)
    {
        string root = fileSystem.Path.GetFullPath(fileSystem.Path.Combine(Enumerable.Range(0, 5).Aggregate(fileSystem.Path.GetDirectoryName(typeof(Program).Assembly.Location), (current, _) => fileSystem.Directory.GetParent(current).FullName)));
        
        string pluginLocation = fileSystem.Path.GetFullPath(fileSystem.Path.Combine(root, relativePath.Replace('\\', fileSystem.Path.DirectorySeparatorChar)));
        
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        
        return loadContext.LoadFromAssemblyName(new AssemblyName(fileSystem.Path.GetFileNameWithoutExtension(pluginLocation)));
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
        
        throw new ApplicationException(Messages.PluginImplementationNotFound(nameof(T), assembly.ToString(), assembly.Location, availableTypes));
    }
}