using Microsoft.Maui.Controls;
using Recollect.Mobile.Models;
using Recollect.Mobile.Services;
using System.Collections.ObjectModel;

namespace Recollect.Mobile.Pages;

public partial class MediaPage : ContentPage
{
    private readonly MediaService _mediaService;
    private readonly AdventureService _adventureService;

    public ObservableCollection<MediaItem> MediaItems { get; set; } = new();

    public MediaPage(MediaService mediaService, AdventureService adventureService)
    {
        InitializeComponent();
        _mediaService = mediaService;
        _adventureService = adventureService;
        BindingContext = this;
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
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
                var lastWaypoint = _adventureService.CurrentAdventure.Waypoints.LastOrDefault();
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
                var lastWaypoint = _adventureService.CurrentAdventure.Waypoints.LastOrDefault();
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
}
