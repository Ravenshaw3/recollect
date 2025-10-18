using Recollect.Mobile.Models;
using Recollect.Mobile.Services;
using System.Collections.ObjectModel;

namespace Recollect.Mobile.Pages;

public partial class AdventuresPage : ContentPage
{
    public ObservableCollection<Adventure> Adventures { get; } = new();
    private AdventureService? _adventureService;
    private DataService? _dataService;

    public AdventuresPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var sp = Handler?.MauiContext?.Services;
        _adventureService ??= sp?.GetService<AdventureService>();
        _dataService ??= sp?.GetService<DataService>();
        await LoadAdventuresAsync();
    }

    private async Task LoadAdventuresAsync()
    {
        Adventures.Clear();
        if (_adventureService == null) return;
        await _adventureService.LoadSavedAdventuresAsync();
        foreach (var a in _adventureService.SavedAdventures)
        {
            Adventures.Add(a);
        }
        StatusLabel.Text = $"{Adventures.Count} adventures";
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("New Adventure", "Enter a name:");
        if (string.IsNullOrWhiteSpace(name)) return;
        if (_dataService == null || _adventureService == null) return;
        var adv = await _dataService.CreateAdventureAsync(name.Trim());
        await _adventureService.SetCurrentAdventureAsync(adv);
        await LoadAdventuresAsync();
        await DisplayAlert("Created", $"Adventure '{adv.Name}' created.", "OK");
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // no-op; using explicit buttons
    }

    private async void OnSelectClicked(object sender, EventArgs e)
    {
        if (_adventureService == null) return;
        if ((sender as Button)?.CommandParameter is Adventure adv)
        {
            await _adventureService.SetCurrentAdventureAsync(adv);
            await DisplayAlert("Selected", $"Current adventure set to '{adv.Name}'.", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_adventureService == null) return;
        if ((sender as Button)?.CommandParameter is Adventure adv)
        {
            var ok = await DisplayAlert("Delete", $"Delete '{adv.Name}'?", "Yes", "No");
            if (!ok) return;
            await _adventureService.DeleteAdventureAsync(adv.Id);
            await LoadAdventuresAsync();
        }
    }

    private async void OnSearchChanged(object sender, TextChangedEventArgs e)
    {
        await LoadAdventuresAsync();
        var term = (e.NewTextValue ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(term)) return;
        for (int i = Adventures.Count - 1; i >= 0; i--)
        {
            if (!Adventures[i].Name.ToLowerInvariant().Contains(term))
            {
                Adventures.RemoveAt(i);
            }
        }
        StatusLabel.Text = $"{Adventures.Count} match(es)";
    }

    private async void OnRenameClicked(object sender, EventArgs e)
    {
        if (_adventureService == null) return;
        if ((sender as Button)?.CommandParameter is Adventure adv)
        {
            var newName = await DisplayPromptAsync("Rename Adventure", "Enter new name:", initialValue: adv.Name);
            if (string.IsNullOrWhiteSpace(newName)) return;
            await _adventureService.SetCurrentAdventureAsync(adv);
            await _adventureService.UpdateAdventureNameAsync(newName.Trim());
            await LoadAdventuresAsync();
        }
    }
}


