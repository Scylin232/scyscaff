using CommandLine;

namespace ScyScaff.Core.Models.CLI;

// CLI arguments (options) model.
internal class Options
{
    [Option('p', "path", Required = false, HelpText = "Specify working directory.")]
    public string? Path { get; set; }

    [Option('f', "file", Required = false, HelpText = "Specify config file.")]
    public string? File { get; set; }
}