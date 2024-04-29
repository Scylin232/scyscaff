namespace ScyScaff.Core.Models.Parser;

public class ScaffolderConfig
{
    public string ProjectName { get; set; } = string.Empty;
    
    public string Auth { get; set; } = string.Empty;
    public ScaffolderDashboard? Dashboard { get; set; }
    
    public string DefaultFramework { get; set; } = string.Empty;
    public string DefaultDatabase { get; set; } = string.Empty;
    public Dictionary<string, string> DefaultServiceFlags { get; set; } = new();

    public List<ScaffolderGlobalWorker> GlobalWorkers { get; set; } = new();
    public Dictionary<string, ScaffolderService> Services { get; set; } = new();
}