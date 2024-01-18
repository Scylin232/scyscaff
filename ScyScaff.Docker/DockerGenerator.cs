using Scriban;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Docker.Models.Builder;
using ScyScaff.Docker.Models.Plugins;

namespace ScyScaff.Docker;

public static class DockerGenerator
{
    public static void GenerateComposeServices(ScaffolderConfig config, string workingDirectory)
    {
        int serviceIndex = 0;
        List<DockerComposeService> composeServices = new();
        
        foreach (KeyValuePair<string, ScaffolderService> service in config.Services)
        {
            IDockerCompatible? dockerCompatible = service.Value.AssignedFrameworkPlugin as IDockerCompatible;
            
            if (dockerCompatible is null) continue;

            IEnumerable<DockerComposeService> generatedServices = dockerCompatible.GetComposeServices(config.ProjectName, service.Key, serviceIndex);
            
            composeServices.AddRange(generatedServices);
            
            serviceIndex++;
        }

        List<string> registeredVolumes = composeServices
            .Where(service => service.Volumes != null)
            .SelectMany(service => service.Volumes!.Keys)
            .ToList();
        
        Template dockerComposeTemplate = Template.Parse(File.ReadAllText("./Templates/docker-compose.liquid"));
        string dockerComposeResult = dockerComposeTemplate.Render(new
        {
            Services = composeServices,
            config.ProjectName,
            registeredVolumes,
        });
        
        File.WriteAllText(Path.Combine(workingDirectory, "docker-compose.dev.yml"), dockerComposeResult);
    }
}