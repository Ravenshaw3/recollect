using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recollect.Api.Models;

namespace Recollect.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AdventureDbContext _context;

    public AdminController(AdventureDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var totalAdventures = await _context.Adventures.CountAsync();
            var totalWaypoints = await _context.Waypoints.CountAsync();
            var totalNotes = await _context.Notes.CountAsync();
            var totalMediaItems = await _context.MediaItems.CountAsync();
            
            var recentAdventures = await _context.Adventures
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new { a.Id, a.Name, a.CreatedAt })
                .ToListAsync();

            return Ok(new
            {
                TotalAdventures = totalAdventures,
                TotalWaypoints = totalWaypoints,
                TotalNotes = totalNotes,
                TotalMediaItems = totalMediaItems,
                RecentAdventures = recentAdventures
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("adventures")]
    public async Task<IActionResult> GetAllAdventures()
    {
        try
        {
            var adventures = await _context.Adventures
                .Include(a => a.Waypoints)
                .Include(a => a.Notes)
                .Include(a => a.MediaItems)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(adventures);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("adventures/{id}")]
    public async Task<IActionResult> GetAdventure(int id)
    {
        try
        {
            var adventure = await _context.Adventures
                .Include(a => a.Waypoints)
                .Include(a => a.Notes)
                .Include(a => a.MediaItems)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (adventure == null)
            {
                return NotFound(new { Success = false, Error = "Adventure not found" });
            }

            return Ok(adventure);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpPut("adventures/{id}")]
    public async Task<IActionResult> UpdateAdventure(int id, [FromBody] AdventureDto adventure)
    {
        try
        {
            var existingAdventure = await _context.Adventures.FindAsync(id);
            if (existingAdventure == null)
            {
                return NotFound(new { Success = false, Error = "Adventure not found" });
            }

            existingAdventure.Name = adventure.Name;
            existingAdventure.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Message = "Adventure updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpDelete("adventures/{id}")]
    public async Task<IActionResult> DeleteAdventure(int id)
    {
        try
        {
            var adventure = await _context.Adventures.FindAsync(id);
            if (adventure == null)
            {
                return NotFound(new { Success = false, Error = "Adventure not found" });
            }

            _context.Adventures.Remove(adventure);
            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Message = "Adventure deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("waypoints")]
    public async Task<IActionResult> GetAllWaypoints()
    {
        try
        {
            var waypoints = await _context.Waypoints
                .OrderByDescending(w => w.Timestamp)
                .ToListAsync();

            return Ok(waypoints);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("notes")]
    public async Task<IActionResult> GetAllNotes()
    {
        try
        {
            var notes = await _context.Notes
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();

            return Ok(notes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("media")]
    public async Task<IActionResult> GetAllMediaItems()
    {
        try
        {
            var mediaItems = await _context.MediaItems
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            return Ok(mediaItems);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpDelete("waypoints/{id}")]
    public async Task<IActionResult> DeleteWaypoint(int id)
    {
        try
        {
            var waypoint = await _context.Waypoints.FindAsync(id);
            if (waypoint == null)
            {
                return NotFound(new { Success = false, Error = "Waypoint not found" });
            }

            _context.Waypoints.Remove(waypoint);
            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Message = "Waypoint deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpDelete("notes/{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        try
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound(new { Success = false, Error = "Note not found" });
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Message = "Note deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpDelete("media/{id}")]
    public async Task<IActionResult> DeleteMediaItem(int id)
    {
        try
        {
            var mediaItem = await _context.MediaItems.FindAsync(id);
            if (mediaItem == null)
            {
                return NotFound(new { Success = false, Error = "Media item not found" });
            }

            _context.MediaItems.Remove(mediaItem);
            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Message = "Media item deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }
}
