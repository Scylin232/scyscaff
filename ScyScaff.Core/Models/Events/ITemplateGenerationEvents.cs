using System.IO.Abstractions;

namespace ScyScaff.Core.Models.Events;

public interface ITemplateGenerationEvents
{
    Task OnServiceGenerationStarted(IDirectoryInfo serviceDirectory);
    Task OnServiceGenerationEnded(IDirectoryInfo serviceDirectory);
}