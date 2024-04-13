using System.IO.Abstractions;
using CommandLine;
using ScyScaff.Core.Models.CLI;
using ScyScaff.Core.Services.Builder;
using ScyScaff.Core.Services.Plugins;
using ScyScaff.Core.Utils.Application;

// Parse given arguments and start callback with input data (Serves as application entry point).
await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async options =>
{
    // Initialize services.
    FileSystem fileSystem = new();
    PluginGatherer pluginGatherer = new();
    ApplicationExit applicationExit = new();
    Downloader downloader = new();
    
    // Initialize bootstrap.
    Bootstrap bootstrap = new(fileSystem, pluginGatherer, applicationExit, downloader, options);

    // Start generation.
    await bootstrap.StartGeneration();
});