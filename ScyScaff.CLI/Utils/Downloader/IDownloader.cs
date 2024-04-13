using System.IO.Abstractions;

namespace ScyScaff.Core.Utils.Downloader;

public interface IDownloader
{
    Task DownloadDefaultPlugins(IFileSystem fileSystem, string pluginsFolderPath);
}