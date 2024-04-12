using System.IO.Abstractions.TestingHelpers;
using Moq;
using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.CLI;
using ScyScaff.Core.Models.Exceptions;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Services.Builder;
using ScyScaff.Tests.Models;

namespace ScyScaff.Tests.Builder;

public class BootstrapTests
{
    private readonly MockFileSystem _mockFileSystem;
    
    private readonly Mock<IPluginGatherer> _pluginGatherMock;
    private readonly Mock<IApplicationExit> _applicationExitMock;
    
    private readonly Options _options;
    
    private readonly Bootstrap _bootstrap;
    
    public BootstrapTests()
    {
        // Arrange
        _mockFileSystem = new();
        
        string dockerTemplateContent = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "./Templates/docker-compose.liquid"));
        
        _mockFileSystem.AddDirectory("./ExampleTemplateTree/");
        _mockFileSystem.AddFile("./ExampleTemplateTree/Test.txt.liquid", new MockFileData("{{ config.project_name }}"));

        _mockFileSystem.AddDirectory("./TestDirectory/");
        _mockFileSystem.AddFile("./TestDirectory/config.yml", new MockFileData("ProjectName: TestProject\n\nAuth: TestAuth\nDashboard: TestDashboard\n\nGlobalWorkers:\n  - TestGlobalWorker\n\nServices:\n  TestService:\n    Framework: TestFramework\n    Database: TestDatabase\n    Flags:\n      TestFlagKey: TestFlagValue\n    Models:\n      TestModel:\n        TestPropertyKey: TestPropertyValue"));
        
        _mockFileSystem.AddFile("./Templates/docker-compose.liquid", new MockFileData(dockerTemplateContent));
        
        _pluginGatherMock = new();

        _pluginGatherMock.Setup(pg => pg.GatherFrameworkPlugins()).Returns(() => new List<IFrameworkTemplatePlugin>
        {
            new TestFrameworkPlugin()
        });
        
        _pluginGatherMock.Setup(pg => pg.GatherDashboardPlugins()).Returns(() => new List<IDashboardTemplatePlugin>
        {
            new TestDashboardPlugin()
        });
        
        _pluginGatherMock.Setup(pg => pg.GatherGlobalWorkerPlugins()).Returns(() => new List<IGlobalWorkerPlugin>
        {
            new TestGlobalWorkerPlugin()
        });
        
        _applicationExitMock = new();
        
        _options = new()
        {
            Add = false,
            Path = "./TestDirectory/",
            File = "config.yml"
        };

        _bootstrap = new(_mockFileSystem, _pluginGatherMock.Object, _applicationExitMock.Object, _options);
    }
    
    [Fact]
    public async Task StartGeneration_GenerationCompletedSuccessfully_Test()
    {
        // Act
        await _bootstrap.StartGeneration();
        
        // Assert
        string testFileContent = await _mockFileSystem.File.ReadAllTextAsync("./Test.txt");
        Assert.Equal("TestProject", testFileContent);

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
        _mockFileSystem.AddFile("./TestDirectory/config.yml", new MockFileData("$invalid_data$"));
        
        // Act / Assert
        await Assert.ThrowsAsync<ConfigDeserializeErrorException>(async () => await _bootstrap.StartGeneration());
    }

    [Fact]
    public async Task StartGeneration_ValidatorErrorThrownSuccessfully_Test()
    {
        // Arrange
        _mockFileSystem.AddFile("./TestDirectory/config.yml", new MockFileData("ProjectName: 123"));
        
        // Act / Assert
        await Assert.ThrowsAsync<ValidatorErrorException>(async () => await _bootstrap.StartGeneration());
    }
}