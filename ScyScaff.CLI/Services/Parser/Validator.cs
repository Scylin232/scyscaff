using System.Text.RegularExpressions;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Services.Parser;

internal static class Validator
{
    internal static string? EnsureConfig(ScaffolderConfig config, List<IFrameworkPlugin> loadedPlugins)
    {
        // Check if project name is not empty and contains latin letters only.
        if (config.ProjectName.Length <= 0 || !Regex.IsMatch(config.ProjectName, @"^[a-zA-Z]+$"))
            return "Project name can't be empty and can contain latin letters only.";

        // Check if framework: exists, supports selected auth, supports selected database.
        foreach (KeyValuePair<string, ScaffolderService> service in config.Services)
        {
            IFrameworkPlugin? foundFramework = loadedPlugins.Find(plugin => plugin.FrameworkName == service.Value.Framework);

            if (foundFramework is null)
                return $"Framework {service.Value.Framework} was not found.";

            if (!foundFramework.SupportedAuth.Contains(config.Auth))
                return $"Framework {foundFramework.FrameworkName} does not support {config.Auth} auth.";

            if (!foundFramework.SupportedDatabases.Contains(service.Value.Database))
                return $"Framework {foundFramework.FrameworkName} does not support {service.Value.Database} database.";

            service.Value.AssignedFrameworkPlugin = foundFramework;
        }
        
        // Return null if no errors was found.
        return null;
    }
}