using ScyScaff.Docker.Models.Builder;
using ScyScaff.Docker.Models.Plugins;

namespace ScyScaff.Tests.Models;

public class TestFrameworkDockerPlugin : TestFrameworkPlugin, IDockerCompatible
{
    public IEnumerable<DockerComposeService> GetComposeServices(string projectName, string serviceName, int serviceIndex)
    {
        List<DockerComposeService> dockerComposeServices = new();
        
        dockerComposeServices.Add(new DockerComposeService
        {
            Image = "TestImage"
        });

        return dockerComposeServices;
    }
}