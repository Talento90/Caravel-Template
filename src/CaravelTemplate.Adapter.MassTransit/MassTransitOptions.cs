namespace CaravelTemplate.Adapter.MassTransit;

public record MassTransitOptions(string Host, ushort Port, string VirtualHost, string Username, string Password)
{
    
}