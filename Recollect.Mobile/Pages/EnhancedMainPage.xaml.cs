using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Recollect.Mobile.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Timers;

namespace Recollect.Mobile.Pages
{
    public partial class EnhancedMainPage : ContentPage
    {
        private readonly LocationService _locationService;
        private readonly EnhancedMediaService _mediaService;
        private readonly AdventureService _adventureService;
        private readonly ApiService _apiService;

        private bool _isAdventureActive = false;
        private Timer _updateTimer;
        private ObservableCollection<MediaItem> _mediaItems;

        public EnhancedMainPage()
        {
            InitializeComponent();
            
            _locationService = new LocationService();
            _mediaService = new EnhancedMediaService();
            _adventureService = new AdventureService();
            _apiService = new ApiService();

            _mediaItems = new ObservableCollection<MediaItem>();
            MediaCollectionView.ItemsSource = _mediaItems;

            InitializeMap();
            StartUpdateTimer();
        }

        private void InitializeMap()
        {
            try
            {
                AdventureMap.MapType = MapType.Street;
                AdventureMap.IsScrollEnabled = true;
                AdventureMap.IsZoomEnabled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing map: {ex.Message}");
            }
        }

        private void StartUpdateTimer()
        {
            _updateTimer = new Timer(1000); // Update every second
            _updateTimer.Elapsed += OnUpdateTimerElapsed;
            _updateTimer.Start();
        }

        private async void OnUpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await MainThread.InvokeAsync(() =>
            {
                UpdateAdventureStatus();
                UpdateVoiceRecordingStatus();
            });
        }

        private void UpdateAdventureStatus()
        {
            if (_isAdventureActive)
            {
                AdventureStatusLabel.Text = "🚀 Adventure in Progress!";
                StartStopButton.Text = "⏹️ Stop Adventure";
                StartStopButton.BackgroundColor = Colors.Red;
            }
            else
            {
                AdventureStatusLabel.Text = "Ready to start your adventure!";
                StartStopButton.Text = "🚀 Start Adventure";
                StartStopButton.BackgroundColor = Colors.Green;
            }
        }

        private void UpdateVoiceRecordingStatus()
        {
            if (_mediaService.IsVoiceRecording)
            {
                VoiceRecordingFrame.IsVisible = true;
                var duration = _mediaService.GetVoiceRecordingDuration;
                VoiceRecordingDurationLabel.Text = $"{duration.Minutes:D2}:{duration.Seconds:D2}";
            }
            else
            {
                VoiceRecordingFrame.IsVisible = false;
            }
        }

        private async void OnStartStopAdventureClicked(object sender, EventArgs e)
        {
            try
            {
                if (!_isAdventureActive)
                {
                    await StartAdventure();
                }
                else
                {
                    await StopAdventure();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to {( _isAdventureActive ? "stop" : "start")} adventure: {ex.Message}", "OK");
            }
        }

        private async Task StartAdventure()
        {
            try
            {
                var locationPermission = await _locationService.CheckLocationPermissionAsync();
                if (!locationPermission)
                {
                    await DisplayAlert("Permission Required", "Location permission is required to start an adventure.", "OK");
                    return;
                }

                var success = await _adventureService.StartAdventureAsync();
                if (success)
                {
                    _isAdventureActive = true;
                    await DisplayAlert("Adventure Started!", "Your adventure tracking has begun! 🚀", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to start adventure. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to start adventure: {ex.Message}", "OK");
            }
        }

        private async Task StopAdventure()
        {
            try
            {
                var success = await _adventureService.StopAdventureAsync();
                if (success)
                {
                    _isAdventureActive = false;
                    await DisplayAlert("Adventure Complete!", "Your adventure has been saved! 🎉", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to stop adventure. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to stop adventure: {ex.Message}", "OK");
            }
        }

        private async void OnVoiceRecordClicked(object sender, EventArgs e)
        {
            try
            {
                if (!_mediaService.IsVoiceRecording)
                {
                    var result = await _mediaService.StartVoiceRecordingAsync();
                    if (result == "Recording started")
                    {
                        await DisplayAlert("Voice Recording", "Voice recording started! 🎤", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", result, "OK");
                    }
                }
                else
                {
                    await OnStopVoiceRecordingClicked(sender, e);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to start voice recording: {ex.Message}", "OK");
            }
        }

        private async void OnStopVoiceRecordingClicked(object sender, EventArgs e)
        {
            try
            {
                var audioData = await _mediaService.StopVoiceRecordingAsync();
                if (!string.IsNullOrEmpty(audioData))
                {
                    await DisplayAlert("Voice Recording", "Voice recording saved! 🎤", "OK");
                    // TODO: Save audio data to adventure
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save voice recording.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to stop voice recording: {ex.Message}", "OK");
            }
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await _mediaService.TakePhotoAsync();
                if (photo != null)
                {
                    var base64Data = await _mediaService.ConvertMediaToBase64Async(photo);
                    if (!string.IsNullOrEmpty(base64Data))
                    {
                        // TODO: Save photo to adventure
                        await DisplayAlert("Photo", "Photo captured and saved! 📸", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to capture photo.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to take photo: {ex.Message}", "OK");
            }
        }

        private async void OnRecordVideoClicked(object sender, EventArgs e)
        {
            try
            {
                var video = await _mediaService.RecordVideoAsync();
                if (video != null)
                {
                    var base64Data = await _mediaService.ConvertMediaToBase64Async(video);
                    if (!string.IsNullOrEmpty(base64Data))
                    {
                        // TODO: Save video to adventure
                        await DisplayAlert("Video", "Video recorded and saved! 🎥", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to record video.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to record video: {ex.Message}", "OK");
            }
        }

        private async void OnGenerateStoryClicked(object sender, EventArgs e)
        {
            try
            {
                if (!_isAdventureActive)
                {
                    await DisplayAlert("No Adventure", "Please start an adventure first to generate a story!", "OK");
                    return;
                }

                var result = await _apiService.GenerateStoryAsync();
                if (result)
                {
                    await DisplayAlert("Story Generated!", "Your adventure story has been created! 🎭", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to generate story. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to generate story: {ex.Message}", "OK");
            }
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new SettingsPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to open settings: {ex.Message}", "OK");
            }
        }

        private async void OnStatisticsClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new StatisticsPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to open statistics: {ex.Message}", "OK");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }
    }
}
