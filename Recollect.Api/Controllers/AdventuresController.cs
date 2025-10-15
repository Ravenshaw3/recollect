using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recollect.Api.Models;

namespace Recollect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdventuresController : ControllerBase
{
    private readonly AdventureDbContext _context;

    public AdventuresController(AdventureDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromBody] AdventureDto adventure)
    {
        try
        {
            // Set timestamps
            adventure.CreatedAt = DateTime.UtcNow;
            adventure.UpdatedAt = DateTime.UtcNow;

            // Save adventure to database
            _context.Adventures.Add(adventure);
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                Success = true, 
                Id = adventure.Id,
                Message = "Adventure uploaded successfully" 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                Success = false, 
                Error = ex.Message 
            });
        }
    }

    [HttpGet("{id}")]
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
                return NotFound(new { 
                    Success = false, 
                    Error = "Adventure not found" 
                });
            }
            
            return Ok(adventure);
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                Success = false, 
                Error = ex.Message 
            });
        }
    }

    [HttpGet]
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
            return BadRequest(new { 
                Success = false, 
                Error = ex.Message 
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdventure(int id)
    {
        try
        {
            var adventure = await _context.Adventures.FindAsync(id);
            if (adventure == null)
            {
                return NotFound(new { 
                    Success = false, 
                    Error = "Adventure not found" 
                });
            }

            _context.Adventures.Remove(adventure);
            await _context.SaveChangesAsync();
            
            return Ok(new { 
                Success = true, 
                Message = "Adventure deleted successfully" 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                Success = false, 
                Error = ex.Message 
            });
        }
    }
}