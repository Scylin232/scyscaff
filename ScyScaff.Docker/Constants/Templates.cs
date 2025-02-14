﻿namespace ScyScaff.Docker.Constants;

public static class Templates
{
  public const string DockerCompose = @"services:
  {{~ for service in services ~}}
  {{ service.container_name }}:{{if service.image != null}}
    image: {{ service.image }}{{end}}{{ if service.build != null }}
    build:
      context: {{ service.build.context }}
      dockerfile: {{ service.build.dockerfile }}{{end}}
    container_name: {{ service.container_name }}{{ if service.dependencies != null }}
    depends_on:{{ for dependency in service.dependencies }}
      {{ dependency.key }}:{{ if dependency.value.condition != null }}
        condition: {{dependency.value.condition}}{{ end }}{{ end }}{{ end }}{{ if service.volumes != null }}
    volumes:{{ for volume in service.volumes }}
      - {{ volume.key }}:{{ volume.value }}{{ end }}{{ end }}{{ if service.ports != null }}
    ports:{{ for port in service.ports }}
      {{ if port.value != null }}- ""{{ port.key }}:{{ port.value }}""{{ else }}- ""{{ port.key }}""{{end}}{{ end }}{{ end }}{{ if service.environment_variables != null }}
    environment:{{ for env in service.environment_variables }}
      {{ env.key }}: ""{{ env.value }}""{{ end }}{{ end }}
    networks:
      - {{ project_name | string.downcase }}-intranet
    {{ service.extra_properties }}
  {{~ end ~}}

volumes:
{{~ for registered_volume in registered_volumes ~}}
{{~ if (registered_volume | string.starts_with ""./"") == false  ~}}
  {{ registered_volume }}:
{{~ end ~}}
{{~ end ~}}

networks:
  {{ project_name | string.downcase }}-intranet:
    driver: bridge";
}