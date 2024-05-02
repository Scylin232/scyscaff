using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using ScyScaff.CLI.Models.CLI;
using ScyScaff.CLI.Models.Exceptions;
using ScyScaff.CLI.Services.Builder;
using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Tests.Models;

namespace ScyScaff.Tests.Builder;

public class BootstrapTests
{
    private readonly MockFileSystem _mockFileSystem;
    private readonly Options _options;
    private readonly Bootstrap _bootstrap;

    private const string ConfigFilePath = "./TestDirectory/config.yml";
    
    public BootstrapTests()
    {
        // Arrange
        _mockFileSystem = new();
        
        string configContent = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "./Builder/Materials/BootstrapTestScaffolderConfiguration.txt"));
        
        _mockFileSystem.AddDirectory(Constants.TemplateTreePath);
        _mockFileSystem.AddFile($"{Constants.TemplateTreePath}Test.txt.liquid", new MockFileData("{{ config.project_name }}"));

        _mockFileSystem.AddDirectory("./TestDirectory/");
        _mockFileSystem.AddFile(ConfigFilePath, new MockFileData(configContent));
        
        Mock<IPluginGatherer> pluginGatherMock = new();

        pluginGatherMock.Setup(pg => pg.GatherPlugins<IFrameworkTemplatePlugin>(It.IsAny<IFileSystem>(), It.IsAny<string>(), It.IsAny<PluginType>())).Returns(() => new List<IFrameworkTemplatePlugin>
        {
            new TestFrameworkPlugin()
        });
        
        pluginGatherMock.Setup(pg => pg.GatherPlugins<IDashboardTemplatePlugin>(It.IsAny<IFileSystem>(), It.IsAny<string>(), It.IsAny<PluginType>())).Returns(() => new List<IDashboardTemplatePlugin>
        {
            new TestDashboardPlugin()
        });
        
        pluginGatherMock.Setup(pg => pg.GatherPlugins<IGlobalWorkerTemplatePlugin>(It.IsAny<IFileSystem>(), It.IsAny<string>(), It.IsAny<PluginType>())).Returns(() => new List<IGlobalWorkerTemplatePlugin>
        {
            new TestGlobalWorkerTemplatePlugin()
        });
        
        Mock<IPathGatherer> pathGathererMock = new();
        pathGathererMock.Setup(app => app.GetPluginTemplateTreePath(It.IsAny<ITemplatePlugin>()))
            .Returns(Constants.TemplateTreePath);
        
        Mock<IDownloader> downloaderMock = new();
        
        _options = new()
        {
            Add = true,
            Path = "./TestDirectory/",
            File = "config.yml"
        };

        _bootstrap = new(_mockFileSystem, pluginGatherMock.Object, pathGathererMock.Object, downloaderMock.Object, _options);
    }
    
    [Fact]
    public async Task StartGeneration_GenerationCompletedSuccessfully_Test()
    {
        // Act
        await _bootstrap.StartGeneration();
        
        // Assert
        string testFileContent = await _mockFileSystem.File.ReadAllTextAsync("./Test.txt");
        Assert.Equal(Constants.ProjectName, testFileContent);

        string dockerFileContent = await _mockFileSystem.File.ReadAllTextAsync("./TestDirectory/docker-compose.dev.yml");
        Assert.NotEqual(string.Empty, dockerFileContent);
    }

    [Fact]
    public async Task StartGeneration_FileNotFoundThrownSuccessfully_Test()
    {
        // Arrange
        _options.Path = "./UnknownPath/";
        
        // Act / Assert
        await Assert.ThrowsAsync<ConfigFileNotFoundException>(async () => await _bootstrap.StartGeneration());
    }

    [Fact]
    public async Task StartGeneration_DeserializeFailThrownSuccessfully_Test()
    {
        // Arrange
        _mockFileSystem.AddFile(ConfigFilePath, new MockFileData("$invalid_data$"));
        
        // Act / Assert
        await Assert.ThrowsAsync<ConfigDeserializeErrorException>(async () => await _bootstrap.StartGeneration());
    }

    [Fact]
    public async Task StartGeneration_ValidatorErrorThrownSuccessfully_Test()
    {
        // Arrange
        _mockFileSystem.AddFile(ConfigFilePath, new MockFileData("ProjectName: 123"));
        
        // Act / Assert
        await Assert.ThrowsAsync<ValidatorErrorException>(async () => await _bootstrap.StartGeneration());
    }
}