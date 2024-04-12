using ScyScaff.Core.Models.Application;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Core.Services.Plugins;

namespace ScyScaff.Core.Services.Builder;

public class PluginGatherer : IPluginGatherer
{
    public List<IFrameworkTemplatePlugin> GatherFrameworkPlugins()
    {
        string[] frameworkPluginPaths =
        {
            "D:\\dev\\CSharp\\ScyScaffPlugin.AspNet\\bin\\Debug\\net8.0\\ScyScaffPlugin.AspNet.dll"
        };
        
        List<IFrameworkTemplatePlugin> loadedFrameworkPlugins = PluginLoader<IFrameworkTemplatePlugin>.ConstructPlugins(frameworkPluginPaths);

        return loadedFrameworkPlugins;
    }

    public List<IDashboardTemplatePlugin> GatherDashboardPlugins()
    {
        string[] dashboardPluginPaths =
        {
            "D:\\dev\\CSharp\\ScyScaffPlugin.NextCRUDDashboard\\ScyScaffPlugin.NextCRUDDashboard\\bin\\Debug\\net8.0\\ScyScaffPlugin.NextCRUDDashboard.dll",
            "D:\\dev\\CSharp\\ScyScaffPlugin.SvelteCrud\\ScyScaffPlugin.SvelteCrud\\bin\\Debug\\net8.0\\ScyScaffPlugin.SvelteCrud.dll"
        };
        
        List<IDashboardTemplatePlugin> loadedDashboardPlugins = PluginLoader<IDashboardTemplatePlugin>.ConstructPlugins(dashboardPluginPaths);
        
        return loadedDashboardPlugins;
    }

    public List<IGlobalWorkerPlugin> GatherGlobalWorkerPlugins()
    {
        string[] globalWorkerPluginPaths =
        {
            "D:\\dev\\CSharp\\ScyScaffPlugin.GrafanaPrometheusGlobalWorker\\bin\\Debug\\net8.0\\ScyScaffPlugin.GrafanaPrometheusGlobalWorker.dll",
            "D:\\dev\\CSharp\\ScyScaffPlugin.ELK\\bin\\Debug\\net8.0\\ScyScaffPlugin.ELK.dll"
        };
        
        List<IGlobalWorkerPlugin> loadedGlobalWorkerPlugins = PluginLoader<IGlobalWorkerPlugin>.ConstructPlugins(globalWorkerPluginPaths);

        return loadedGlobalWorkerPlugins;
    }
}