using System.IO.Abstractions;
using Scriban;
using ScyScaff.Docker.Models.Builder;

namespace ScyScaff.Docker;

public static class DockerGenerator
{
    // Generates a Docker-Compose file based on the received Compose services.
    public static void GenerateComposeServices(IFileSystem fileSystem, List<DockerComposeService> composeServices, string projectName, string workingDirectory)
    {
        // Get a list of keys of all Volumes.
        List<string> registeredVolumes = composeServices
            .Where(service => service.Volumes != null)
            .SelectMany(service => service.Volumes!.Keys)
            .ToList();
        
        // Parse & Render Docker-Compose template. 
        Template dockerComposeTemplate = Template.Parse(fileSystem.File.ReadAllText("./Templates/docker-compose.liquid"));
        string dockerComposeResult = dockerComposeTemplate.Render(new
        {
            Services = composeServices,
            projectName,
            registeredVolumes,
        });
        
        // Write the Docker-Compose file to the project folder
        fileSystem.File.WriteAllText(fileSystem.Path.Combine(workingDirectory, "docker-compose.dev.yml"), dockerComposeResult);
    }
}