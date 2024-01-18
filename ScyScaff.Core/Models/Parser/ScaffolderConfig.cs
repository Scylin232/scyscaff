namespace ScyScaff.Core.Models.Parser;

public class ScaffolderConfig
{
    public string ProjectName { get; set; } = string.Empty;
    
    public string Auth { get; set; } = string.Empty;
    public string DefaultFramework { get; set; } = string.Empty;
    public string DefaultDatabase { get; set; } = string.Empty;
    
    public Dictionary<string, ScaffolderService> Services { get; set; } = new();
}