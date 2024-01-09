using ScyScaff.Core.Enums;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Parsing;

public class Microservice
{
    public string? Framework { get; set; }
    public string? Database { get; set; }
    
    public Dictionary<string, FieldTypeProvider> Fields { get; set; } = new();
    
    // NOTE: This value is assigned at runtime (in Validator) so we can iterate over microservices with reference to the appropriate plugin.
    public IFrameworkPlugin? AssignedFrameworkPlugin { get; set; }
}