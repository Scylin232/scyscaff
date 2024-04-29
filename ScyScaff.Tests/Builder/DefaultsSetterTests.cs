using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Services.Parser;

namespace ScyScaff.Tests.Builder;

public class DefaultsSetterTests
{
    [Fact]
    public void SetDefaults_DefaultsWereSetSuccessfully_Test()
    {
        // Arrange
        ScaffolderConfig scaffolderConfig = new()
        {
            DefaultDatabase = "DefaultDatabase",
            DefaultFramework = "DefaultFramework",
            DefaultServiceFlags = new Dictionary<string, string>
            {
                { "DefaultFlagKey", "DefaultFlagValue" }
            },
            Services = new Dictionary<string, ScaffolderService>
            {
                {
                    "TestService",
                    new ScaffolderService()
                }
            }
        };
        
        // Act
        DefaultsSetter.SetDefaults(scaffolderConfig);
        
        // Assert
        ScaffolderService service = scaffolderConfig.Services["TestService"];
        
        Assert.Equal(scaffolderConfig.DefaultDatabase, service.Database);
        Assert.Equal(scaffolderConfig.DefaultFramework, service.Framework);
        Assert.Equal(scaffolderConfig.DefaultServiceFlags, service.Flags);
    }
}