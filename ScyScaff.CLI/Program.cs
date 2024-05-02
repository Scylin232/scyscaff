using System.IO.Abstractions;
using CommandLine;
using ScyScaff.CLI.Models.CLI;
using ScyScaff.CLI.Services.Builder;
using ScyScaff.CLI.Services.Plugins;
using ScyScaff.CLI.Utils.Application;

// Parse given arguments and start callback with input data (Serves as application entry point).
await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async options =>
{
    try
    {
        // Initialize services.
        FileSystem fileSystem = new();
        PluginGatherer pluginGatherer = new();
        PathGatherer pathGatherer = new();
        Downloader downloader = new();

        // Initialize bootstrap.
        Bootstrap bootstrap = new(fileSystem, pluginGatherer, pathGatherer, downloader, options);

        // Start generation.
        await bootstrap.StartGeneration();
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception.Message);
    }
});