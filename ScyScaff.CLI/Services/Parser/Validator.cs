using System.Text.RegularExpressions;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Utils.Constants;

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
            return Messages.ProjectNameEmptyError;

        // Check if specified dashboard specified and exists.
        if (config.Dashboard is not null)
        {
            IDashboardTemplatePlugin? foundDashboard = loadedDashboardPlugins.Find(plugin => plugin.DashboardName == config.Dashboard);
        
            if (foundDashboard is null)
                return Messages.DashboardMissing(loadedDashboardPlugins.Select(plugin => plugin.DashboardName), config.Dashboard);

            config.AssignedDashboardPlugin = foundDashboard;
        }
        
        // Check if framework: exists, supports selected auth, supports selected database, supports specified flags.
        foreach (KeyValuePair<string, ScaffolderService> service in config.Services)
        {
            IFrameworkTemplatePlugin? foundFramework = loadedFrameworkPlugins.Find(plugin => plugin.FrameworkName == service.Value.Framework);

            if (foundFramework is null)
                return Messages.FrameworkMissing(loadedFrameworkPlugins.Select(plugin => plugin.FrameworkName), service.Value.Framework!);

            if (!foundFramework.SupportedAuth.Contains(config.Auth))
                return Messages.FrameworkAuthMissing(foundFramework.SupportedAuth, config.Auth, foundFramework.FrameworkName);

            if (!foundFramework.SupportedDatabases.Contains(service.Value.Database))
                return Messages.FrameworkDatabaseMissing(foundFramework.SupportedDatabases, service.Value.Database!, foundFramework.FrameworkName);

            foreach (KeyValuePair<string, string> flag in service.Value.Flags)
            {
                if (!foundFramework.SupportedFlags.ContainsKey(flag.Key))
                {
                    Console.WriteLine(Messages.FrameworkFlagKeyNotSupported(foundFramework.SupportedFlags.Keys, flag.Key, foundFramework.FrameworkName));
                    continue;
                }

                if (!foundFramework.SupportedFlags[flag.Key].Contains(flag.Value))
                    Console.WriteLine(Messages.FrameworkFlagValueNotSupported(foundFramework.SupportedFlags[flag.Key], flag.Key, flag.Value, foundFramework.FrameworkName));
            }
    
            service.Value.AssignedFrameworkPlugin = foundFramework;
        }

        // Check if global worker: exists.
        foreach (string globalWorkerName in config.GlobalWorkers)
        {
            IGlobalWorkerPlugin? foundGlobalWorker = loadedGlobalWorkerPlugins.Find(plugin => plugin.GlobalWorkerName == globalWorkerName);

            if (foundGlobalWorker is null)
                return Messages.GlobalWorkerMissing(loadedGlobalWorkerPlugins.Select(plugin => plugin.GlobalWorkerName), globalWorkerName);
            
            config.AssignedGlobalWorkerPlugins.Add(foundGlobalWorker);
        }
        
        // Return null if no errors was found.
        return null;
    }
}