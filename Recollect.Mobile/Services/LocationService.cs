using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using System.Collections.ObjectModel;
using Recollect.Mobile.Models;

namespace Recollect.Mobile.Services;

public class LocationService
{
    private EventHandler<GeolocationLocationChangedEventArgs>? _locationHandler;
    private ObservableCollection<Waypoint>? _activeWaypoints;
    private bool _isPaused;
    private DateTime _lastAddedUtc = DateTime.MinValue;
    private Location? _lastLocation;

    // Throttling settings
    private static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(10);
    private const double MinDistanceMeters = 20; // ~20m

    public async Task StartTrackingAsync(ObservableCollection<Waypoint> waypoints)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        if (status != PermissionStatus.Granted) return;

        _activeWaypoints = waypoints;
        _isPaused = false;

        _locationHandler = (s, e) =>
        {
            if (_isPaused || _activeWaypoints == null) return;

            var nowUtc = DateTime.UtcNow;
            var current = e.Location;

            // Time throttle
            if (nowUtc - _lastAddedUtc < MinInterval)
            {
                return;
            }

            // Distance throttle
            if (_lastLocation != null)
            {
                var km = Location.CalculateDistance(_lastLocation, current, DistanceUnits.Kilometers);
                if (km * 1000 < MinDistanceMeters)
                {
                    return;
                }
            }

            _activeWaypoints.Add(new Waypoint
            {
                Latitude = current.Latitude,
                Longitude = current.Longitude,
                Timestamp = DateTime.Now
            });

            _lastAddedUtc = nowUtc;
            _lastLocation = current;
        };

        Geolocation.LocationChanged += _locationHandler;

        var request = new GeolocationListeningRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
        await Geolocation.StartListeningForegroundAsync(request);
    }

    public async Task StopTrackingAsync()
    {
        Geolocation.StopListeningForeground();
        if (_locationHandler != null)
        {
            Geolocation.LocationChanged -= _locationHandler;
            _locationHandler = null;
        }
        _activeWaypoints = null;
        _isPaused = false;
        _lastAddedUtc = DateTime.MinValue;
        _lastLocation = null;
        await Task.CompletedTask;
    }

    public Task PauseTrackingAsync()
    {
        _isPaused = true;
        return Task.CompletedTask;
    }

    public Task ResumeTrackingAsync()
    {
        _isPaused = false;
        return Task.CompletedTask;
    }
}