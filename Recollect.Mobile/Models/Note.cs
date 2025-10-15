namespace Recollect.Mobile.Models;

public class Note
{
    public int Id { get; set; }
    public int AdventureId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
