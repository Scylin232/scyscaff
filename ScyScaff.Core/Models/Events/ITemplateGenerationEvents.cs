namespace ScyScaff.Core.Models.Events;

public interface ITemplateGenerationEvents
{
    Task OnServiceGenerationStarted(DirectoryInfo serviceDirectory);
    Task OnServiceGenerationEnded(DirectoryInfo serviceDirectory);
}