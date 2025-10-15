using Microsoft.EntityFrameworkCore;
using Recollect.Mobile.Models;

namespace Recollect.Mobile.Data;

public class RecollectDbContext : DbContext
{
    public DbSet<Adventure> Adventures { get; set; }
    public DbSet<Waypoint> Waypoints { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<MediaItem> MediaItems { get; set; }

    public RecollectDbContext(DbContextOptions<RecollectDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Adventure entity
        modelBuilder.Entity<Adventure>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // Configure Waypoint entity
        modelBuilder.Entity<Waypoint>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.MediaUri).HasMaxLength(500);
            entity.Property(e => e.Timestamp).IsRequired();
            
            // Foreign key relationship
            entity.HasOne<Adventure>()
                  .WithMany(a => a.Waypoints)
                  .HasForeignKey(w => w.AdventureId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Note entity
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Timestamp).IsRequired();
            
            // Foreign key relationship
            entity.HasOne<Adventure>()
                  .WithMany()
                  .HasForeignKey(n => n.AdventureId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure MediaItem entity
        modelBuilder.Entity<MediaItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ThumbnailPath).HasMaxLength(500);
            entity.Property(e => e.Caption).HasMaxLength(500);
            entity.Property(e => e.Timestamp).IsRequired();
            
            // Foreign key relationship
            entity.HasOne<Adventure>()
                  .WithMany()
                  .HasForeignKey(m => m.AdventureId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
