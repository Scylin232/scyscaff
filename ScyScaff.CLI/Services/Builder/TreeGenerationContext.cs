using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Services.Builder;

internal class TreeGenerationContext(ScaffolderConfig config, ScaffolderService? service,  ITemplatePlugin templatePlugin, string entityName)
{
    public ScaffolderConfig Config { get; } = config;
    public ScaffolderService? Service { get; } = service;
 
    public ITemplatePlugin TemplatePlugin { get; } = templatePlugin;

    public string EntityName { get; } = entityName;

    public string EntityDirectory { get; set; } = string.Empty;
    public int TemplateTreePathLength { get; set; }
}