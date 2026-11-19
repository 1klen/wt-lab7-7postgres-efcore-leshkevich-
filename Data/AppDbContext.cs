using Microsoft.EntityFrameworkCore;
using wt_lab6_minimalapi_leshkevich.Models;

namespace wt_lab6_minimalapi_leshkevich.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Car> Cars => Set<Car>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>().HasData(
            new Car
            {
                Id = 1,
                Brand = "Mercedes-Benz",
                Model = "E 320 W211",
                PricePerDay = 90,
                IsAvailable = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Car
            {
                Id = 2,
                Brand = "BMW",
                Model = "530i E60",
                PricePerDay = 85,
                IsAvailable = true,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}