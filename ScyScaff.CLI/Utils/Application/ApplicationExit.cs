using ScyScaff.Core.Models.Application;

namespace ScyScaff.Core.Utils.Application;

public class ApplicationExit : IApplicationExit
{
    public void ExitErrorCodeMinusOne() => Environment.Exit(-1);
}