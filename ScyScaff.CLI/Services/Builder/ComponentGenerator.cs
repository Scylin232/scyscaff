using System.IO.Abstractions;
using ScyScaff.Core.Models.CLI;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Models.Application;
using ScyScaff.Docker.Models.Builder;
using ScyScaff.Docker.Models.Plugins;

namespace ScyScaff.Core.Services.Builder;

public class ComponentGenerator(IFileSystem fileSystem, IApplicationExit applicationExit, ScaffolderConfig config, string workingDirectory, Options options)
{
    public readonly List<DockerComposeService> ComposeServices = new();
    
    private int _serviceIndex;

    public async Task GenerateComponent(ITemplatePlugin plugin, string entityName, ScaffolderService? service = default)
    {
        _serviceIndex++;

        TreeGenerationContext generationContext = new TreeGenerationContext(
            fileSystem,
            applicationExit,
            config,
            ComposeServices,
            service,
            plugin,
            entityName,
            options.Add);
        
        await TemplateTreeGenerator.GenerateFromTree(generationContext, workingDirectory);
        
        IDockerCompatible? dockerCompatible = plugin as IDockerCompatible;

        if (dockerCompatible is null) return;

        List<DockerComposeService> composeServices = dockerCompatible.GetComposeServices(config.ProjectName, service, entityName, _serviceIndex).ToList();

        foreach (DockerComposeService composeService in composeServices)
            composeService.LinkedService = service;
        
        ComposeServices.AddRange(composeServices);
    }
}