using System.IO.Abstractions;
using ScyScaff.Core.Models.Parser;

namespace ScyScaff.Core.Models.Events;

public interface IGenerationEvents
{
    Task OnGenerationStarted(IDirectoryInfo entityDirectory, IScaffolderEntity? entity);
    Task OnGenerationEnded(IDirectoryInfo entityDirectory, IScaffolderEntity? entity);
}