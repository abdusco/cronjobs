using System;
using Microsoft.EntityFrameworkCore;

namespace HangfireDemo
{
    public class Sale
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Total { get; set; }
    }

    public class DemoDbContext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }

        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Sale>().HasData(new[]
            {
                new Sale {Total = 10},
                new Sale {Total = 15},
                new Sale {Total = 20},
            });
        }
    }
}