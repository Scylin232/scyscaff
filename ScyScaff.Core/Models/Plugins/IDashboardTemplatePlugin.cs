namespace ScyScaff.Core.Models.Plugins;

public interface IDashboardTemplatePlugin : ITemplatePlugin
{
    string DashboardName { get; }
}