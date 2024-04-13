namespace ScyScaff.Core.Models.Builder;

// This class is needed for TemplateTreeGenerator.GenerateServiceFile function

// For example, when we iterate through all the models in the Dashboard plugin, we don't have access to that model's service,
// But we may need the name of the service where this model was taken from
public class ModelContext
{
    public required KeyValuePair<string, Dictionary<string, string>> Model;
    public required string BelongedEntityName;
}