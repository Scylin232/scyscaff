using ScyScaff.Core.Models.Plugins;

namespace ScyScaff.Core.Models.Application;

public interface IPluginGatherer
{
    List<IFrameworkTemplatePlugin> GatherFrameworkPlugins();
    List<IDashboardTemplatePlugin> GatherDashboardPlugins();
    List<IGlobalWorkerPlugin> GatherGlobalWorkerPlugins();
}