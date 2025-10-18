using Recollect.Mobile.Services;

namespace Recollect.Mobile.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadCurrentConfiguration();
    }

    private void LoadCurrentConfiguration()
    {
        var currentConfig = Preferences.Get("api_configuration", "local");
        var availableConfigs = ConfigurationService.GetAvailableConfigurations();
        
        var index = availableConfigs.IndexOf(currentConfig);
        if (index >= 0)
        {
            ApiConfigurationPicker.SelectedIndex = index;
        }
        
        UpdateCurrentApiUrl();
    }

    private void UpdateCurrentApiUrl()
    {
        var currentUrl = ConfigurationService.GetApiBaseUrl();
        CurrentApiUrlLabel.Text = $"Current API URL: {currentUrl}";
    }

    private void OnApiConfigurationChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (picker.SelectedIndex >= 0)
        {
            var selectedConfig = picker.Items[picker.SelectedIndex];
            ConfigurationService.SetApiConfiguration(selectedConfig);
            UpdateCurrentApiUrl();
        }
    }

    private async void OnTestConnectionClicked(object sender, EventArgs e)
    {
        try
        {
            ConnectionStatusLabel.Text = "Testing connection...";
            ConnectionStatusLabel.TextColor = Colors.Orange;
            
            // Test the connection
            var apiService = Handler?.MauiContext?.Services?.GetService<ApiService>();
            if (apiService != null)
            {
                // Try to make a simple API call
                var isConnected = await TestApiConnection(apiService);
                
                if (isConnected)
                {
                    ConnectionStatusLabel.Text = "✅ Connection successful!";
                    ConnectionStatusLabel.TextColor = Colors.Green;
                }
                else
                {
                    ConnectionStatusLabel.Text = "❌ Connection failed";
                    ConnectionStatusLabel.TextColor = Colors.Red;
                }
            }
            else
            {
                ConnectionStatusLabel.Text = "❌ API service not available";
                ConnectionStatusLabel.TextColor = Colors.Red;
            }
        }
        catch (Exception ex)
        {
            ConnectionStatusLabel.Text = $"❌ Error: {ex.Message}";
            ConnectionStatusLabel.TextColor = Colors.Red;
        }
    }

    private async Task<bool> TestApiConnection(ApiService apiService)
    {
        try
        {
            // Try to get adventures as a connection test
            var adventures = await apiService.GetAllAdventuresAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void OnUseCustomUrlClicked(object sender, EventArgs e)
    {
        var customUrl = CustomApiUrlEntry.Text?.Trim();
        if (!string.IsNullOrEmpty(customUrl))
        {
            try
            {
                // Validate URL
                var uri = new Uri(customUrl);
                Preferences.Set("api_configuration", "custom");
                Preferences.Set("custom_api_url", customUrl);
                UpdateCurrentApiUrl();
                
                DisplayAlert("Success", $"Custom URL set to: {customUrl}", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Invalid URL: {ex.Message}", "OK");
            }
        }
        else
        {
            DisplayAlert("Error", "Please enter a valid URL", "OK");
        }
    }
}
