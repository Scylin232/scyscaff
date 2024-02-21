using Scriban;
using ScyScaff.Docker.Models.Builder;

namespace ScyScaff.Docker;

public static class DockerGenerator
{
    public static void GenerateComposeServices(List<DockerComposeService> composeServices, string projectName, string workingDirectory)
    {
        List<string> registeredVolumes = composeServices
            .Where(service => service.Volumes != null)
            .SelectMany(service => service.Volumes!.Keys)
            .ToList();
        
        Template dockerComposeTemplate = Template.Parse(File.ReadAllText("./Templates/docker-compose.liquid"));
        string dockerComposeResult = dockerComposeTemplate.Render(new
        {
            Services = composeServices,
            projectName,
            registeredVolumes,
        });
        
        File.WriteAllText(Path.Combine(workingDirectory, "docker-compose.dev.yml"), dockerComposeResult);
    }
}