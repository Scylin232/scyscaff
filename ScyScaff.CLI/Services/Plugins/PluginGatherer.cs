using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.CLI.Services.Plugins;

public class PluginGatherer : IPluginGatherer
{
    // This method is used to load plugin implementations from a specified folder in a file system.
    // It uses generics to allow loading different types of plugins.
    public List<T> GatherPlugins<T>(IFileSystem fileSystem, string folder, PluginType pluginType)
        where T : class
    {
        // Initialize a list to store the paths to plugin executables.
        List<string> executablePaths = new List<string>();
        
        // Setup JsonSerializerOptions with a converter to handle enum strings properly.
        JsonSerializerOptions serializerOptions = new();
        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        
        // Retrieve all directories in the specified folder without searching subdirectories.
        IEnumerable<string> directories = fileSystem.Directory.EnumerateDirectories(folder, "*", SearchOption.TopDirectoryOnly);
        
        // Loop through each directory.
        foreach (string directory in directories)
        {
            // Combine the directory path with "metadata.json" to create the full path to the metadata file.
            string metadataPath = fileSystem.Path.Combine(directory, "metadata.json");
            
            // Continue to the next directory if the metadata file does not exist.
            if (!fileSystem.File.Exists(metadataPath)) continue;

            // Read the contents of the metadata file.
            string metadataContent = fileSystem.File.ReadAllText(metadataPath);
            // Deserialize the JSON content into a PluginMetadata object.
            PluginMetadata? parsedMetadata = JsonSerializer.Deserialize<PluginMetadata>(metadataContent, serializerOptions);

            // Skip this directory if metadata is not parsed, the plugin type doesn't match, or if no executable path is provided.
            if (parsedMetadata is null || parsedMetadata.PluginType != pluginType || string.IsNullOrEmpty(parsedMetadata.ExecutablePath)) continue;

            // Combine the directory path with the executable path from metadata and add to the list.
            executablePaths.Add(fileSystem.Path.Combine(directory, parsedMetadata.ExecutablePath));
        }
        
        // Load and construct plugin instances from the executable paths found.
        List<T> loadedPlugins = PluginLoader<T>.ConstructPlugins(fileSystem, executablePaths);

        // Return the list of loaded plugins.
        return loadedPlugins;
    }
}