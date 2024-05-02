using CommandLine;

namespace ScyScaff.CLI.Models.CLI;

// CLI arguments (options) model.
public class Options
{
    [Option('p', "path", Required = false, HelpText = "Specify working directory.")]
    public string? Path { get; set; }

    [Option('f', "file", Required = false, HelpText = "Specify config file.")]
    public string? File { get; set; }
    
    [Option('a', "add", Required = false, HelpText = "Allows scaffolder to add new lines to exiting files.")]
    public bool? Add { get; set; }
}