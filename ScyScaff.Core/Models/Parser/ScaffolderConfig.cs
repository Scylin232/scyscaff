using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Parser;

public class ScaffolderConfig
{
    public string ProjectName { get; set; } = string.Empty;
    
    public string Auth { get; set; } = string.Empty;
    public string? Dashboard { get; set; } = null;
    
    public string DefaultFramework { get; set; } = string.Empty;
    public string DefaultDatabase { get; set; } = string.Empty;
    public Dictionary<string, string> DefaultFlags { get; set; } = new();

    public List<string> GlobalWorkers { get; set; } = new();
    public Dictionary<string, ScaffolderService> Services { get; set; } = new();
    
    // NOTE: These values are assigned at runtime (in Validator), so we can reference them later.
    public IDashboardTemplatePlugin? AssignedDashboardPlugin { get; set; }
    public List<IGlobalWorkerTemplatePlugin> AssignedGlobalWorkerPlugins { get; } = new();
}