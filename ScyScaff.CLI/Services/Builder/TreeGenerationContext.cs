using System.IO.Abstractions;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Models.Application;
using ScyScaff.Docker.Models.Builder;

namespace ScyScaff.Core.Services.Builder;

public class TreeGenerationContext(IFileSystem fileSystem, IApplicationExit applicationExit, ScaffolderConfig config, List<DockerComposeService> composeServices, ScaffolderService? service, ITemplatePlugin templatePlugin, string entityName, bool? isAddModeEnabled)
{
    public IFileSystem FileSystem { get; } = fileSystem;
    public IApplicationExit ApplicationExit { get; } = applicationExit;
    
    public ScaffolderConfig Config { get; } = config;
    public List<DockerComposeService> ComposeServices { get; } = composeServices;
    
    public ScaffolderService? Service { get; } = service;
 
    public ITemplatePlugin TemplatePlugin { get; } = templatePlugin;

    public string EntityName { get; } = entityName;

    public string EntityDirectory { get; set; } = string.Empty;
    public int TemplateTreePathLength { get; set; }

    public bool? IsAddModeEnabled { get; } = isAddModeEnabled;
}