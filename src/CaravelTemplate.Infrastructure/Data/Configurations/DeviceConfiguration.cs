using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaravelTemplate.Infrastructure.Data.Configurations
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            //builder.Property(p => p.Id).IsRequired();
            //builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        }
    }
}