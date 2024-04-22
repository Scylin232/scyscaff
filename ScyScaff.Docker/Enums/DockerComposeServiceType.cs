namespace ScyScaff.Docker.Enums;

// Service type enumerator for Docker Services, required for filtering in necessary scenarios,
// for example, in the Grafana-Prometheus plugin.
public enum DockerComposeServiceType
{
    Unknown,
    Database,
    Dashboard,
    Framework,
    GlobalWorker,        
}