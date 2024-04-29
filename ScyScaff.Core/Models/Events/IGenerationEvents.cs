using System.IO.Abstractions;
using ScyScaff.Core.Models.Parser;

namespace ScyScaff.Core.Models.Events;

public interface IGenerationEvents
{
    Task OnGenerationStarted(IDirectoryInfo serviceDirectory, IScaffolderEntity? entity);
    Task OnGenerationEnded(IDirectoryInfo serviceDirectory, IScaffolderEntity? entity);
}