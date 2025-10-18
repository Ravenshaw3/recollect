using Recollect.Mobile.Models;
using System.Collections.ObjectModel;

namespace Recollect.Mobile.Services;

public class AdventureService
{
    private Adventure _currentAdventure = new();
    private readonly DataService? _dataService;

    public Adventure CurrentAdventure => _currentAdventure;
    public IReadOnlyList<Adventure> SavedAdventures { get; private set; } = new List<Adventure>();

    public AdventureService(DataService? dataService = null)
    {
        try
        {
            _dataService = dataService;
            // Don't load adventures during construction to avoid startup issues
            SavedAdventures = new List<Adventure>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AdventureService constructor error: {ex.Message}");
            SavedAdventures = new List<Adventure>();
        }
    }

    public Task StartNewAdventureAsync(string name)
    {
        _currentAdventure = new Adventure
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Unnamed Adventure" : name,
            Waypoints = new ObservableCollection<Waypoint>()
        };
        return Task.CompletedTask;
    }

    public Task SetCurrentAdventureAsync(Adventure adventure)
    {
        if (adventure == null) return Task.CompletedTask;
        _currentAdventure = adventure;
        return Task.CompletedTask;
    }

    public async Task SaveCurrentAdventureAsync()
    {
        if (!string.IsNullOrWhiteSpace(_currentAdventure.Name) && _currentAdventure.Waypoints.Count > 0 && _dataService != null)
        {
            await _dataService.SaveAdventureAsync(_currentAdventure);
            await LoadSavedAdventuresAsync();
        }
    }

    public async Task AddWaypointAsync(double latitude, double longitude, string? note = null, string? mediaUri = null)
    {
        var waypoint = new Waypoint
        {
            AdventureId = _currentAdventure.Id,
            Latitude = latitude,
            Longitude = longitude,
            Note = note,
            MediaUri = mediaUri
        };
        
        _currentAdventure.Waypoints.Add(waypoint);
        
        // Save to database if adventure is persisted
        if (_currentAdventure.Id > 0 && _dataService != null)
        {
            await _dataService.SaveWaypointAsync(waypoint);
        }
    }

    public async Task UpdateAdventureNameAsync(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            _currentAdventure.Name = name;
            if (_currentAdventure.Id > 0 && _dataService != null)
            {
                await _dataService.SaveAdventureAsync(_currentAdventure);
            }
        }
    }

    public async Task DeleteAdventureAsync(int adventureId)
    {
        if (_dataService == null) return;
        await _dataService.DeleteAdventureAsync(adventureId);
        await LoadSavedAdventuresAsync();
        if (_currentAdventure.Id == adventureId)
        {
            _currentAdventure = new Adventure();
        }
    }

    public void ClearCurrentAdventure()
    {
        _currentAdventure = new Adventure();
    }

    public bool HasCurrentAdventure => !string.IsNullOrWhiteSpace(_currentAdventure.Name) && _currentAdventure.Waypoints.Count > 0;

    public async Task LoadSavedAdventuresAsync()
    {
        try
        {
            if (_dataService != null)
            {
                SavedAdventures = await _dataService.GetAllAdventuresAsync();
            }
            else
            {
                SavedAdventures = new List<Adventure>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load adventures: {ex.Message}");
            SavedAdventures = new List<Adventure>();
        }
    }
}
