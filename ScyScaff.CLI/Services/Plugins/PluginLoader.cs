using System.IO.Abstractions;
using System.Reflection;
using ScyScaff.CLI.Utils.Constants;
using ScyScaff.CLI.Utils.Plugins;

namespace ScyScaff.CLI.Services.Plugins;

// A generic static class responsible for loading and creating plugins
internal static class PluginLoader<T>
    where T : class
{
    // Constructs plugins of type T from the specified file system and plugin paths
    public static List<T> ConstructPlugins(IFileSystem fileSystem, IEnumerable<string> pluginsAbsolutePath)
    {
        // Selects each plugin path, loads its assembly, and creates plugins from it
        return pluginsAbsolutePath.SelectMany(pluginPath =>
        {
            Assembly pluginAssembly = LoadPlugin(fileSystem, pluginPath);
            
            return CreatePlugin(pluginAssembly);
        }).ToList();
    }
    
    // Loads the assembly of a plugin located at the specified path
    private static Assembly LoadPlugin(IFileSystem fileSystem, string relativePath)
    {
        // Resolves the root path based on the current executing assembly
        string root = fileSystem.Path.GetFullPath(fileSystem.Path.Combine(
            Enumerable.Range(0, 5).Aggregate(
                fileSystem.Path.GetDirectoryName(typeof(Program).Assembly.Location), 
                (current, _) => fileSystem.Directory.GetParent(current!)!.FullName
            )!
        ));
        
        // Constructs the absolute path of the plugin
        string pluginLocation = fileSystem.Path.GetFullPath(
            fileSystem.Path.Combine(root, relativePath.Replace('\\', fileSystem.Path.DirectorySeparatorChar))
        );
        
        // Creates a new plugin load context and loads the assembly
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        
        return loadContext.LoadFromAssemblyName(new AssemblyName(fileSystem.Path.GetFileNameWithoutExtension(pluginLocation)));
    }

    // Creates instances of plugins of type T from the specified assembly
    private static IEnumerable<T> CreatePlugin(Assembly assembly)
    {
        int count = 0;
        
        // Iterates through each type in the assembly
        foreach (Type type in assembly.GetTypes())
        {
            // Checks if the type is assignable to T
            if (!typeof(T).IsAssignableFrom(type)) continue;
            
            // Creates an instance of the plugin
            T? result = Activator.CreateInstance(type) as T;
            
            // Skips if the instance is null
            if (result == null) continue;
            
            count++; // Increments the count of created plugins
                
            yield return result; // Yields the created plugin
        }

        // If no plugin of type T is found in the assembly, throws an exception
        if (count != 0) yield break;
        
        // Constructs a string containing information about available types in the assembly
        string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        
        // Throws an application exception indicating that the plugin implementation was not found
        throw new ApplicationException(Messages.PluginImplementationNotFound(nameof(T), assembly.ToString(), assembly.Location, availableTypes));
    }
}