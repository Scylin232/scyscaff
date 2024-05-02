using System.Reflection;
using System.Runtime.Loader;

namespace ScyScaff.CLI.Utils.Plugins;

// Initializes a new instance of the PluginLoadContext class
// with the provided plugin path.
internal class PluginLoadContext(string pluginPath) : AssemblyLoadContext
{
    // Represents a custom assembly load context for loading plugins
    // based on the specified plugin path.
    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

    // Loads the specified assembly.
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // Resolves the path of the assembly based on its name.
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

        // Loads the assembly if its path is resolved, otherwise returns null.
        return assemblyPath is not null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    // Loads the specified unmanaged DLL.
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        // Resolves the path of the unmanaged DLL based on its name.
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        
        // Loads the unmanaged DLL if its path is resolved, otherwise returns IntPtr.Zero.
        return libraryPath is not null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}