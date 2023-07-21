using Microsoft.EntityFrameworkCore;

namespace Samples;

public class FooDbContext : DbContext
{
    public FooDbContext(DbContextOptions<FooDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FirstEntity>().HasKey(f => f.Id);
        modelBuilder.Entity<FirstEntity>().Property(f => f.Name);

        modelBuilder.Entity<SecondEntity>().HasKey(f => f.Id);
        modelBuilder.Entity<SecondEntity>().Property(f => f.Name);
    }
}