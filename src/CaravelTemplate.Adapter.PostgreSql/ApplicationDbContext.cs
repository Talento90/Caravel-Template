using CaravelTemplate.Books;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Adapter.PostgreSql;

public class ApplicationDbContext : DbContext
{
    public const string Schema = "book_shop";
    
    public DbSet<Book> Books { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schema);
    }
}