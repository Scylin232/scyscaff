using System.IO.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.CLI;
using ScyScaff.Core.Models.Exceptions;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Services.Parser;
using ScyScaff.Core.Utils.Constants;
using ScyScaff.Docker;

namespace ScyScaff.Core.Services.Builder;

public class Bootstrap(IFileSystem fileSystem, IPluginGatherer pluginGatherer, IApplicationExit applicationExit, IDownloader downloader, Options options)
{
    private ScaffolderConfig _scaffolderConfig = new();

    private List<IFrameworkTemplatePlugin> _loadedFrameworkPlugins = new();
    private List<IDashboardTemplatePlugin> _loadedDashboardPlugins = new();
    private List<IGlobalWorkerTemplatePlugin> _loadedGlobalWorkerPlugins = new();
    
    private string _configFilePath = string.Empty;
    private string _workingDirectory = string.Empty;
    private string _dataFolderPath = string.Empty;

    // Application entry point called by Program.cs.
    public async Task StartGeneration()
    {
        await InitializeDataFolder();
        InitializeFiles();
        InitializePlugins();

        await ParseAndSetConfig();
        
        SetDefaultsAndValidate();
        
        await GenerateComponents();
    }

    private async Task InitializeDataFolder()
    {
        // We declare a variable that tracks whether the "Data Folder" has been created (Usually on first launch)
        bool dataFolderCreated = false;
        
        // Define the root data folder path
        _dataFolderPath = fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScyScaff");

        // Create the data folder if it doesn't exist
        if (!fileSystem.Directory.Exists(_dataFolderPath))
        {
            dataFolderCreated = true;
            fileSystem.Directory.CreateDirectory(_dataFolderPath);
        }

        // List of subdirectories to create under the main data folder
        string[] subDirectories = { "Plugins/" };

        // Create each subdirectory
        foreach (string subDirectory in subDirectories)
        {
            string subDirectoryPath = fileSystem.Path.Combine(_dataFolderPath, subDirectory);
            
            if (!fileSystem.Directory.Exists(subDirectoryPath))
                fileSystem.Directory.CreateDirectory(subDirectoryPath);
        }

        // If the folder has been created, then download the standard plugin package.
        if (dataFolderCreated)
        {
            Console.WriteLine("No plugins were found! Downloading the standard plugin package...");
            await downloader.DownloadDefaultPlugins(fileSystem, fileSystem.Path.Combine(_dataFolderPath, "Plugins/"));
        }
    }
    
    private void InitializeFiles()
    {
        // Get working directory and filename from arguments, otherwise use default values.
        _workingDirectory = options.Path ?? fileSystem.Directory.GetCurrentDirectory();
        string specifiedFile = options.File ?? Defaults.DefaultFilename;
    
        // Combine desired directory and filename to get the file path.
        _configFilePath = fileSystem.Path.Combine(_workingDirectory, specifiedFile);
    
        // Check if specified filepath exists.
        if (!fileSystem.File.Exists(_configFilePath))
            throw new ConfigFileNotFoundException(Messages.FileNotFoundInWorkingDirectory(specifiedFile, _workingDirectory));
    }

    private void InitializePlugins()
    {
        // Gather plugins, then store them in variables.
        string pluginsPath = fileSystem.Path.Combine(_dataFolderPath, "Plugins/");
        
        _loadedFrameworkPlugins = pluginGatherer.GatherPlugins<IFrameworkTemplatePlugin>(fileSystem, pluginsPath, PluginType.FrameworkPlugin);
        _loadedDashboardPlugins = pluginGatherer.GatherPlugins<IDashboardTemplatePlugin>(fileSystem, pluginsPath, PluginType.DashboardPlugin);
        _loadedGlobalWorkerPlugins = pluginGatherer.GatherPlugins<IGlobalWorkerTemplatePlugin>(fileSystem, pluginsPath, PluginType.GlobalWorkerPlugin);
    }

    private async Task ParseAndSetConfig()
    {
        // Read desired file content.
        string fileContent = await fileSystem.File.ReadAllTextAsync(_configFilePath);
    
        // Initialize YAML deserializer.
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
    
        try
        {
            // Try to deserialize.
            _scaffolderConfig = deserializer.Deserialize<ScaffolderConfig>(fileContent);
        }
        catch (Exception exception)
        {
            // Exit if parsing error was thrown.
            throw new ConfigDeserializeErrorException(exception.Message);
        }
    }

    private void SetDefaultsAndValidate()
    {
        // Set default values to null fields (if possible).
        DefaultsSetter.SetDefaults(_scaffolderConfig);
    
        // Ensure that config is fully valid.
        string? validationError = Validator.EnsureConfig(_scaffolderConfig, _loadedFrameworkPlugins, _loadedDashboardPlugins, _loadedGlobalWorkerPlugins);
    
        // Display an error message and exit program if config is not valid.
        if (validationError is not null)
            throw new ValidatorErrorException(validationError);
    }

    private async Task GenerateComponents()
    {
        // Initialize component generator.
        ComponentGenerator componentGenerator = new(fileSystem, applicationExit, _scaffolderConfig, _workingDirectory, options);
    
        // Finally! Generate services.
        foreach (KeyValuePair<string, ScaffolderService> service in _scaffolderConfig.Services)
            await componentGenerator.GenerateComponent(service.Value.AssignedFrameworkPlugin!, service.Key, service.Value);

        // And generate dashboard, if was specified.
        if (_scaffolderConfig.AssignedDashboardPlugin is not null)
            await componentGenerator.GenerateComponent(_scaffolderConfig.AssignedDashboardPlugin, "Dashboard");

        // And generate global workers, if was specified.
        foreach (IGlobalWorkerTemplatePlugin globalWorkerPlugin in _scaffolderConfig.AssignedGlobalWorkerPlugins)
            await componentGenerator.GenerateComponent(globalWorkerPlugin, "Global");
        
        // Generate docker-compose files from all services (if IDockerCompatible implemented).
        DockerGenerator.GenerateComposeServices(fileSystem, componentGenerator.ComposeServices, _scaffolderConfig.ProjectName, _workingDirectory);
    }
}