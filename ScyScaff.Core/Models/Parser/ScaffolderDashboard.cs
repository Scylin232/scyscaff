using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Parser;

public class ScaffolderDashboard : IScaffolderEntity
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Flags { get; set; } = new();
    
    // NOTE: This value is assigned at runtime (in Validator) so we can access Dashboard with reference to the appropriate plugin.
    public IDashboardTemplatePlugin? DashboardTemplatePlugin { get; set; }
}