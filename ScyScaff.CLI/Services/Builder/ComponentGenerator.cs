using System.IO.Abstractions;
using ScyScaff.CLI.Models.CLI;
using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Docker.Models.Builder;
using ScyScaff.Docker.Models.Plugins;

namespace ScyScaff.CLI.Services.Builder;

public class ComponentGenerator(IFileSystem fileSystem, IPathGatherer pathGatherer, ScaffolderConfig config, string workingDirectory, Options options)
{
    // We store a list of Compose Services in order to pass it to ScyScaff.Docker at the end of generation
    // Also, we pass this list to Template files so that plugins that need it (for example, Grafana-Prometheus) can read them
    public readonly List<DockerComposeService> ComposeServices = new();
    
    // We store _entityIndex to make it convenient for docker services to calculate ports.
    private int _entityIndex;

    public async Task GenerateComponent(ITemplatePlugin plugin, string entityName, IScaffolderEntity scaffolderEntity)
    {
        // Add one to _entityIndex.
        _entityIndex++;

        // We declare the generation context to pass it to the generator.
        TreeGenerationContext generationContext = new TreeGenerationContext(
            fileSystem,
            pathGatherer,
            config,
            ComposeServices,
            scaffolderEntity,
            plugin,
            entityName,
            options.Add ?? false);
        
        // We get a Template Tree and generate files from it.
        await TemplateTreeGenerator.GenerateFromTree(generationContext, workingDirectory);
        
        // Try to get IDockerCompatible to generate a Docker Service.
        IDockerCompatible? dockerCompatible = plugin as IDockerCompatible;

        // If the plugin does not support IDockerCompatible, then we skip the Docker Service generation step.
        if (dockerCompatible is null) return;

        // We declare a list of all received Docker Services and store it in this variable.
        List<DockerComposeService> composeServices = dockerCompatible.GetComposeServices(config.ProjectName, scaffolderEntity, entityName, _entityIndex).ToList();

        // We bind a Scaffolder Service to each generated Docker Service so that plugins that need it can read this information.
        foreach (DockerComposeService composeService in composeServices)
            composeService.LinkedEntity = scaffolderEntity;
        
        // Add the generated Docker Services to the general list.
        ComposeServices.AddRange(composeServices);
    }
}