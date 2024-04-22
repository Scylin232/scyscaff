using ScyScaff.Core.Models.Application;

namespace ScyScaff.Core.Utils.Application;

// Represents a class for handling application exit
public class ApplicationExit : IApplicationExit
{
    // Exits the application with the exit code -1
    public void ExitErrorCodeMinusOne() => Environment.Exit(-1);
}