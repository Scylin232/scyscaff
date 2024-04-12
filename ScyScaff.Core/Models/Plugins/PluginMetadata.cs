namespace ScyScaff.Core.Models.Plugins;

public class PluginMetadata
{
    public PluginType PluginType { get; set; } = PluginType.Unknown;
    public string? ExecutablePath { get; set; }
}