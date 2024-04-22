namespace ScyScaff.Core.Models.Plugins;

// This class is used for JSON metadata of plugins (metadata.json) so that the loader (ScyScaff.CLI.PluginGatherer)
// can figure out what type of plugin it is and where to look for its DLL file path.
public class PluginMetadata
{
    public PluginType PluginType { get; set; } = PluginType.Unknown;
    public string? ExecutablePath { get; set; }
}