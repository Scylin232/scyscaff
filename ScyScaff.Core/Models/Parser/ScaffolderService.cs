﻿using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Parser;

public class ScaffolderService : IScaffolderEntity
{
    public string? Framework { get; set; }
    public string? Database { get; set; }
    public Dictionary<string, string> Flags { get; set; } = new();
    
    public Dictionary<string, Dictionary<string, string>> Models { get; set; } = new();
    
    // NOTE: This value is assigned at runtime (in Validator) so we can iterate over Services with reference to the appropriate plugin.
    public IFrameworkTemplatePlugin? AssignedFrameworkPlugin { get; set; }
}