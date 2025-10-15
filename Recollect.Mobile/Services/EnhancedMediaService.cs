using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Essentials;

namespace Recollect.Mobile.Services
{
    public class EnhancedMediaService
    {
        private VoiceRecordingService _voiceRecorder;

        public EnhancedMediaService()
        {
            _voiceRecorder = new VoiceRecordingService();
        }

        public async Task<MediaFile> TakePhotoAsync()
        {
            try
            {
                var permission = await CheckCameraPermissionAsync();
                if (!permission)
                    return null;

                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    return null;

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Recollect",
                    Name = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg",
                    SaveToAlbum = true,
                    CompressionQuality = 75,
                    PhotoSize = PhotoSize.Medium
                });

                return file;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error taking photo: {ex.Message}");
                return null;
            }
        }

        public async Task<MediaFile> RecordVideoAsync()
        {
            try
            {
                var permission = await CheckCameraPermissionAsync();
                if (!permission)
                    return null;

                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
                    return null;

                var file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                {
                    Directory = "Recollect",
                    Name = $"video_{DateTime.Now:yyyyMMdd_HHmmss}.mp4",
                    SaveToAlbum = true,
                    Quality = VideoQuality.Medium
                });

                return file;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error recording video: {ex.Message}");
                return null;
            }
        }

        public async Task<MediaFile> PickPhotoAsync()
        {
            try
                {
                var permission = await CheckStoragePermissionAsync();
                if (!permission)
                    return null;

                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                    return null;

                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 75
                });

                return file;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error picking photo: {ex.Message}");
                return null;
            }
        }

        public async Task<MediaFile> PickVideoAsync()
        {
            try
            {
                var permission = await CheckStoragePermissionAsync();
                if (!permission)
                    return null;

                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickVideoSupported)
                    return null;

                var file = await CrossMedia.Current.PickVideoAsync();

                return file;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error picking video: {ex.Message}");
                return null;
            }
        }

        public async Task<string> StartVoiceRecordingAsync()
        {
            try
            {
                var success = await _voiceRecorder.StartRecordingAsync();
                return success ? "Recording started" : "Failed to start recording";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting voice recording: {ex.Message}");
                return "Error starting recording";
            }
        }

        public async Task<string> StopVoiceRecordingAsync()
        {
            try
            {
                var audioData = await _voiceRecorder.StopRecordingAsync();
                return audioData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping voice recording: {ex.Message}");
                return null;
            }
        }

        public bool IsVoiceRecording => _voiceRecorder.IsRecording;

        public TimeSpan GetVoiceRecordingDuration => _voiceRecorder.GetRecordingDuration();

        public async Task<bool> CheckCameraPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                }
                return status == PermissionStatus.Granted;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckStoragePermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();
                }
                return status == PermissionStatus.Granted;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckMicrophonePermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Microphone>();
                }
                return status == PermissionStatus.Granted;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> ConvertMediaToBase64Async(MediaFile mediaFile)
        {
            try
            {
                if (mediaFile == null)
                    return null;

                using (var stream = mediaFile.GetStream())
                {
                    var bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                    return Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting media to base64: {ex.Message}");
                return null;
            }
        }
    }
}
