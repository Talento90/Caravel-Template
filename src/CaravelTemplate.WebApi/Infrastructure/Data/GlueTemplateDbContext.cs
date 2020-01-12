﻿using CaravelTemplate.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.WebApi.Infrastructure.Data
{
    public class CaravelTemplateDbContext : DbContext
    {
        public DbSet<Device> Devices { get; set; } = null!;

        public CaravelTemplateDbContext(DbContextOptions<CaravelTemplateDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaravelTemplateDbContext).Assembly);
        }
    }
}