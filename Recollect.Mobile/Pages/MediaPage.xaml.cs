using Microsoft.Maui.Controls;
using Recollect.Mobile.Models;
using Recollect.Mobile.Services;
using System.Collections.ObjectModel;

namespace Recollect.Mobile.Pages;

public partial class MediaPage : ContentPage
{
    private MediaService? _mediaService;
    private AdventureService? _adventureService;
    private ApiService? _apiService;

    public ObservableCollection<MediaItem> MediaItems { get; set; } = new();

    public MediaPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_mediaService == null || _adventureService == null || _apiService == null)
        {
            var sp = Handler?.MauiContext?.Services;
            _mediaService ??= sp?.GetService<MediaService>();
            _adventureService ??= sp?.GetService<AdventureService>();
            _apiService ??= sp?.GetService<ApiService>();
        }
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            if (_mediaService == null)
            {
                await DisplayAlert("Error", "Media service not available", "OK");
                return;
            }
            var photo = await _mediaService.CapturePhotoAsync();
            if (photo != null)
            {
                var mediaItem = new MediaItem
                {
                    FilePath = photo.FullPath,
                    ThumbnailPath = photo.FullPath, // In a real app, you'd generate thumbnails
                    Caption = "Photo captured",
                    Type = MediaType.Photo,
                    Timestamp = DateTime.Now
                };
                
                MediaItems.Add(mediaItem);
                var lastWaypoint = _adventureService?.CurrentAdventure?.Waypoints?.LastOrDefault();
                if (lastWaypoint != null)
                    lastWaypoint.MediaUri = photo.FullPath;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to capture photo: {ex.Message}", "OK");
        }
    }

    private async void OnRecordVideoClicked(object sender, EventArgs e)
    {
        try
        {
            if (_mediaService == null)
            {
                await DisplayAlert("Error", "Media service not available", "OK");
                return;
            }
            var video = await _mediaService.CaptureVideoAsync();
            if (video != null)
            {
                var mediaItem = new MediaItem
                {
                    FilePath = video.FullPath,
                    ThumbnailPath = video.FullPath, // In a real app, you'd generate thumbnails
                    Caption = "Video recorded",
                    Type = MediaType.Video,
                    Timestamp = DateTime.Now
                };
                
                MediaItems.Add(mediaItem);
                var lastWaypoint = _adventureService?.CurrentAdventure?.Waypoints?.LastOrDefault();
                if (lastWaypoint != null)
                    lastWaypoint.MediaUri = video.FullPath;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to record video: {ex.Message}", "OK");
        }
    }

    private async void OnSelectFromGalleryClicked(object sender, EventArgs e)
    {
        try
        {
            if (_mediaService == null)
            {
                await DisplayAlert("Error", "Media service not available", "OK");
                return;
            }
            var media = await _mediaService.PickFromGalleryAsync();
            if (media != null)
            {
                var mediaItem = new MediaItem
                {
                    FilePath = media.FullPath,
                    ThumbnailPath = media.FullPath,
                    Caption = "From gallery",
                    Type = media.ContentType?.StartsWith("video") == true ? MediaType.Video : MediaType.Photo,
                    Timestamp = DateTime.Now
                };
                
                MediaItems.Add(mediaItem);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to select from gallery: {ex.Message}", "OK");
        }
    }

    private async void OnUploadAudioClicked(object sender, EventArgs e)
    {
        try
        {
            var audioTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "audio/*", "audio/mpeg", "audio/mp4", "audio/3gpp", "audio/ogg", "audio/wav" } },
                { DevicePlatform.iOS, new[] { "public.audio" } },
                { DevicePlatform.WinUI, new[] { ".mp3", ".m4a", ".aac", ".3gp", ".ogg", ".wav" } },
                { DevicePlatform.MacCatalyst, new[] { "public.audio" } }
            });
            var pickOptions = new PickOptions
            {
                PickerTitle = "Select audio file",
                FileTypes = audioTypes
            };
            var fileResult = await FilePicker.PickAsync(pickOptions);
            if (fileResult == null)
                return;

            await using var stream = await fileResult.OpenReadAsync();
            var adventureId = _adventureService?.CurrentAdventure?.Id ?? 0;
            if (adventureId <= 0)
            {
                await DisplayAlert("No Adventure", "Please start or select an adventure before uploading.", "OK");
                return;
            }

            double? lat = null, lng = null;
            var last = _adventureService?.CurrentAdventure?.Waypoints?.LastOrDefault();
            if (last != null)
            {
                lat = last.Latitude; lng = last.Longitude;
            }

            if (_apiService == null)
            {
                await DisplayAlert("Error", "API service not available", "OK");
                return;
            }
            var ok = await _apiService.UploadAudioAsync(adventureId, stream, fileResult.FileName, lat, lng);
            if (ok)
            {
                await DisplayAlert("Uploaded", "Audio uploaded as voice note.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to upload audio.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Upload failed: {ex.Message}", "OK");
        }
    }
}
