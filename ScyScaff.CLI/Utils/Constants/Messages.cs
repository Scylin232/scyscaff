namespace ScyScaff.CLI.Utils.Constants;

public static class Messages
{
    public const string ProjectNameEmptyError = "Project name can't be empty and can contain latin letters only.";
    public const string AddModeNotEnabledError = "You executed the scaffolder in a directory where there are already files with the same name. Add the \"--add true\" option to continue. This will only add new lines and will not remove any code you've written, but you will likely need to rework lines you've already modified. The Docker-Compose file will be overwritten completely.";
    
    public static string GlobalWorkerMissing(IEnumerable<string> globalWorkersList, string globalWorkerName) => $"Global worker {globalWorkerName} was not found. Available global workers: {string.Join(", ", globalWorkersList)}";
    
    public static string DashboardMissing(IEnumerable<string> dashboardsList, string dashboardName) => $"Dashboard {dashboardName} was not found. Available dashboards: {string.Join(", ", dashboardsList)}";
    
    public static string FrameworkMissing(IEnumerable<string> frameworksList, string frameworkName) => $"Framework '{frameworkName}' was not found. Available frameworks: {string.Join(", ", frameworksList)}";
    public static string FrameworkAuthMissing(IEnumerable<string> authList, string authName, string frameworkName) => $"Framework '{frameworkName}' does not support '{authName}' auth. Supported auth: {string.Join(", ", authList)}";
    public static string FrameworkDatabaseMissing(IEnumerable<string> databasesList, string databaseName, string frameworkName) => $"Framework '{frameworkName}' does not support '{databaseName}' database. Supported databases: {string.Join(", ", databasesList)}";
   
    public static string PluginFlagKeyNotSupported(IEnumerable<string> flagKeysList, string flagKey, string pluginName) => $"Flag '{flagKey}' is not supported by '{pluginName}'. Supported flags: {string.Join(", ", flagKeysList)}";
    public static string PluginFlagValueNotSupported(IEnumerable<string> flagValuesList, string flagKey, string flagValue, string pluginName) => $"Flag '{flagKey}' value of '{flagValue}' is not supported by '{pluginName}'. Supported flag values: {string.Join(", ", flagValuesList)}";

    public static string FileNotFoundInWorkingDirectory(string specifiedFile, string workingDirectory) => $"Could not find file {specifiedFile} at {workingDirectory}";

    public static string PluginImplementationNotFound(string type, string assembly, string assemblyLocation, string availableTypes) => $"Can't find any type which implements {type} in {assembly} from {assemblyLocation}.\nAvailable types: {availableTypes}";
}