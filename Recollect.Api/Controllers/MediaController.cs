using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recollect.Api.Models;

namespace Recollect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly AdventureDbContext _context;
    private readonly IWebHostEnvironment _env;

    public MediaController(AdventureDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("upload-audio")]
    [RequestSizeLimit(50_000_000)] // ~50MB
    public async Task<IActionResult> UploadAudio([FromQuery] int adventureId, IFormFile file, [FromQuery] double? lat, [FromQuery] double? lng)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Success = false, Error = "No file provided" });
        }

        var adventure = await _context.Adventures
            .Include(a => a.MediaItems)
            .FirstOrDefaultAsync(a => a.Id == adventureId);
        if (adventure == null)
        {
            return NotFound(new { Success = false, Error = "Adventure not found" });
        }

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "audio");
        Directory.CreateDirectory(uploadsRoot);

        var safeFileName = Path.GetFileName(file.FileName);
        var uniqueName = $"{DateTime.UtcNow:yyyyMMdd_HHmmssfff}_{Guid.NewGuid():N}_{safeFileName}";
        var savedPath = Path.Combine(uploadsRoot, uniqueName);

        await using (var stream = System.IO.File.Create(savedPath))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/audio/{uniqueName}".Replace("\\", "/");

        var media = new MediaItemDto
        {
            FilePath = relativePath,
            ThumbnailPath = string.Empty,
            Caption = "Voice note",
            Type = "audio",
            Timestamp = DateTime.UtcNow,
            Latitude = lat,
            Longitude = lng
        };

        adventure.MediaItems.Add(media);
        adventure.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Success = true, Path = relativePath });
    }
}


