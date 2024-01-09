using ScyScaff.Core.Models.Parsing;

namespace ScyScaff.Core.Services;

internal static class DefaultsSetter
{
    internal static void SetDefaults(ScaffolderConfig config)
    {
        // Set default values to fields that are null.
        foreach (KeyValuePair<string, Microservice> microservice in config.Microservices)
        {
            microservice.Value.Database ??= config.DefaultDatabase;
            microservice.Value.Framework ??= config.DefaultFramework;
        }
    }
}