using Microsoft.EntityFrameworkCore;
using Recollect.Mobile.Data;
using Recollect.Mobile.Models;

namespace Recollect.Mobile.Services;

public class DataService
{
    private readonly RecollectDbContext _context;

    public DataService(RecollectDbContext context)
    {
        _context = context;
    }

    // Adventure operations
    public async Task<Adventure> SaveAdventureAsync(Adventure adventure)
    {
        if (adventure.Id == 0)
        {
            _context.Adventures.Add(adventure);
        }
        else
        {
            adventure.UpdatedAt = DateTime.Now;
            _context.Adventures.Update(adventure);
        }

        await _context.SaveChangesAsync();
        return adventure;
    }

    public async Task<List<Adventure>> GetAllAdventuresAsync()
    {
        return await _context.Adventures
            .Include(a => a.Waypoints)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Adventure?> GetAdventureByIdAsync(int id)
    {
        return await _context.Adventures
            .Include(a => a.Waypoints)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAdventureAsync(int id)
    {
        var adventure = await _context.Adventures.FindAsync(id);
        if (adventure != null)
        {
            _context.Adventures.Remove(adventure);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Adventure> CreateAdventureAsync(string name)
    {
        var adv = new Adventure
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Unnamed Adventure" : name,
            CreatedAt = DateTime.Now
        };
        _context.Adventures.Add(adv);
        await _context.SaveChangesAsync();
        return adv;
    }

    // Waypoint operations
    public async Task<Waypoint> SaveWaypointAsync(Waypoint waypoint)
    {
        if (waypoint.Id == 0)
        {
            _context.Waypoints.Add(waypoint);
        }
        else
        {
            _context.Waypoints.Update(waypoint);
        }

        await _context.SaveChangesAsync();
        return waypoint;
    }

    public async Task<List<Waypoint>> GetWaypointsByAdventureIdAsync(int adventureId)
    {
        return await _context.Waypoints
            .Where(w => w.AdventureId == adventureId)
            .OrderBy(w => w.Timestamp)
            .ToListAsync();
    }

    // Note operations
    public async Task<Note> SaveNoteAsync(Note note)
    {
        if (note.Id == 0)
        {
            _context.Notes.Add(note);
        }
        else
        {
            _context.Notes.Update(note);
        }

        await _context.SaveChangesAsync();
        return note;
    }

    public async Task<List<Note>> GetNotesByAdventureIdAsync(int adventureId)
    {
        return await _context.Notes
            .Where(n => n.AdventureId == adventureId)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    // Media operations
    public async Task<MediaItem> SaveMediaItemAsync(MediaItem mediaItem)
    {
        if (mediaItem.Id == 0)
        {
            _context.MediaItems.Add(mediaItem);
        }
        else
        {
            _context.MediaItems.Update(mediaItem);
        }

        await _context.SaveChangesAsync();
        return mediaItem;
    }

    public async Task<List<MediaItem>> GetMediaItemsByAdventureIdAsync(int adventureId)
    {
        return await _context.MediaItems
            .Where(m => m.AdventureId == adventureId)
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync();
    }

    // Offload: remove local media files and clear pointers to free storage
    public async Task<int> OffloadAdventureMediaAsync(int adventureId)
    {
        var count = 0;
        var mediaItems = await GetMediaItemsByAdventureIdAsync(adventureId);
        foreach (var m in mediaItems)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(m.FilePath) && File.Exists(m.FilePath))
                {
                    File.Delete(m.FilePath);
                    count++;
                }
                if (!string.IsNullOrWhiteSpace(m.ThumbnailPath) && File.Exists(m.ThumbnailPath) && m.ThumbnailPath != m.FilePath)
                {
                    try { File.Delete(m.ThumbnailPath); } catch { /* ignore */ }
                }
                m.FilePath = string.Empty;
                m.ThumbnailPath = string.Empty;
                _context.MediaItems.Update(m);
            }
            catch
            {
                // ignore file IO failures; continue updating DB pointers
                m.FilePath = string.Empty;
                m.ThumbnailPath = string.Empty;
                _context.MediaItems.Update(m);
            }
        }

        // Clear waypoint media pointers
        var waypoints = await _context.Waypoints.Where(w => w.AdventureId == adventureId && !string.IsNullOrEmpty(w.MediaUri)).ToListAsync();
        foreach (var w in waypoints)
        {
            w.MediaUri = null;
            _context.Waypoints.Update(w);
        }

        await _context.SaveChangesAsync();
        return count;
    }

    // Database initialization
    public async Task InitializeDatabaseAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }
}
