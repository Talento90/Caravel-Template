using Asp.Versioning;
using Caravel.MediatR;
using CaravelTemplate.Adapter.Api.Extensions;
using CaravelTemplate.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Serilog;

namespace CaravelTemplate.Adapter.Api;

public class ApiServer
{
    private readonly WebApplication _application;

    public ApiServer(string[] args, Action<IHostApplicationBuilder> hostBuilder, Action<WebApplication> webApplicationOptions)
    {
        var builder = WebApplication.CreateBuilder(args);
        hostBuilder.Invoke(builder);

        builder.Services
            .AddFeatureManagement(builder.Configuration.GetSection("FeatureManagement"))
            .AddFeatureFilter<PercentageFilter>();

        builder.AddOpenTelemetry();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddOpenApi();
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
        
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationRequestBehavior<,>));
        });

        builder.Services.AddEndpoints(typeof(ApiServer).Assembly);
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);
        
        _application = builder.Build();
        
        // API Versioning
        var apiVersionSet = _application.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var versionedGroup = _application
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);
        
        // Configure the HTTP request pipeline.
        if (_application.Environment.IsDevelopment())
        {
            _application.UseSwagger();
            _application.UseSwaggerUI();
        }
        
        //TODO: Add Middleware Enrichers
        _application.UseSerilogRequestLogging();
        _application.UseHttpsRedirection();
        
        // Map the application endpoints
        _application.MapEndpoints(versionedGroup);
        webApplicationOptions.Invoke(_application);
    }

    public async Task StartAsync()
    {
        await _application.RunAsync();
    }
}