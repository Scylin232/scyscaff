using System.Text.RegularExpressions;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Services.Parser;

internal static class Validator
{
    internal static string? EnsureConfig(
        ScaffolderConfig config,
        List<IFrameworkTemplatePlugin> loadedFrameworkPlugins,
        List<IDashboardTemplatePlugin> loadedDashboardPlugins,
        List<IGlobalWorkerPlugin> loadedGlobalWorkerPlugins)
    {
        // Check if project name is not empty and contains latin letters only.
        if (config.ProjectName.Length <= 0 || !Regex.IsMatch(config.ProjectName, @"^[a-zA-Z]+$"))
            return "Project name can't be empty and can contain latin letters only.";

        // Check if specified dashboard specified and exists.
        if (config.Dashboard is not null)
        {
            IDashboardTemplatePlugin? foundDashboard = loadedDashboardPlugins.Find(plugin => plugin.DashboardName == config.Dashboard);
        
            if (foundDashboard is null)
                return $"Dashboard {config.Dashboard} was not found.";

            config.AssignedDashboardPlugin = foundDashboard;
        }
        
        // Check if framework: exists, supports selected auth, supports selected database, supports specified flags.
        foreach (KeyValuePair<string, ScaffolderService> service in config.Services)
        {
            IFrameworkTemplatePlugin? foundFramework = loadedFrameworkPlugins.Find(plugin => plugin.FrameworkName == service.Value.Framework);

            if (foundFramework is null)
                return $"Framework {service.Value.Framework} was not found.";

            if (!foundFramework.SupportedAuth.Contains(config.Auth))
                return $"Framework {foundFramework.FrameworkName} does not support {config.Auth} auth.";

            if (!foundFramework.SupportedDatabases.Contains(service.Value.Database))
                return $"Framework {foundFramework.FrameworkName} does not support {service.Value.Database} database.";

            foreach (string flag in service.Value.Flags.Keys.Where(flag => !foundFramework.SupportedFlags.Contains(flag)))
                Console.WriteLine($"Flag {flag} is not supported by {foundFramework.FrameworkName}, be aware.");
            
            service.Value.AssignedFrameworkPlugin = foundFramework;
        }

        // Check if global worker: exists.
        foreach (string globalWorkerName in config.GlobalWorkers)
        {
            IGlobalWorkerPlugin? foundGlobalWorker = loadedGlobalWorkerPlugins.Find(plugin => plugin.GlobalWorkerName == globalWorkerName);

            if (foundGlobalWorker is null)
                return $"Global worker {globalWorkerName} was not found.";
        }
        
        // Return null if no errors was found.
        return null;
    }
}