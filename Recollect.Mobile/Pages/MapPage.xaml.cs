using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Recollect.Mobile.Models;
using Recollect.Mobile.Services;
using System.Collections.Specialized;
using System.Linq;

namespace Recollect.Mobile.Pages;

public partial class MapPage : ContentPage
{
    private LocationService? _locationService;
    private AdventureService? _adventureService;
    private Polyline _routeLine;

    public MapPage()
    {
        try
        {
            InitializeComponent();
            // Initialize route line safely
            _routeLine = new Polyline 
            { 
                StrokeColor = Colors.Blue, 
                StrokeWidth = 5 
            };
            
            // Add route line to map safely
            if (AdventureMap?.MapElements != null)
            {
                AdventureMap.MapElements.Add(_routeLine);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MapPage initialization failed: {ex.Message}");
            // Ensure we have a valid state even if initialization fails
            _routeLine = new Polyline { StrokeColor = Colors.Blue, StrokeWidth = 5 };
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            var sp = Handler?.MauiContext?.Services;
            _locationService ??= sp?.GetService<LocationService>();
            _adventureService ??= sp?.GetService<AdventureService>();
            if (_adventureService != null)
            {
                _adventureService.CurrentAdventureChanged -= OnCurrentAdventureChanged;
                _adventureService.CurrentAdventureChanged += OnCurrentAdventureChanged;
            }

            if (BindingContext == null)
            {
                BindingContext = _adventureService?.CurrentAdventure;
            }

            if (_adventureService?.CurrentAdventure?.Waypoints != null)
            {
                _adventureService.CurrentAdventure.Waypoints.CollectionChanged -= OnWaypointsChanged;
                _adventureService.CurrentAdventure.Waypoints.CollectionChanged += OnWaypointsChanged;
            }
        }
        catch { }
    }

    private void OnCurrentAdventureChanged()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            BindingContext = _adventureService?.CurrentAdventure;
            if (_adventureService?.CurrentAdventure?.Waypoints != null)
            {
                _adventureService.CurrentAdventure.Waypoints.CollectionChanged -= OnWaypointsChanged;
                _adventureService.CurrentAdventure.Waypoints.CollectionChanged += OnWaypointsChanged;
            }
        });
    }

    private void OnWaypointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        try
        {
            if (_adventureService?.CurrentAdventure?.Waypoints == null || _adventureService.CurrentAdventure.Waypoints.Count < 2) 
                return;

            var locations = _adventureService.CurrentAdventure.Waypoints
                .Where(w => w != null && w.Latitude != 0 && w.Longitude != 0)
                .Select(w => new Microsoft.Maui.Devices.Sensors.Location(w.Latitude, w.Longitude))
                .ToList();
            
            if (locations.Any() && _routeLine?.Geopath != null)
            {
                // Clear existing geopath and add new locations
                _routeLine.Geopath.Clear();
                foreach (var location in locations)
                {
                    _routeLine.Geopath.Add(location);
                }

                if (AdventureMap != null)
                {
                    var lastLocation = locations.Last();
                    AdventureMap.MoveToRegion(Microsoft.Maui.Maps.MapSpan.FromCenterAndRadius(lastLocation, Microsoft.Maui.Maps.Distance.FromKilometers(1)));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnWaypointsChanged error: {ex.Message}");
        }
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        // Check if we need to start a new adventure
        if (_adventureService == null || !_adventureService.HasCurrentAdventure)
        {
            var adventureName = await DisplayPromptAsync("New Adventure", "Enter a name for your adventure:", "Start", "Cancel");
            if (!string.IsNullOrWhiteSpace(adventureName))
            {
                if (_adventureService != null)
                {
                    await _adventureService.StartNewAdventureAsync(adventureName);
                    // Save the adventure to get an ID
                    await _adventureService.SaveCurrentAdventureAsync();
                }
            }
            else
            {
                return; // User cancelled
            }
        }
        
        if (_locationService != null && _adventureService != null)
        {
            var waypoints = _adventureService.CurrentAdventure?.Waypoints;
            if (waypoints != null)
            {
                var api = Handler?.MauiContext?.Services?.GetService<ApiService>();
                var advId = _adventureService.CurrentAdventure?.Id ?? 0;
                await _locationService.StartTrackingAsync(waypoints, api, advId);
            }
        }
    }

    private async void OnStopClicked(object sender, EventArgs e)
    {
        if (_locationService != null)
        {
            await _locationService.StopTrackingAsync();
        }
    }

    private async void OnPauseClicked(object sender, EventArgs e)
    {
        if (_locationService != null)
        {
            await _locationService.PauseTrackingAsync();
        }
    }

    private async void OnResumeClicked(object sender, EventArgs e)
    {
        if (_locationService != null && _adventureService?.CurrentAdventure?.Waypoints != null)
        {
            await _locationService.ResumeTrackingAsync();
        }
    }

    private async void OnPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                var waypoint = _adventureService?.CurrentAdventure.Waypoints.LastOrDefault();
                if (waypoint != null) waypoint.MediaUri = photo.FullPath;
            }
        }
        catch (Exception ex)
        {
            // Handle camera permission or other errors
            await DisplayAlert("Error", $"Failed to capture photo: {ex.Message}", "OK");
        }
    }
}