using System.Text.Json.Serialization;

namespace Recollect.Api.Models;

public class AdventureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<WaypointDto> Waypoints { get; set; } = new();
    public List<NoteDto> Notes { get; set; } = new();
    public List<MediaItemDto> MediaItems { get; set; } = new();
}

public class WaypointDto
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Note { get; set; }
    public string? MediaUri { get; set; }
    public DateTime Timestamp { get; set; }
}

public class NoteDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class MediaItemDto
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}