using CommandLine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ScyScaff.Core.Models.CLI;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Utils.CLI;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Services.Builder;
using ScyScaff.Core.Services.Parser;
using ScyScaff.Core.Services.Plugins;
using ScyScaff.Docker;

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
    
    string[] frameworkPluginPaths =
    {
        "D:\\dev\\CSharp\\ScyScaffPlugin.AspNet\\bin\\Debug\\net8.0\\ScyScaffPlugin.AspNet.dll"
    };

    string[] dashboardPluginPaths =
    {
        "D:\\dev\\CSharp\\ScyScaffPlugin.NextCRUDDashboard\\ScyScaffPlugin.NextCRUDDashboard\\bin\\Debug\\net8.0\\ScyScaffPlugin.NextCRUDDashboard.dll"
    };
    
    // Load & Create plugins in memory, then store them in variables.
    List<IFrameworkTemplatePlugin> loadedFrameworkPlugins = PluginLoader<IFrameworkTemplatePlugin>.ConstructPlugins(frameworkPluginPaths);
    List<IDashboardTemplatePlugin> loadedDashboardPlugins = PluginLoader<IDashboardTemplatePlugin>.ConstructPlugins(dashboardPluginPaths);
    
    // Read desired file content.
    string fileContent = await File.ReadAllTextAsync(filePath);
    
    // Initialize YAML deserializer.
    IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance)
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
    string? validationError = Validator.EnsureConfig(scaffolderConfig, loadedFrameworkPlugins, loadedDashboardPlugins);
    
    // Display an error message and exit program if config is not valid.
    if (validationError is not null)
    {
        Console.WriteLine(validationError);
        Environment.Exit(-1);
    }
    
    // Finally! Generate services.
    foreach (KeyValuePair<string, ScaffolderService> service in scaffolderConfig.Services)
    {
        TreeGenerationContext serviceGenerationContext = new TreeGenerationContext(
            scaffolderConfig, 
            service.Value, 
            service.Value.AssignedFrameworkPlugin!, 
            service.Key);
    
        await TemplateTreeGenerator.GenerateFromTree(serviceGenerationContext, workingDirectory);
    }

    // And generate dashboard! If was specified.
    if (scaffolderConfig.AssignedDashboardPlugin is not null)
    {
        TreeGenerationContext dashboardGenerationContext = new TreeGenerationContext(
            scaffolderConfig,
            null,
            scaffolderConfig.AssignedDashboardPlugin,
            "Dashboard");

        await TemplateTreeGenerator.GenerateFromTree(dashboardGenerationContext, workingDirectory);
    }

    // Generate docker-compose files from all services (if IDockerCompatible implemented).
    DockerGenerator.GenerateComposeServices(scaffolderConfig, workingDirectory);
});