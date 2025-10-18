using Microsoft.Maui.Controls;
using Recollect.Mobile.Models;
using Recollect.Mobile.Services;
using System.Collections.ObjectModel;

namespace Recollect.Mobile.Pages;

public partial class NotesPage : ContentPage
{
    private AdventureService? _adventureService;
    public ObservableCollection<Note> Notes { get; set; } = new();

    public NotesPage(AdventureService? adventureService = null)
    {
        InitializeComponent();
        _adventureService = adventureService;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _adventureService ??= Handler?.MauiContext?.Services?.GetService<AdventureService>();
        if (_adventureService != null)
        {
            _adventureService.CurrentAdventureChanged -= OnCurrentAdventureChanged;
            _adventureService.CurrentAdventureChanged += OnCurrentAdventureChanged;
        }
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
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Update UI state if needed; bindings already point to page collection
        });
    }

    private async void OnAddNoteClicked(object sender, EventArgs e)
    {
        var title = await DisplayPromptAsync("New Note", "Enter note title:", "OK", "Cancel");
        if (!string.IsNullOrWhiteSpace(title))
        {
            var content = await DisplayPromptAsync("Note Content", "Enter your note:", "OK", "Cancel");
            if (!string.IsNullOrWhiteSpace(content))
            {
                var note = new Note
                {
                    Title = title,
                    Content = content,
                    Timestamp = DateTime.Now,
                    AdventureId = _adventureService?.CurrentAdventure?.Id ?? 0
                };
                
                Notes.Add(note);
                
                // Save to adventure if available
                if (_adventureService?.CurrentAdventure != null)
                {
                    await _adventureService.SaveCurrentAdventureAsync();
                    // Incremental upload
                    if (Handler?.MauiContext?.Services?.GetService<ApiService>() is ApiService api)
                    {
                        await api.AddNoteAsync(_adventureService.CurrentAdventure.Id, note);
                    }
                }
            }
        }
    }

    private async void OnVoiceNoteClicked(object sender, EventArgs e)
    {
        try
        {
            var audioTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "audio/*" } },
                { DevicePlatform.iOS, new[] { "public.audio" } },
                { DevicePlatform.WinUI, new[] { ".mp3", ".m4a", ".wav", ".aac" } },
                { DevicePlatform.MacCatalyst, new[] { "public.audio" } }
            });
            var pick = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select voice note",
                FileTypes = audioTypes
            });
            if (pick == null) return;

            await using var stream = await pick.OpenReadAsync();
            if (Handler?.MauiContext?.Services?.GetService<AdventureService>() is AdventureService adv)
            {
                var adventureId = adv.CurrentAdventure?.Id ?? 0;
                if (adventureId <= 0)
                {
                    await DisplayAlert("No Adventure", "Start an adventure first.", "OK");
                    return;
                }
                // Save locally as media item; upload from Review per user selection
                if (Handler?.MauiContext?.Services?.GetService<DataService>() is DataService data)
                {
                    var item = new MediaItem
                    {
                        AdventureId = adventureId,
                        FilePath = pick.FullPath,
                        ThumbnailPath = pick.FullPath,
                        Caption = "Voice note",
                        Type = MediaType.Audio,
                        Timestamp = DateTime.Now
                    };
                    await data.SaveMediaItemAsync(item);
                    await DisplayAlert("Saved", "Voice note added. Upload from Review.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
