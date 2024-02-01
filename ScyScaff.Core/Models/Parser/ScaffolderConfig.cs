using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Parser;

public class ScaffolderConfig
{
    public string ProjectName { get; set; } = string.Empty;
    
    public string Auth { get; set; } = string.Empty;
    public string? Dashboard { get; set; } = null;
    
    public string DefaultFramework { get; set; } = string.Empty;
    public string DefaultDatabase { get; set; } = string.Empty;
    
    public Dictionary<string, ScaffolderService> Services { get; set; } = new();
    
    // NOTE: This value is assigned at runtime (in Validator) so we can reference to the appropriate plugin.
    public IDashboardTemplatePlugin? AssignedDashboardPlugin { get; set; }
}