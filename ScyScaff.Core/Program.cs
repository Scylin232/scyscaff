using System.Reflection;
using CommandLine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ScyScaff.Core.Models.CLI;
using ScyScaff.Core.Models.Parsing;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Services;
using ScyScaff.Core.Utils;

// Parse given arguments and start callback with input data (Serves as application entry point).
await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async options =>
{
    // Get working directory and filename from arguments, otherwise use default values.
    string workingDirectory = options.Path ?? Directory.GetCurrentDirectory();
    string specifiedFile = options.File ?? Constants.DefaultFilename;

    // Combine desired directory and filename to get the file path.
    string filePath = Path.Combine(workingDirectory, specifiedFile);
    
    // Check if specified filepath exists.
    if (!File.Exists(filePath))
    {
        Console.WriteLine($"Could not find file {specifiedFile} at {workingDirectory}");
        Environment.Exit(-1);
    }
    
    string[] pluginPaths =
    {
        "D:\\dev\\CSharp\\ScyScaffPlugin.AspNet\\bin\\Debug\\net8.0\\ScyScaffPlugin.AspNet.dll"
    };

    // Load & Create plugins in memory, then store them in this variable.
    List<IFrameworkPlugin> loadedPlugins = pluginPaths.SelectMany(pluginPath =>
    {
        Assembly pluginAssembly = PluginLoader.LoadPlugin(pluginPath);
        
        return PluginLoader.CreatePlugin(pluginAssembly);
    }).ToList();
    
    // Read desired file content.
    string fileContent = await File.ReadAllTextAsync(filePath);

    // Initialize YAML deserializer.
    IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    // Initialize deserialized field.
    ScaffolderConfig? scaffolderConfig = null;
    
    try
    {
        // Try to deserialize.
        scaffolderConfig = deserializer.Deserialize<ScaffolderConfig>(fileContent);
    }
    catch (Exception exception)
    {
        // Exit if parsing error was thrown.
        Console.WriteLine(exception.Message);
        Environment.Exit(-1);
    }
    
    // Set default values to null fields (if possible).
    DefaultsSetter.SetDefaults(scaffolderConfig);
    
    // Ensure that config is fully valid.
    string? validationError = Validator.EnsureConfig(scaffolderConfig, loadedPlugins);

    // Display an error message and exit program if config is not valid.
    if (validationError is not null)
    {
        Console.WriteLine(validationError);
        Environment.Exit(-1);
    }

    foreach (KeyValuePair<string, Microservice> microservice in scaffolderConfig.Microservices)
        microservice.Value.AssignedFrameworkPlugin?.GenerateFrameworkFiles(
            workingDirectory,
            scaffolderConfig.ProjectName,
            microservice.Key, 
            microservice.Value
        );
});