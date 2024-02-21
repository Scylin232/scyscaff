﻿using ScyScaff.Docker.Enums;

namespace ScyScaff.Docker.Models.Builder;

public class DockerComposeService
{
    public DockerComposeServiceType Type = DockerComposeServiceType.Unknown;
    
    public string? Image { get; set; }
    public ComposeBuild? Build { get; set; }
    
    public string? ContainerName { get; set; }
    public string? ExtraProperties { get; set; }
    
    public Dictionary<int, int?>? Ports { get; set; }
    public Dictionary<string, string>? Volumes { get; set; }
    public Dictionary<string, string>? EnvironmentVariables { get; set; }
    public Dictionary<string, ComposeDependency>? Dependencies { get; set; }
}

public class ComposeDependency
{
    public string? Condition { get; set; }
}

public class ComposeBuild
{
    public required string Context { get; set; }
    public required string Dockerfile { get; set; }
}