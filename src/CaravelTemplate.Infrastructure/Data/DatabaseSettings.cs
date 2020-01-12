namespace CaravelTemplate.Infrastructure.Data
{
    public class DatabaseSettings
    {
        public bool IsInMemory { get; set; }
        public string? ConnectionString { get; set; }
    }
}