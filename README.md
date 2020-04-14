# Caravel Template

This template uses Caravel package as an SDK and it bootstraps a full functional web api with the following structure:

* CaravelTemplate (Application Domain)
* CaravelTemplate.Core (Application business logic)
* CaravelTemplate.Infrastructure (External dependencies such as database)
* CaravelTemplate.WebApi (HTTP Web Api using ASP.NET 3.1)
* CaravelTemplate.WebApi.Tests (Integration tests)

PS: `CaravelTemplate` will be replaced for your project name when generating the project.

### Features

* [Caravel SDK](https://github.com/talento90/caravel) (Errors, Middleware, Exceptions)
* Business logic using CQRS pattern  ([MediatR](https://github.com/jbogard/MediatR))
* Entity Framework Core (InMemory and PostgresSQL)
* Health Check mechanism
* Swagger using [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle)
* Docker and Docker Compose
* Logging using [Serilog](https://serilog.net/)
* Testing using [Bogus](https://github.com/bchavez/Bogus) (Fake data generator) and [Fluent Assertions](https://fluentassertions.com/)


## Installation

#### Download and Install Template
```bash
git clone git@github.com:Talento90/caravel-template.git
dotnet new --install ~/caravel-template
```

#### Generate Project
```bash
dotnet new caravel-webapi -n MyProject -o ./
```
Note: `MyProject` is  going to replace the `CaravelTemplate`  

#### Run Web Api

`dotnet run --project src/MyProject.WebApi`

`open http://localhost:5000/swagger/index.html`


## Docker Compose

* Setup PostgresSQL database
* Setup WebApi

```bash
# Setup and run docker compose
docker-compose up

# Remove containers
docker-compose down
```

## Generate HTTPS Certificate

```bash
dotnet dev-certs https -ep "dev-localhost.pfx" -p password123 --trust
```

## Entity Framework Migrations

```bash
# Install dotnet-ef tool
dotnet tool install --global dotnet-ef

# Update dotnet-ef tool
dotnet tool update --global dotnet-ef

# Run Migration
dotnet ef migrations add {Migratio Name} --output-dir Data/Migrations --project src/CaravelTemplate.Infrastructure
```