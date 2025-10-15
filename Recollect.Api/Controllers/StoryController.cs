using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recollect.Api.Models;
using System.Text.Json;

namespace Recollect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoryController : ControllerBase
{
    private readonly AdventureDbContext _context;
    private readonly ILogger<StoryController> _logger;

    public StoryController(AdventureDbContext context, ILogger<StoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{id}/story")]
    public async Task<IActionResult> GetAdventureStory(int id)
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

            var story = GenerateAnimatedStory(adventure);
            return Ok(story);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating story for adventure {AdventureId}", id);
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandomAdventureStory()
    {
        try
        {
            var adventures = await _context.Adventures
                .Include(a => a.Waypoints)
                .Include(a => a.Notes)
                .Include(a => a.MediaItems)
                .ToListAsync();

            if (!adventures.Any())
            {
                return NotFound(new { Success = false, Error = "No adventures found" });
            }

            var randomAdventure = adventures[Random.Shared.Next(adventures.Count)];
            var story = GenerateAnimatedStory(randomAdventure);
            return Ok(story);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating random adventure story");
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    private AdventureStory GenerateAnimatedStory(AdventureDto adventure)
    {
        var story = new AdventureStory
        {
            AdventureId = adventure.Id,
            AdventureName = adventure.Name,
            CreatedAt = adventure.CreatedAt,
            TotalDuration = CalculateDuration(adventure),
            TotalDistance = CalculateDistance(adventure.Waypoints),
            WaypointCount = adventure.Waypoints.Count,
            NoteCount = adventure.Notes.Count,
            MediaCount = adventure.MediaItems.Count
        };

        // Generate humorous story based on real data
        story.Title = GenerateStoryTitle(adventure);
        story.Introduction = GenerateIntroduction(adventure);
        story.Chapters = GenerateStoryChapters(adventure);
        story.Conclusion = GenerateConclusion(adventure);
        story.AnimationSequence = GenerateAnimationSequence(adventure);

        return story;
    }

    private string GenerateStoryTitle(AdventureDto adventure)
    {
        var titles = new[]
        {
            $"The Epic Quest of {adventure.Name}",
            $"{adventure.Name}: A Journey Through Time and Space",
            $"The Legendary Adventure of {adventure.Name}",
            $"{adventure.Name}: When GPS Meets Destiny",
            $"The Great {adventure.Name} Expedition",
            $"{adventure.Name}: A Tale of Waypoints and Wonder"
        };

        return titles[Random.Shared.Next(titles.Length)];
    }

    private string GenerateIntroduction(AdventureDto adventure)
    {
        var introductions = new[]
        {
            $"Once upon a time, in a land not so far away, an intrepid explorer embarked on an adventure called '{adventure.Name}'. " +
            $"Little did they know that their phone's GPS would become their most trusted companion in this epic journey.",

            $"In the digital age of {adventure.CreatedAt:yyyy}, a brave soul set out on a quest known as '{adventure.Name}'. " +
            $"Armed with nothing but a smartphone and an insatiable thirst for adventure, they would discover that the real treasure was the waypoints they made along the way.",

            $"The story begins with a simple tap on a mobile app, but '{adventure.Name}' would prove to be anything but simple. " +
            $"This is the tale of how one person's casual stroll became an epic adventure worthy of legend."
        };

        return introductions[Random.Shared.Next(introductions.Length)];
    }

    private List<StoryChapter> GenerateStoryChapters(AdventureDto adventure)
    {
        var chapters = new List<StoryChapter>();
        var waypoints = adventure.Waypoints.OrderBy(w => w.Timestamp).ToList();

        for (int i = 0; i < waypoints.Count; i++)
        {
            var waypoint = waypoints[i];
            var chapter = new StoryChapter
            {
                ChapterNumber = i + 1,
                Title = GenerateChapterTitle(i, waypoint),
                Content = GenerateChapterContent(waypoint, i, waypoints.Count),
                Timestamp = waypoint.Timestamp,
                Location = new Location { Latitude = waypoint.Latitude, Longitude = waypoint.Longitude },
                AnimationType = GetAnimationType(i, waypoints.Count),
                Duration = i < waypoints.Count - 1 ? 
                    (waypoints[i + 1].Timestamp - waypoint.Timestamp).TotalMinutes : 0
            };

            chapters.Add(chapter);
        }

        return chapters;
    }

    private string GenerateChapterTitle(int index, WaypointDto waypoint)
    {
        var titles = new[]
        {
            $"Chapter {index + 1}: The Great GPS Awakening",
            $"Chapter {index + 1}: When Coordinates Collide",
            $"Chapter {index + 1}: The Waypoint of No Return",
            $"Chapter {index + 1}: Latitude and Longitude of Destiny",
            $"Chapter {index + 1}: The Plot Thickens at {waypoint.Latitude:F4}°N",
            $"Chapter {index + 1}: A Pin Drop Heard 'Round the World"
        };

        return titles[Random.Shared.Next(titles.Length)];
    }

    private string GenerateChapterContent(WaypointDto waypoint, int index, int total)
    {
        var contentTemplates = new[]
        {
            $"Our hero found themselves at coordinates {waypoint.Latitude:F6}, {waypoint.Longitude:F6}. " +
            $"The GPS signal was strong, but the adventure was stronger. " +
            $"{(string.IsNullOrEmpty(waypoint.Note) ? "No notes were left behind, but the memory would live on forever." : $"They left a note: '{waypoint.Note}'")}",

            $"At precisely {waypoint.Timestamp:HH:mm}, the adventure took a turn. " +
            $"The waypoint at {waypoint.Latitude:F4}°N, {waypoint.Longitude:F4}°W marked a pivotal moment in this epic tale. " +
            $"{(string.IsNullOrEmpty(waypoint.Note) ? "Sometimes, the most profound moments are the ones we don't document." : $"The note reads: '{waypoint.Note}' - profound words indeed.")}",

            $"The journey continued to waypoint #{index + 1} of {total}. " +
            $"Located at {waypoint.Latitude:F6}, {waypoint.Longitude:F6}, this spot would become legendary. " +
            $"{(string.IsNullOrEmpty(waypoint.Note) ? "Legend has it that silence speaks louder than words." : $"The inscription: '{waypoint.Note}' - a message for future adventurers.")}"
        };

        return contentTemplates[Random.Shared.Next(contentTemplates.Length)];
    }

    private string GenerateConclusion(AdventureDto adventure)
    {
        var conclusions = new[]
        {
            $"And so ends the tale of '{adventure.Name}' - a story that will be told for generations. " +
            $"From the first waypoint to the last, this adventure proved that sometimes the journey is more important than the destination. " +
            $"The GPS coordinates may fade, but the memories will last forever.",

            $"The adventure '{adventure.Name}' came to a close, but the legend was just beginning. " +
            $"Every waypoint was a story, every note a chapter, and every photo a memory. " +
            $"This is how legends are born - one GPS coordinate at a time.",

            $"As the final waypoint was reached, our hero realized that '{adventure.Name}' was more than just an adventure - " +
            $"it was a journey of self-discovery. The GPS guided the way, but the heart led the adventure. " +
            $"And so, another epic tale was added to the annals of digital exploration."
        };

        return conclusions[Random.Shared.Next(conclusions.Length)];
    }

    private List<AnimationFrame> GenerateAnimationSequence(AdventureDto adventure)
    {
        var frames = new List<AnimationFrame>();
        var waypoints = adventure.Waypoints.OrderBy(w => w.Timestamp).ToList();

        for (int i = 0; i < waypoints.Count; i++)
        {
            var waypoint = waypoints[i];
            frames.Add(new AnimationFrame
            {
                FrameNumber = i + 1,
                Timestamp = waypoint.Timestamp,
                Location = new Location { Latitude = waypoint.Latitude, Longitude = waypoint.Longitude },
                AnimationType = GetAnimationType(i, waypoints.Count),
                Duration = 2.0, // 2 seconds per frame
                Text = GenerateFrameText(waypoint, i, waypoints.Count),
                SoundEffect = GetSoundEffect(i, waypoints.Count)
            });
        }

        return frames;
    }

    private string GenerateFrameText(WaypointDto waypoint, int index, int total)
    {
        var texts = new[]
        {
            $"📍 Waypoint {index + 1} of {total}",
            $"🗺️ The adventure continues...",
            $"⚡ GPS signal acquired!",
            $"🎯 Target locked: {waypoint.Latitude:F4}°N",
            $"🌟 Another milestone reached!",
            $"📱 Location saved to history"
        };

        return texts[Random.Shared.Next(texts.Length)];
    }

    private string GetAnimationType(int index, int total)
    {
        var types = new[] { "fadeIn", "slideIn", "bounce", "zoom", "rotate", "pulse" };
        return types[index % types.Length];
    }

    private string GetSoundEffect(int index, int total)
    {
        var sounds = new[] { "ding", "pop", "chime", "beep", "whoosh", "click" };
        return sounds[index % sounds.Length];
    }

    private TimeSpan CalculateDuration(AdventureDto adventure)
    {
        if (adventure.Waypoints.Count < 2) return TimeSpan.Zero;
        
        var first = adventure.Waypoints.Min(w => w.Timestamp);
        var last = adventure.Waypoints.Max(w => w.Timestamp);
        return last - first;
    }

    private double CalculateDistance(List<WaypointDto> waypoints)
    {
        if (waypoints.Count < 2) return 0;

        double totalDistance = 0;
        for (int i = 1; i < waypoints.Count; i++)
        {
            totalDistance += CalculateDistanceBetweenPoints(
                waypoints[i - 1].Latitude, waypoints[i - 1].Longitude,
                waypoints[i].Latitude, waypoints[i].Longitude
            );
        }

        return totalDistance;
    }

    private double CalculateDistanceBetweenPoints(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}

public class AdventureStory
{
    public int AdventureId { get; set; }
    public string AdventureName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public List<StoryChapter> Chapters { get; set; } = new();
    public string Conclusion { get; set; } = string.Empty;
    public List<AnimationFrame> AnimationSequence { get; set; } = new();
    public TimeSpan TotalDuration { get; set; }
    public double TotalDistance { get; set; }
    public int WaypointCount { get; set; }
    public int NoteCount { get; set; }
    public int MediaCount { get; set; }
}

public class StoryChapter
{
    public int ChapterNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Location Location { get; set; } = new();
    public string AnimationType { get; set; } = string.Empty;
    public double Duration { get; set; }
}

public class AnimationFrame
{
    public int FrameNumber { get; set; }
    public DateTime Timestamp { get; set; }
    public Location Location { get; set; } = new();
    public string AnimationType { get; set; } = string.Empty;
    public double Duration { get; set; }
    public string Text { get; set; } = string.Empty;
    public string SoundEffect { get; set; } = string.Empty;
}

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
