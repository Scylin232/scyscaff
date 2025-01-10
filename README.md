# Scylin's Scaffolder

**ScyScaff** is a modular tool designed to automate the process of creating multiple projects based on their defined entities, with built-in support for **Docker**.

# Example

Input configuration file:
```yaml
ProjectName: ScyCommerce

Auth: auth0

Dashboard:
  Name: svelte-crud

GlobalWorkers:
  - Name: grafana-prometheus
  - Name: elk

DefaultFramework: aspnet-ddd
DefaultDatabase: postgresql
DefaultServiceFlags:
  Logging: elk

Services:
  Products:
    Flags:
      Metrics: prometheus
    Models:
      Category:
        Title: string
        Description: string
      Supplier:
        Name: string
        Info: string
  Reviews:
    Models:
      Review:
        Content: string
```

Generated structure:
```
> ScyCommerce.Dashboard
    > Svelte-Kit based dashboard containing pages with CRUD tables for all models

> ScyCommerce.Global
    > ELK/Grafana-Prometheus configurations for the specified services

> ScyCommerce.Products
    > API
        > (Category CRUD Controller)
        > (Supplier CRUD Controller)
    > Domain
        > (Category Entity)
        > (Supplier Entity)
> ScyCommerce.Reviews
    > API
        > (Review CRUD Controller)
    > Domain
        > (Review Entity)
        
> docker-compose.dev.yml
    > Configurations of all services
```

*You are not limited to the technologies shown in the configuration file. Check the documentation to learn how to create your own **Plugin**.*

# Documentation | Usage

As the project is still a **Work in Progress (WIP)**, there is no centralized installation mechanism yet.  
To try out the tool, download the source code.

Detailed documentation can be found here: [https://scyscaff-documentation.netlify.app/](https://scyscaff-documentation.netlify.app/)

# Plugins

Standard plugin repositories (automatically loaded on the first run):
- [ASP.NET Plugin](https://github.com/Scylin232/scyscaff-aspnet-plugin)
- [Svelte Plugin](https://github.com/Scylin232/scyscaff-svelte-crud-plugin)
- [Grafana-Prometheus Plugin](https://github.com/Scylin232/scyscaff-grafana-prometheus-plugin)
- [ELK Stack Plugin](https://github.com/Scylin232/scyscaff-elk-stack-plugin)
