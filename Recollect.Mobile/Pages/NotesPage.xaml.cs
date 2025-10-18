using Microsoft.Maui.Controls;
using Recollect.Mobile.Models;
using Recollect.Mobile.Services;
using System.Collections.ObjectModel;

namespace Recollect.Mobile.Pages;

public partial class NotesPage : ContentPage
{
    private readonly AdventureService? _adventureService;
    public ObservableCollection<Note> Notes { get; set; } = new();

    public NotesPage(AdventureService? adventureService = null)
    {
        InitializeComponent();
        _adventureService = adventureService;
        BindingContext = this;
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
            if (Handler?.MauiContext?.Services?.GetService<ApiService>() is ApiService api &&
                Handler.MauiContext.Services.GetService<AdventureService>() is AdventureService adv)
            {
                var adventureId = adv.CurrentAdventure?.Id ?? 0;
                if (adventureId <= 0)
                {
                    await DisplayAlert("No Adventure", "Start an adventure first.", "OK");
                    return;
                }
                var ok = await api.UploadAudioAsync(adventureId, stream, pick.FileName);
                await DisplayAlert(ok ? "Uploaded" : "Error", ok ? "Voice note uploaded" : "Upload failed", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
