using ScyScaff.Core.Models.Parsing;

namespace ScyScaff.Core.Models.Plugins;

public interface IFrameworkPlugin
{
    string FrameworkName { get; }
    
    string[] SupportedAuth { get; }
    string[] SupportedDatabases { get; }
    
    void GenerateFrameworkFiles(
        string directory, 
        string projectName,
        string serviceName,
        Microservice belongedMicroservice
    );
}