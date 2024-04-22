using System.IO.Abstractions;
using ScyScaff.Core.Models.Parser;

namespace ScyScaff.Core.Models.Events;

public interface ITemplateGenerationEvents
{
    Task OnServiceGenerationStarted(IDirectoryInfo serviceDirectory, ScaffolderService? service);
    Task OnServiceGenerationEnded(IDirectoryInfo serviceDirectory, ScaffolderService? service);
}