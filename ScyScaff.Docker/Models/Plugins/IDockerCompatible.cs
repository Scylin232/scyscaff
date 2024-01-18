using ScyScaff.Docker.Models.Builder;

namespace ScyScaff.Docker.Models.Plugins;

public interface IDockerCompatible
{
    IEnumerable<DockerComposeService> GetComposeServices(string projectName, string serviceName, int serviceIndex);
}