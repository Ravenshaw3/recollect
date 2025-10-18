using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Recollect.Mobile.Models;
using Recollect.Mobile.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Recollect.Mobile.Pages;

public partial class ReviewPage : ContentPage
{
    private ApiService? _apiService;
    private UploadQueueService? _uploadQueue;
    private AdventureService? _adventureService;

    public string AdventureName { get; set; } = "My Adventure";
    public string AdventureSummary { get; set; } = "Tracked route with photos and notes";
    public int WaypointCount { get; set; }
    public string Distance { get; set; } = "0.0";
    public string Duration { get; set; } = "0:00";

    public ReviewPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_apiService == null || _adventureService == null || _uploadQueue == null)
        {
            var sp = Handler?.MauiContext?.Services;
            _apiService ??= sp?.GetService<ApiService>();
            _adventureService ??= sp?.GetService<AdventureService>();
            _uploadQueue ??= sp?.GetService<UploadQueueService>();
        }
        if (_adventureService != null)
        {
            _adventureService.CurrentAdventureChanged -= OnCurrentAdventureChanged;
            _adventureService.CurrentAdventureChanged += OnCurrentAdventureChanged;
        }
        LoadAdventureData();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_adventureService != null)
        {
            _adventureService.CurrentAdventureChanged -= OnCurrentAdventureChanged;
        }
    }

    private void OnCurrentAdventureChanged()
    {
        MainThread.BeginInvokeOnMainThread(LoadAdventureData);
    }

    private void LoadAdventureData()
    {
        // In a real app, this would load from local storage or database
        if (_adventureService?.CurrentAdventure != null)
        {
            WaypointCount = _adventureService.CurrentAdventure.Waypoints.Count;
            AdventureName = _adventureService.CurrentAdventure.Name;
            
            // Calculate distance (simplified)
            if (_adventureService.CurrentAdventure.Waypoints.Count > 1)
            {
                double totalDistance = 0;
                for (int i = 1; i < _adventureService.CurrentAdventure.Waypoints.Count; i++)
                {
                    var prev = _adventureService.CurrentAdventure.Waypoints[i - 1];
                    var curr = _adventureService.CurrentAdventure.Waypoints[i];
                    totalDistance += CalculateDistance(prev.Latitude, prev.Longitude, curr.Latitude, curr.Longitude);
                }
                Distance = totalDistance.ToString("F2");
            }
        }
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Simplified distance calculation (Haversine formula would be more accurate)
        const double R = 6371; // Earth's radius in kilometers
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Share", "Share functionality will be implemented in a future update.", "OK");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Save", "Adventure saved locally!", "OK");
    }

    private async void OnUploadClicked(object sender, EventArgs e)
    {
        try
        {
            if (_uploadQueue != null && _adventureService?.CurrentAdventure != null)
            {
                var adventure = _adventureService.CurrentAdventure;
                var toast = Toast.Make($"Uploading '{adventure.Name}'...", ToastDuration.Short);
                await toast.Show();
                _uploadQueue.EnqueueAdventureUpload(adventure);

                void Handler(string status)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        var t = Toast.Make(status, ToastDuration.Short);
                        await t.Show();
                    });
                }

                _uploadQueue.StatusChanged -= Handler;
                _uploadQueue.StatusChanged += Handler;
            }
            else
            {
                await DisplayAlert("Error", "Services not available. Please restart the app.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Upload failed: {ex.Message}", "OK");
        }
    }
}
