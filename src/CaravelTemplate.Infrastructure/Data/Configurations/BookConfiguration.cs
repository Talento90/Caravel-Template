using CaravelTemplate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaravelTemplate.Infrastructure.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("books", CaravelTemplateDbContext.DefaultSchema);

            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Id).IsRequired();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Name).HasMaxLength(100);
        }
    }
}