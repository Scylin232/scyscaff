using ScyScaff.Core.Models.Parser;

namespace ScyScaff.Core.Services.Parser;

public static class DefaultsSetter
{
    public static void SetDefaults(ScaffolderConfig config)
    {
        // Set default values to fields that are null.
        foreach (KeyValuePair<string, ScaffolderService> service in config.Services)
        {
            service.Value.Database ??= config.DefaultDatabase;
            service.Value.Framework ??= config.DefaultFramework;
            service.Value.Flags = service.Value.Flags.Union(config.DefaultFlags).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}