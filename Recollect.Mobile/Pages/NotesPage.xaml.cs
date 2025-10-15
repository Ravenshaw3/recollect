using Microsoft.Maui.Controls;
using Recollect.Mobile.Models;
using System.Collections.ObjectModel;

namespace Recollect.Mobile.Pages;

public partial class NotesPage : ContentPage
{
    public ObservableCollection<Note> Notes { get; set; } = new();

    public NotesPage()
    {
        InitializeComponent();
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
                    Timestamp = DateTime.Now
                };
                
                Notes.Add(note);
            }
        }
    }

    private async void OnVoiceNoteClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Voice Note", "Voice recording feature will be implemented in a future update.", "OK");
    }
}
