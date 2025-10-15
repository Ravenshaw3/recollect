using Microsoft.EntityFrameworkCore;
using Recollect.Api.Models;

namespace Recollect.Api;

public class AdventureDbContext : DbContext
{
    public DbSet<AdventureDto> Adventures { get; set; } = null!;
    public DbSet<WaypointDto> Waypoints { get; set; } = null!;
    public DbSet<NoteDto> Notes { get; set; } = null!;
    public DbSet<MediaItemDto> MediaItems { get; set; } = null!;

    public AdventureDbContext(DbContextOptions<AdventureDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Adventure entity
        modelBuilder.Entity<AdventureDto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // Configure Waypoint entity
        modelBuilder.Entity<WaypointDto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
        });

        // Configure Note entity
        modelBuilder.Entity<NoteDto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
        });

        // Configure MediaItem entity
        modelBuilder.Entity<MediaItemDto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Timestamp).IsRequired();
        });
    }
}