using System.Diagnostics;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Reflection;
using ScyScaff.Core.Models.Application;

namespace ScyScaff.Core.Utils.Downloader;

public class Downloader : IDownloader
{
    // The best solution would be to create a separate web server for this, which would accept requests and return a list of links, depending on the version.
    // But now, due to my social status, I do not have the physical ability to pay for anything on the Internet, sorry :P
    
    // We use a function instead of static dictionary, since small updates may not break support for old plugins,
    // so several versions can use the same download links.
    private static Dictionary<string, string> GetStandardPluginsDownloadUrls(string version)
    {
        if (version == "1.0.0.0")
            return new Dictionary<string, string>
            {
                { "aspnet-ddd", "" },
                { "svelte-crud", "" },
                { "elk", "" },
                { "grafana-prometheus", "" }
            };
        
        return new Dictionary<string, string>();
    }
    
    public async Task DownloadDefaultPlugins(IFileSystem fileSystem, string pluginsFolderPath)
    {
        string assemblyVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion!;
        Dictionary<string, string> downloadUrls = GetStandardPluginsDownloadUrls(assemblyVersion);

        using HttpClient client = new();
        
        foreach (KeyValuePair<string, string> downloadUrl in downloadUrls)
        {
            string downloadPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), $"{downloadUrl.Key}.zip");
            
            byte[] downloadedPluginBytes = await client.GetByteArrayAsync(downloadUrl.Value);
            await fileSystem.File.WriteAllBytesAsync(downloadPath, downloadedPluginBytes);

            string destinationPath = fileSystem.Path.Combine(pluginsFolderPath, $"{downloadUrl.Key}/");
            ZipFile.ExtractToDirectory(downloadPath, destinationPath);

            fileSystem.File.Delete(downloadPath);
        }
    }
}