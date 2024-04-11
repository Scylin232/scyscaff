using ScyScaff.Core.Models.Application;

namespace ScyScaff.Core.Services.Builder;

public class ApplicationExit : IApplicationExit
{
    public void ExitErrorCodeMinusOne() => Environment.Exit(-1);
}