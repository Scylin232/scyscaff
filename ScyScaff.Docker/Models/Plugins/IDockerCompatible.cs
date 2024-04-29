using ScyScaff.Core.Models.Parser;
using ScyScaff.Docker.Models.Builder;

namespace ScyScaff.Docker.Models.Plugins;

public interface IDockerCompatible
{
    IEnumerable<DockerComposeService> GetComposeServices(string projectName, IScaffolderEntity scaffolderEntity, string serviceName, int serviceIndex);
}