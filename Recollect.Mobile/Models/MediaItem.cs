namespace Recollect.Mobile.Models;

public class MediaItem
{
    public int Id { get; set; }
    public int AdventureId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public enum MediaType
{
    Photo,
    Video
}
