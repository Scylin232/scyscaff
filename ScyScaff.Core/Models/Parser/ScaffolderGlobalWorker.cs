using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Parser;

public class ScaffolderGlobalWorker : IScaffolderEntity
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Flags { get; set; } = new();
    
    // NOTE: This value is assigned at runtime (in Validator) so we can iterate over Global Workers with reference to the appropriate plugin.
    public IGlobalWorkerTemplatePlugin? GlobalWorkerTemplatePlugin { get; set; }
}