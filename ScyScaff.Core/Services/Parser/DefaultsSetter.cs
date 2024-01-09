using ScyScaff.Core.Models.Parser;

namespace ScyScaff.Core.Services.Parser;

internal static class DefaultsSetter
{
    internal static void SetDefaults(ScaffolderConfig config)
    {
        // Set default values to fields that are null.
        foreach (KeyValuePair<string, ScaffolderService> service in config.Services)
        {
            service.Value.Database ??= config.DefaultDatabase;
            service.Value.Framework ??= config.DefaultFramework;
        }
    }
}