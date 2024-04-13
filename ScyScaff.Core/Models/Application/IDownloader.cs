using System.IO.Abstractions;

namespace ScyScaff.Core.Models.Application;

public interface IDownloader
{
    Task DownloadDefaultPlugins(IFileSystem fileSystem, string pluginsFolderPath);
}