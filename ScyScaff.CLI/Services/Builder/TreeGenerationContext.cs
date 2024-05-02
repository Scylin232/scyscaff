using System.IO.Abstractions;
using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Docker.Models.Builder;

namespace ScyScaff.CLI.Services.Builder;

public class TreeGenerationContext(IFileSystem fileSystem, IPathGatherer pathGatherer, ScaffolderConfig config, List<DockerComposeService> composeServices, IScaffolderEntity scaffolderEntity, ITemplatePlugin templatePlugin, string entityName, bool isAddModeEnabled)
{
    public IFileSystem FileSystem { get; } = fileSystem;
    public IPathGatherer PathGatherer { get; } = pathGatherer;
    
    public ScaffolderConfig Config { get; } = config;
    public List<DockerComposeService> ComposeServices { get; } = composeServices;
    
    public ITemplatePlugin TemplatePlugin { get; } = templatePlugin;
    public int TemplateTreePathLength { get; set; }
    
    public IScaffolderEntity ScaffolderEntity { get; } = scaffolderEntity;
    public string EntityName { get; } = entityName;
    public string EntityDirectory { get; set; } = string.Empty;

    public bool IsAddModeEnabled { get; } = isAddModeEnabled;
}