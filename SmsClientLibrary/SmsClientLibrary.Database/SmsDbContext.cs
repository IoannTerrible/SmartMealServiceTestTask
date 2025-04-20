using Microsoft.EntityFrameworkCore;
using SmsClientLibrary.Database.Entities;
using SmsClientLibrary.Database.EntityConfigurations;

namespace SmsClientLibrary.Database;

public class SmsDbContext : DbContext
{
    public DbSet<DishEntity> Dishes { get; set; } = null!;

    public SmsDbContext(DbContextOptions<SmsDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DishEntityConfiguration());
    }
}
