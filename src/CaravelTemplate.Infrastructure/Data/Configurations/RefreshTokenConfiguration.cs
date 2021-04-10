using CaravelTemplate.Entities;
using CaravelTemplate.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaravelTemplate.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Id).IsRequired();
            builder.Property(p => p.Token).IsRequired();
        }
    }
}