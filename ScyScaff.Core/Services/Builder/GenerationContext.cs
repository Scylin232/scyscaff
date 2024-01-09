using ScyScaff.Core.Models.Parser;

namespace ScyScaff.Core.Services.Builder;

internal class GenerationContext(ScaffolderConfig config, ScaffolderService service, int templateTreePathLength, string serviceDirectory, KeyValuePair<object, object>? model)
{
    public ScaffolderConfig Config { get; } = config;
    public ScaffolderService Service { get; } = service;
    public int TemplateTreePathLength { get; } = templateTreePathLength;
    public string ServiceDirectory { get; } = serviceDirectory;
}