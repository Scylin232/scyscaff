namespace ScyScaff.Core.Models.Plugins;

public interface ITemplateGenerationEvents
{
    Task OnServiceGenerationStarted(DirectoryInfo serviceDirectory);
    Task OnServiceGenerationEnded(DirectoryInfo serviceDirectory);
}