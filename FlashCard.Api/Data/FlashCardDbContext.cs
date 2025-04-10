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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
        });

        modelBuilder.Entity<Flashcard>(entity =>
        {
            entity.Property(e => e.Front).HasMaxLength(200);
            entity.Property(e => e.Back).HasMaxLength(500);
            entity.Property(e => e.Source).HasMaxLength(20);

            entity.HasOne(f => f.User)
                .WithMany(u => u.Flashcards)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.Generation)
                .WithMany(g => g.Flashcards)
                .HasForeignKey(f => f.GenerationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Generation>(entity =>
        {
            entity.Property(e => e.Model).HasMaxLength(255);
            entity.Property(e => e.SourceTextHash).HasMaxLength(255);

            entity.HasOne(g => g.User)
                .WithMany(u => u.Generations)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GenerationErrorLog>(entity =>
        {
            entity.Property(e => e.Model).HasMaxLength(255);
            entity.Property(e => e.ErrorCode).HasMaxLength(50);
            entity.Property(e => e.ErrorMessage).HasMaxLength(500);
            entity.Property(e => e.SourceTextHash).HasMaxLength(255);

            entity.HasOne(e => e.User)
                .WithMany(u => u.GenerationErrorLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
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