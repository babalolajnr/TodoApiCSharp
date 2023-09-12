using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Database;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Todo> Todos { get; set; } = null!;

    #region Required
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
    #endregion

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            if (entry.Entity is IEntityWithTimestamps entityWithTimestamps)
            {
                entityWithTimestamps.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChanges();
    }
}

public interface IEntityWithTimestamps
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime? DeletedAt { get; set; }
}