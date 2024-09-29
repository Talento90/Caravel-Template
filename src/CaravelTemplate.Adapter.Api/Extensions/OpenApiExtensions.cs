namespace CaravelTemplate.Adapter.Api.Extensions;

public static class OpenApiExtensions
{
    public static void AddOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}