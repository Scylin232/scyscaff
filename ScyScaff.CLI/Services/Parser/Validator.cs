using System.Text.RegularExpressions;
using ScyScaff.CLI.Utils.Constants;
using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.CLI.Services.Parser;

public static class Validator
{
    public static string? EnsureConfig(
        ScaffolderConfig config,
        List<IFrameworkTemplatePlugin> loadedFrameworkPlugins,
        List<IDashboardTemplatePlugin> loadedDashboardPlugins,
        List<IGlobalWorkerTemplatePlugin> loadedGlobalWorkerPlugins)
    {
        // Check if project name is not empty and contains latin letters only.
        if (config.ProjectName.Length <= 0 || !Regex.IsMatch(config.ProjectName, "^[a-zA-Z]+$"))
            return Messages.ProjectNameEmptyError;

        // Check if specified dashboard: exists, supports specified flags.
        if (config.Dashboard is not null)
        {
            IDashboardTemplatePlugin? foundDashboard = loadedDashboardPlugins.Find(plugin => plugin.Name == config.Dashboard.Name);
        
            if (foundDashboard is null)
                return Messages.DashboardMissing(loadedDashboardPlugins.Select(plugin => plugin.Name), config.Dashboard.Name);

            string? flagCheckError = CheckFlags(foundDashboard, config.Dashboard);

            if (flagCheckError is not null)
                return flagCheckError;
            
            config.Dashboard.DashboardTemplatePlugin = foundDashboard;
        }
        
        // Check if framework: exists, supports selected auth, supports selected database, supports specified flags.
        foreach (KeyValuePair<string, ScaffolderService> service in config.Services)
        {
            IFrameworkTemplatePlugin? foundFramework = loadedFrameworkPlugins.Find(plugin => plugin.Name == service.Value.Framework);

            if (foundFramework is null)
                return Messages.FrameworkMissing(loadedFrameworkPlugins.Select(plugin => plugin.Name), service.Value.Framework!);

            if (!foundFramework.SupportedAuth.Contains(config.Auth))
                return Messages.FrameworkAuthMissing(foundFramework.SupportedAuth, config.Auth, foundFramework.Name);

            if (!foundFramework.SupportedDatabases.Contains(service.Value.Database))
                return Messages.FrameworkDatabaseMissing(foundFramework.SupportedDatabases, service.Value.Database!, foundFramework.Name);

            string? flagCheckError = CheckFlags(foundFramework, service.Value);

            if (flagCheckError is not null)
                return flagCheckError;
    
            service.Value.AssignedFrameworkPlugin = foundFramework;
        }

        // Check if global worker: exists, supports specified flags.
        int globalWorkerIndex = 0;
        
        foreach (ScaffolderGlobalWorker globalWorker in config.GlobalWorkers)
        {
            IGlobalWorkerTemplatePlugin? foundGlobalWorker = loadedGlobalWorkerPlugins.Find(plugin => plugin.Name == globalWorker.Name);

            if (foundGlobalWorker is null)
                return Messages.GlobalWorkerMissing(loadedGlobalWorkerPlugins.Select(plugin => plugin.Name), globalWorker.Name);

            string? flagCheckError = CheckFlags(foundGlobalWorker, globalWorker);

            if (flagCheckError is not null)
                return flagCheckError;

            config.GlobalWorkers[globalWorkerIndex].GlobalWorkerTemplatePlugin = foundGlobalWorker;

            globalWorkerIndex++;
        }
        
        // Return null if no errors was found.
        return null;
    }

    private static string? CheckFlags(ITemplatePlugin plugin, IScaffolderEntity scaffolderEntity)
    {
        foreach (KeyValuePair<string, string> flag in scaffolderEntity.Flags)
        {
            if (!plugin.SupportedFlags.ContainsKey(flag.Key))
                return Messages.PluginFlagKeyNotSupported(plugin.SupportedFlags.Keys, flag.Key, plugin.Name);

            if (!plugin.SupportedFlags[flag.Key].Contains(flag.Value) && !plugin.SupportedFlags[flag.Key].Contains("*"))
                return Messages.PluginFlagValueNotSupported(plugin.SupportedFlags[flag.Key], flag.Key, flag.Value, plugin.Name);
        }

        return null;
    }
}