using System.IO.Abstractions.TestingHelpers;
using ScyScaff.Docker;
using ScyScaff.Docker.Enums;
using ScyScaff.Docker.Models.Builder;

namespace ScyScaff.Tests.Docker;

public class DockerGeneratorTests
{
    [Fact]
    public void GenerateComposeServices_SuccessTemplateGeneration_Test()
    {
        // Arrange
        string expectedFileContent = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "./Docker/Materials/DockerComposeOutput.txt"));
        
        MockFileSystem mockFileSystem = new();
        
        mockFileSystem.AddDirectory(Constants.WorkingDirectory);

        List<DockerComposeService> composeServices = new()
        {
            new DockerComposeService
            {
                Type = DockerComposeServiceType.Framework,
                Image = "TestImage",
                Build = new ComposeBuild
                {
                    Context = "TestContext",
                    Dockerfile = "TestDockerFile"
                },
                ContainerName = "TestContainer",
                ExtraProperties = "TestExtraProperties",
                Ports = new Dictionary<int, int?>
                {
                    { 100, 200 }
                },
                Volumes = new Dictionary<string, string>
                {
                    { "TestVolumeA", "TestVolumeB" }
                },
                EnvironmentVariables = new Dictionary<string, string>
                {
                    { "TestVariableA", "TestVariableB" }
                },
                Dependencies = new Dictionary<string, ComposeDependency>
                {
                    {
                        "TestDependencyA", new ComposeDependency
                        {
                            Condition = "TestConditionA"
                        }
                    }
                }
            }
        };
        
        // Act
        DockerGenerator.GenerateComposeServices(mockFileSystem, composeServices, Constants.ProjectName, Constants.WorkingDirectory);
        
        // Assert
        string expectedFilePath = mockFileSystem.Path.Combine(Constants.WorkingDirectory, "docker-compose.dev.yml");
        Assert.True(mockFileSystem.File.Exists(expectedFilePath));

        string fileContent = mockFileSystem.File.ReadAllText(expectedFilePath);
        Assert.Equal(expectedFileContent, fileContent);
    }
}