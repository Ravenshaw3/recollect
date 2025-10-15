using System.Collections.ObjectModel;

namespace Recollect.Mobile.Models;

public class Adventure
{
    public int Id { get; set; }
    public string Name { get; set; } = "Unnamed Recollect";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public ObservableCollection<Waypoint> Waypoints { get; set; } = new();
}

public class Waypoint
{
    public int Id { get; set; }
    public int AdventureId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Note { get; set; }
    public string? MediaUri { get; set; } // Photo/video path
    public DateTime Timestamp { get; set; } = DateTime.Now;
}