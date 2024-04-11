using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Services.Parser;
using ScyScaff.Core.Utils.Constants;
using ScyScaff.Tests.Models;

namespace ScyScaff.Tests.Builder;

public class ValidatorTests
{
    private readonly List<IFrameworkTemplatePlugin> _emptyFrameworkPlugins = new();
    private readonly List<IDashboardTemplatePlugin> _emptyDashboardPlugins = new();
    private readonly List<IGlobalWorkerPlugin> _emptyGlobalWorkerPlugins = new();
    
    private readonly List<IFrameworkTemplatePlugin> _filledFrameworkPlugins = new()
    {
        new TestFrameworkPlugin()
    };
    private readonly List<IDashboardTemplatePlugin> _filledDashboardPlugins = new()
    {
        new TestDashboardPlugin()
    };
    private readonly List<IGlobalWorkerPlugin> _filledGlobalWorkerPlugins = new()
    {
        new TestGlobalWorkerPlugin()
    };
    
    private readonly ScaffolderConfig _scaffolderConfig = new()
    {
        ProjectName = Constants.ProjectName,
        Auth = "TestAuth",
        Dashboard = "TestDashboard",
        GlobalWorkers = new List<string>
        {
            "TestGlobalWorker"
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
        Assert.NotNull(_scaffolderConfig.AssignedDashboardPlugin);
    }

    [Fact]
    public void EnsureConfig_DashboardMissingCancelledSuccessfully_Test()
    {
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _emptyFrameworkPlugins, _emptyDashboardPlugins, _emptyGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.DashboardMissing(Array.Empty<string>(), _scaffolderConfig.Dashboard!);
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
        string expectedResult = Messages.FrameworkAuthMissing(_filledFrameworkPlugins[0].SupportedAuth, _scaffolderConfig.Auth, _filledFrameworkPlugins[0].FrameworkName);
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
        string expectedResult = Messages.FrameworkDatabaseMissing(_filledFrameworkPlugins[0].SupportedDatabases, _scaffolderConfig.Services["TestService"].Database!, _filledFrameworkPlugins[0].FrameworkName);
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
        string expectedResult = Messages.FrameworkFlagKeyNotSupported(_filledFrameworkPlugins[0].SupportedFlags.Keys, "UnknownFlagKey", _filledFrameworkPlugins[0].FrameworkName);
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
        string expectedResult = Messages.FrameworkFlagValueNotSupported(_filledFrameworkPlugins[0].SupportedFlags["TestFlagKey"], "TestFlagKey", "UnknownFlagValue", _filledFrameworkPlugins[0].FrameworkName);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void EnsureConfig_GlobalWorkersAssignedSuccessfully_Test()
    {
        // Act
        Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _filledGlobalWorkerPlugins);
        
        // Assert
        Assert.Single(_scaffolderConfig.AssignedGlobalWorkerPlugins);
    }

    [Fact]
    public void EnsureConfig_GlobalWorkerMissingCancelledSuccessfully_Test()
    {
        // Arrange
        _scaffolderConfig.GlobalWorkers = new List<string>
        {
            "UnknownGlobalWorker"
        };
        
        // Act
        string? result = Validator.EnsureConfig(_scaffolderConfig, _filledFrameworkPlugins, _filledDashboardPlugins, _filledGlobalWorkerPlugins);
        
        // Assert
        string expectedResult = Messages.GlobalWorkerMissing(_filledGlobalWorkerPlugins.Select(p => p.GlobalWorkerName), "UnknownGlobalWorker");
        Assert.Equal(expectedResult, result);
    }
}