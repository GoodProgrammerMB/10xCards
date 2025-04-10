using FlashCard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashCard.Api.Data;

public class FlashCardDbContext : DbContext
{
    public FlashCardDbContext(DbContextOptions<FlashCardDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<Generation> Generations { get; set; }
    public DbSet<GenerationErrorLog> GenerationErrorLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configure Flashcard
        modelBuilder.Entity<Flashcard>()
            .HasIndex(f => f.UserId);

        modelBuilder.Entity<Flashcard>()
            .HasIndex(f => f.Source);

        modelBuilder.Entity<Flashcard>()
            .Property(f => f.Source)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Configure Generation
        modelBuilder.Entity<Generation>()
            .HasIndex(g => g.UserId);

        // Configure GenerationErrorLog
        modelBuilder.Entity<GenerationErrorLog>()
            .HasIndex(g => g.UserId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is User || e.Entity is Flashcard || e.Entity is Generation);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((dynamic)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                ((dynamic)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                ((dynamic)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
} 