using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using System.Collections.ObjectModel;
using Recollect.Mobile.Models;

namespace Recollect.Mobile.Services;

public class LocationService
{
    private EventHandler<GeolocationLocationChangedEventArgs>? _locationHandler;

    public async Task StartTrackingAsync(ObservableCollection<Waypoint> waypoints)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        if (status != PermissionStatus.Granted) return;

        _locationHandler = (s, e) =>
        {
            waypoints.Add(new Waypoint { Latitude = e.Location.Latitude, Longitude = e.Location.Longitude });
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
        await Task.CompletedTask;
    }
}