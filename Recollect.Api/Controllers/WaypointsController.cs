using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recollect.Api.Models;

namespace Recollect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaypointsController : ControllerBase
{
    private readonly AdventureDbContext _context;

    public WaypointsController(AdventureDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddWaypoint([FromQuery] int adventureId, [FromBody] WaypointDto waypoint)
    {
        var adv = await _context.Adventures
            .Include(a => a.Waypoints)
            .FirstOrDefaultAsync(a => a.Id == adventureId);
        if (adv == null)
        {
            return NotFound(new { Success = false, Error = "Adventure not found" });
        }

        waypoint.Timestamp = waypoint.Timestamp == default ? DateTime.UtcNow : waypoint.Timestamp;
        adv.Waypoints.Add(waypoint);
        adv.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Success = true });
    }
}


