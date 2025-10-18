using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recollect.Api.Models;

namespace Recollect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly AdventureDbContext _context;

    public NotesController(AdventureDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddNote([FromQuery] int adventureId, [FromBody] NoteDto note)
    {
        if (string.IsNullOrWhiteSpace(note.Title) && string.IsNullOrWhiteSpace(note.Content))
        {
            return BadRequest(new { Success = false, Error = "Invalid note" });
        }

        var adventure = await _context.Adventures
            .Include(a => a.Notes)
            .FirstOrDefaultAsync(a => a.Id == adventureId);
        if (adventure == null)
        {
            return NotFound(new { Success = false, Error = "Adventure not found" });
        }

        note.Timestamp = note.Timestamp == default ? DateTime.UtcNow : note.Timestamp;
        adventure.Notes.Add(note);
        adventure.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Success = true });
    }
}


