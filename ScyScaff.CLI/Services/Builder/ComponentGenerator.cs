using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Docker.Models.Builder;
using ScyScaff.Docker.Models.Plugins;

namespace ScyScaff.Core.Services.Builder;

internal class ComponentGenerator(ScaffolderConfig config, string workingDirectory)
{
    internal readonly List<DockerComposeService> ComponentComposeServices = new();
    
    private int _serviceIndex;

    public async Task GenerateComponent(ITemplatePlugin plugin, string entityName, ScaffolderService? service = default)
    {
        TreeGenerationContext generationContext = new TreeGenerationContext(
            config,
            ComponentComposeServices,
            service,
            plugin,
            entityName);
        
        await TemplateTreeGenerator.GenerateFromTree(generationContext, workingDirectory);
        
        IDockerCompatible? dockerCompatible = plugin as IDockerCompatible;

        if (dockerCompatible is null) return;

        ComponentComposeServices.AddRange(dockerCompatible.GetComposeServices(config.ProjectName, entityName, _serviceIndex));
        _serviceIndex++;
    }
}