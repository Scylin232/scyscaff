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

        ComposeServices.AddRange(dockerCompatible.GetComposeServices(config.ProjectName, entityName, _serviceIndex));
        
        _serviceIndex++;
    }
}