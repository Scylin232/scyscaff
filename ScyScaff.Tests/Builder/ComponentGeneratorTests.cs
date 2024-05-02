using System.IO.Abstractions.TestingHelpers;
using Moq;
using ScyScaff.CLI.Models.CLI;
using ScyScaff.CLI.Models.Exceptions;
using ScyScaff.CLI.Services.Builder;
using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Tests.Models;

namespace ScyScaff.Tests.Builder;

public class ComponentGeneratorTests
{
    private readonly MockFileSystem _mockFileSystem = new();
    private readonly Mock<IPathGatherer> _application = new();
    
    private static readonly ScaffolderConfig ScaffolderConfig = new()
    {
        ProjectName = Constants.ProjectName,
        Auth = "TestAuth",
        Dashboard = new ScaffolderDashboard
        {
            Name = "TestDashboard"
        },
        DefaultFramework = "TestDefaultFramework",
        DefaultDatabase = "TestDefaultDatabase",
        DefaultServiceFlags = new Dictionary<string, string>
        {
            { "TestFlagKey", "TestFlagValue" }
        },
        GlobalWorkers = new List<ScaffolderGlobalWorker>
        {
            new()
            {
                Name = "TestGlobalWorker"
            }
        },
        Services = new Dictionary<string, ScaffolderService>
        {
            {
                "TestService", new ScaffolderService
                {
                    Framework = "TestFramework",
                    Database = "TestDatabase",
                    Flags = new Dictionary<string, string>
                    {
                        { "TestFlagKey", "TestFlagValue" }  
                    },
                    Models = new Dictionary<string, Dictionary<string, string>>
                    {
                        {
                            "TestModel", new Dictionary<string, string>
                            {
                                { "TestModelPropertyKey", "TestModelPropertyValue" }
                            }
                        }
                    }
                }
            }
        }
    };

    public ComponentGeneratorTests()
    {
        _application.Setup(app => app.GetPluginTemplateTreePath(It.IsAny<ITemplatePlugin>()))
            .Returns(Constants.TemplateTreePath);
    }

    [Fact]
    public async Task GenerateComponent_ComponentGeneratedSuccessfully_Test()
    {
        // Arrange
        Options options = new();
        TestFrameworkPlugin testFrameworkPlugin = new();

        string templateContent = "{{ config.project_name }} {{ entity.framework }}";
        
        _mockFileSystem.AddDirectory(Constants.TemplateTreePath);
        _mockFileSystem.AddFile($"{Constants.TemplateTreePath}Test.txt.liquid", new MockFileData(templateContent));
        
        ComponentGenerator componentGenerator = new(_mockFileSystem, _application.Object, ScaffolderConfig, Constants.WorkingDirectory, options);
        
        // Act
        await componentGenerator.GenerateComponent(testFrameworkPlugin, "Test", ScaffolderConfig.Services["TestService"]);
        
        // Assert
        string result = await _mockFileSystem.File.ReadAllTextAsync("./Test.txt");
        string expectedResult = $"{Constants.ProjectName} TestFramework";
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GenerateComponent_AddModeHandledSuccessfully_Test()
    {
        // Arrange
        Options options = new()
        {
            Add = true
        };
        TestFrameworkPlugin testFrameworkPlugin = new();

        string templateContent = @"
        // Custom Code...
        {{~ for field in entity.models[""TestModel""] ~}}
        public {{ field.value }} {{ field.key }} { get; set; }
        {{~ end ~}}";
        
        _mockFileSystem.AddDirectory(Constants.TemplateTreePath);
        _mockFileSystem.AddFile($"{Constants.TemplateTreePath}Test.txt.liquid", new MockFileData(templateContent));
        
        ComponentGenerator componentGenerator = new(_mockFileSystem, _application.Object, ScaffolderConfig, Constants.WorkingDirectory, options);
        
        // Act
        for (int i = 0; i < 2; i++)
        {
            ScaffolderConfig.Services["TestService"].Models["TestModel"].Add($"TestPropertyKey{i}", $"TestPropertyValue{i}");
            
            await componentGenerator.GenerateComponent(testFrameworkPlugin, "Test", ScaffolderConfig.Services["TestService"]);
        }
        
        // Assert
        string result = await _mockFileSystem.File.ReadAllTextAsync("./Test.txt");
        string expectedResult = "\n        // Custom Code...\n        public TestModelPropertyValue TestModelPropertyKey { get; set; }\n        public TestPropertyValue0 TestPropertyKey0 { get; set; }\n        public TestPropertyValue1 TestPropertyKey1 { get; set; }\n\n";
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GenerateComponent_AddModeUnmodifiedContentCancelledSuccessfully_Test()
    {
        // Arrange
        Options options = new()
        {
            Add = true
        };
        TestFrameworkPlugin testFrameworkPlugin = new();

        string templateContent = "{{ config.project_name }}";
        
        _mockFileSystem.AddDirectory(Constants.TemplateTreePath);
        _mockFileSystem.AddFile($"{Constants.TemplateTreePath}Test.txt.liquid", new MockFileData(templateContent));
        
        ComponentGenerator componentGenerator = new(_mockFileSystem, _application.Object, ScaffolderConfig, Constants.WorkingDirectory, options);

        // Act
        for (int i = 0; i < 2; i++)
            await componentGenerator.GenerateComponent(testFrameworkPlugin, "Test", ScaffolderConfig.Services["TestService"]);
        
        // Assert
        string result = await _mockFileSystem.File.ReadAllTextAsync("./Test.txt");
        string expectedResult = Constants.ProjectName;
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GenerateComponent_AddModeErrorTriggeredSuccessfully_Test()
    {
        // Arrange
        Options options = new();
        TestFrameworkPlugin testFrameworkPlugin = new();
        
        _mockFileSystem.AddDirectory(Constants.TemplateTreePath);
        _mockFileSystem.AddFile($"{Constants.TemplateTreePath}Test.txt.liquid", new MockFileData("Test Content"));

        ComponentGenerator componentGenerator = new(_mockFileSystem, _application.Object, ScaffolderConfig, Constants.WorkingDirectory, options);
        
        // Act
        await componentGenerator.GenerateComponent(testFrameworkPlugin, "Test", ScaffolderConfig.Services["TestService"]);
        
        // Assert
        await Assert.ThrowsAsync<AddModeNotEnabledException>(async () => await componentGenerator.GenerateComponent(testFrameworkPlugin, "Test", ScaffolderConfig.Services["TestService"]));
    }

    [Fact]
    public async Task GenerateComponent_ModelIteratorGeneratedSuccessfully_Test()
    {
        // Arrange
        Options options = new()
        {
            Add = true
        };
        TestFrameworkPlugin testFrameworkPlugin = new();

        string templateContent = "{{ model.key }}";
        
        _mockFileSystem.AddDirectory(Constants.TemplateTreePath);
        _mockFileSystem.AddEmptyFile($"{Constants.TemplateTreePath}[...modelIterator...]");
        
        _mockFileSystem.AddDirectory($"{Constants.TemplateTreePath}{{{{model.key}}}}");
        _mockFileSystem.AddFile($"{Constants.TemplateTreePath}{{{{model.key}}}}.txt.liquid", new MockFileData(templateContent));

        ComponentGenerator componentGenerator = new(_mockFileSystem, _application.Object, ScaffolderConfig, Constants.WorkingDirectory, options);
        
        // Act
        // Generation with service specification (Only service models will be used)
        await componentGenerator.GenerateComponent(testFrameworkPlugin, "Test", ScaffolderConfig.Services["TestService"]);
        // Generation without specifying a service (All configuration models will be used)
        await componentGenerator.GenerateComponent(testFrameworkPlugin, "Test", ScaffolderConfig.Dashboard!);
        
        // Assert
        bool directoryExists = _mockFileSystem.Directory.Exists("./TestModel/");
        Assert.True(directoryExists);
        
        string resultFile = await _mockFileSystem.File.ReadAllTextAsync("./TestModel.txt");
        string expectedResultFile = "TestModel";
        Assert.Equal(expectedResultFile, resultFile);
    }

    [Fact]
    public async Task GenerateComponent_DockerCompatibleEntityHandledSuccessfully_Test()
    {
        // Arrange
        Options options = new();
        TestFrameworkDockerPlugin testFrameworkDockerPlugin = new();

        _mockFileSystem.AddDirectory(Constants.TemplateTreePath);
        
        ComponentGenerator componentGenerator = new(_mockFileSystem, _application.Object, ScaffolderConfig, Constants.WorkingDirectory, options);
        
        // Act
        await componentGenerator.GenerateComponent(testFrameworkDockerPlugin, "Test", ScaffolderConfig.Services["TestService"]);
        
        // Assert
        Assert.Single(componentGenerator.ComposeServices);
    }
}