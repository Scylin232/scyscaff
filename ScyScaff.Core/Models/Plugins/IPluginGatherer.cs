using System.IO.Abstractions;

namespace ScyScaff.Core.Models.Plugins;

public interface IPluginGatherer
{
    public List<T> GatherPlugins<T>(IFileSystem fileSystem, string folder, PluginType pluginType)
        where T : class;
}