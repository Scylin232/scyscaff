namespace ScyScaff.Core.Models.Plugins;

// Enum of all plugin types, needed for JSON metadata (PluginMetadata.cs).
public enum PluginType
{
    Unknown,
    FrameworkPlugin,
    DashboardPlugin,
    GlobalWorkerPlugin
}