using System.Diagnostics;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Reflection;
using ScyScaff.Core.Models.Application;

namespace ScyScaff.Core.Utils.Application;

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
        // Retrieves the version of the executing assembly
        string assemblyVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion!;

        // Retrieves download URLs for standard plugins based on the assembly version
        Dictionary<string, string> downloadUrls = GetStandardPluginsDownloadUrls(assemblyVersion);

        // Initializes an HTTP client for downloading plugins
        using HttpClient client = new();
        
        // Iterates through each download URL and downloads the corresponding plugin
        foreach (KeyValuePair<string, string> downloadUrl in downloadUrls)
        {
            // Constructs the download path for the plugin
            string downloadPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), $"{downloadUrl.Key}.zip");
            
            // Downloads the plugin as a byte array
            byte[] downloadedPluginBytes = await client.GetByteArrayAsync(downloadUrl.Value);

            // Writes the downloaded plugin bytes to the specified download path
            await fileSystem.File.WriteAllBytesAsync(downloadPath, downloadedPluginBytes);

            // Constructs the destination path for extracting the plugin
            string destinationPath = fileSystem.Path.Combine(pluginsFolderPath, $"{downloadUrl.Key}/");

            // Extracts the downloaded zip file to the destination path
            ZipFile.ExtractToDirectory(downloadPath, destinationPath);

            // Deletes the downloaded zip file
            fileSystem.File.Delete(downloadPath);
        }
    }
}