﻿using ScyScaff.CLI.Services.Parser;
using ScyScaff.CLI.Utils.Constants;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Tests.Models;

namespace ScyScaff.Tests.Builder;

public class ValidatorTests
{
    private readonly List<IFrameworkTemplatePlugin> _emptyFrameworkPlugins = new();
    private readonly List<IDashboardTemplatePlugin> _emptyDashboardPlugins = new();
    private readonly List<IGlobalWorkerTemplatePlugin> _emptyGlobalWorkerPlugins = new();
    
    private readonly List<IFrameworkTemplatePlugin> _filledFrameworkPlugins = new()
    {
        new TestFrameworkPlugin()
    };
    private readonly List<IDashboardTemplatePlugin> _filledDashboardPlugins = new()
    {
        new TestDashboardPlugin()
    };
    private readonly List<IGlobalWorkerTemplatePlugin> _filledGlobalWorkerPlugins = new()
    {
        new TestGlobalWorkerTemplatePlugin()
    };
    
    private readonly ScaffolderConfig _scaffolderConfig = new()
    {
        ProjectName = Constants.ProjectName,
        Auth = "TestAuth",
        Dashboard = new ScaffolderDashboard
        {
            Name = "TestDashboard"
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
                                { "TestField", "TestDataType" }
                            }
                        }
                    }
                }
            }
        }
    };
    
    [Fact]
    public void EnsureConfig_EmptyProjectNameCanceledSuccessfully_Test()
    {
        // Arrange
        _scaffolderConfig.ProjectName = string.Empty;

        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _emptyFrameworkPlugins, _emptyDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.ProjectNameEmptyError;
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void EnsureConfig_DashboardAssignedSuccessfully_Test()
    {
        // Act
        Validator.EnsureConfig(_scaffolderConfig, _emptyFrameworkPlugins, _filledDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Arrange
        Assert.NotNull(_scaffolderConfig.Dashboard!.DashboardTemplatePlugin);
    }

    [Fact]
    public void EnsureConfig_DashboardMissingCancelledSuccessfully_Test()
    {
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _emptyFrameworkPlugins, _emptyDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.DashboardMissing(Array.Empty<string>(), _scaffolderConfig.Dashboard!.Name);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void EnsureConfig_FrameworkAssignedSuccessfully_Test()
    {
        // Act
        Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        Assert.NotNull(_scaffolderConfig.Services["TestService"].AssignedFrameworkPlugin);
    }

    [Fact]
    public void EnsureConfig_FrameworkMissingCancelledSuccessfully_Test()
    {
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _emptyFrameworkPlugins, _filledDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.FrameworkMissing(Array.Empty<string>(), _scaffolderConfig.Services["TestService"].Framework!);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void EnsureConfig_FrameworkMissingAuthCancelledSuccessfully_Test()
    {
        // Arrange
        _scaffolderConfig.Auth = "Unknown Auth";
        
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.FrameworkAuthMissing(_filledFrameworkPlugins[0].SupportedAuth, _scaffolderConfig.Auth, _filledFrameworkPlugins[0].Name);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void EnsureConfig_FrameworkMissingDatabaseCancelledSuccessfully_Test()
    {
        // Arrange
        _scaffolderConfig.Services["TestService"].Database = "Unknown Database";
        
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.FrameworkDatabaseMissing(_filledFrameworkPlugins[0].SupportedDatabases, _scaffolderConfig.Services["TestService"].Database!, _filledFrameworkPlugins[0].Name);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void EnsureConfig_FrameworkMissingFlagKeyCancelledSuccessfully_Test()
    {
        // Arrange
        _scaffolderConfig.Services["TestService"].Flags.Add("UnknownFlagKey", string.Empty);
        
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.PluginFlagKeyNotSupported(_filledFrameworkPlugins[0].SupportedFlags.Keys, "UnknownFlagKey", _filledFrameworkPlugins[0].Name);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void EnsureConfig_FrameworkMissingFlagValueCancelledSuccessfully_Test()
    {
        // Arrange
        _scaffolderConfig.Services["TestService"].Flags["TestFlagKey"] = "UnknownFlagValue";
        
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.PluginFlagValueNotSupported(_filledFrameworkPlugins[0].SupportedFlags["TestFlagKey"], "TestFlagKey", "UnknownFlagValue", _filledFrameworkPlugins[0].Name);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void EnsureConfig_GlobalWorkersAssignedSuccessfully_Test()
    {
        // Act
        Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _filledGlobalWorkerPlugins);
        
        // Assert
        Assert.Single(_scaffolderConfig.GlobalWorkers.Select(gb => gb.GlobalWorkerTemplatePlugin));
    }

    [Fact]
    public void EnsureConfig_GlobalWorkerMissingCancelledSuccessfully_Test()
    {
        // Arrange
        _scaffolderConfig.GlobalWorkers = new List<ScaffolderGlobalWorker>
        {
            new()
            {
                Name = "UnknownGlobalWorker"
            }
        };
        
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _filledGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.GlobalWorkerMissing(_filledGlobalWorkerPlugins.Select(p => p.Name), "UnknownGlobalWorker");
        Assert.Equal(expectedResult, result);
    }
}