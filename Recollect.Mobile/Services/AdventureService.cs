using Recollect.Mobile.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;

namespace Recollect.Mobile.Services;

public class AdventureService
{
    private Adventure _currentAdventure = new();
    private readonly DataService? _dataService;
    private const string CurrentAdventureIdKey = "current_adventure_id";

    public event Action? CurrentAdventureChanged;

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
        Preferences.Set(CurrentAdventureIdKey, 0);
        CurrentAdventureChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task SetCurrentAdventureAsync(Adventure adventure)
    {
        if (adventure == null) return Task.CompletedTask;
        _currentAdventure = adventure;
        Preferences.Set(CurrentAdventureIdKey, _currentAdventure.Id);
        CurrentAdventureChanged?.Invoke();
        return Task.CompletedTask;
    }

    public async Task SaveCurrentAdventureAsync()
    {
        if (!string.IsNullOrWhiteSpace(_currentAdventure.Name) && _dataService != null)
        {
            await _dataService.SaveAdventureAsync(_currentAdventure);
            await LoadSavedAdventuresAsync();
            Preferences.Set(CurrentAdventureIdKey, _currentAdventure.Id);
        }
    }

    public async Task AddWaypointAsync(double latitude, double longitude, string? note = null, string? mediaUri = null)
    {
        // Ensure the current adventure is persisted before adding waypoints
        if (_currentAdventure.Id == 0 && _dataService != null && !string.IsNullOrWhiteSpace(_currentAdventure.Name))
        {
            await _dataService.SaveAdventureAsync(_currentAdventure);
            Preferences.Set(CurrentAdventureIdKey, _currentAdventure.Id);
        }

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
            CurrentAdventureChanged?.Invoke();
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
            Preferences.Set(CurrentAdventureIdKey, 0);
            CurrentAdventureChanged?.Invoke();
        }
    }

    public void ClearCurrentAdventure()
    {
        _currentAdventure = new Adventure();
        Preferences.Set(CurrentAdventureIdKey, 0);
        CurrentAdventureChanged?.Invoke();
    }

    public bool HasCurrentAdventure => !string.IsNullOrWhiteSpace(_currentAdventure.Name);

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
            var savedId = Preferences.Get(CurrentAdventureIdKey, 0);
            if (savedId > 0)
            {
                var match = SavedAdventures.FirstOrDefault(a => a.Id == savedId);
                if (match != null)
                {
                    _currentAdventure = match;
                    CurrentAdventureChanged?.Invoke();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load adventures: {ex.Message}");
            SavedAdventures = new List<Adventure>();
        }
    }

    public async Task RestoreCurrentAdventureAsync()
    {
        var savedId = Preferences.Get(CurrentAdventureIdKey, 0);
        if (savedId <= 0)
        {
            return;
        }
        if (SavedAdventures == null || SavedAdventures.Count == 0)
        {
            await LoadSavedAdventuresAsync();
        }
        var match = SavedAdventures.FirstOrDefault(a => a.Id == savedId);
        if (match != null)
        {
            _currentAdventure = match;
            CurrentAdventureChanged?.Invoke();
        }
    }
}
