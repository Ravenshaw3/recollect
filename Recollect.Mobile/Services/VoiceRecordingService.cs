using Plugin.AudioRecorder;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Recollect.Mobile.Services
{
    public class VoiceRecordingService
    {
        private AudioRecorderService _audioRecorder;
        private bool _isRecording = false;

        public VoiceRecordingService()
        {
            _audioRecorder = new AudioRecorderService
            {
                StopRecordingOnSilence = false,
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromMinutes(10),
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };
        }

        public bool IsRecording => _isRecording;

        public async Task<bool> StartRecordingAsync()
        {
            try
            {
                if (_isRecording)
                    return false;

                var permission = await CheckMicrophonePermissionAsync();
                if (!permission)
                    return false;

                await _audioRecorder.StartRecording();
                _isRecording = true;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting recording: {ex.Message}");
                return false;
            }
        }

        public async Task<string> StopRecordingAsync()
        {
            try
            {
                if (!_isRecording)
                    return null;

                var audioFile = await _audioRecorder.StopRecording();
                _isRecording = false;

                if (audioFile != null)
                {
                    // Convert to base64 for API transmission
                    var audioBytes = await File.ReadAllBytesAsync(audioFile);
                    return Convert.ToBase64String(audioBytes);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping recording: {ex.Message}");
                return null;
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

        public TimeSpan GetRecordingDuration()
        {
            return _audioRecorder.GetAudioFileInfo()?.Duration ?? TimeSpan.Zero;
        }

        public void CancelRecording()
        {
            if (_isRecording)
            {
                _audioRecorder.StopRecording();
                _isRecording = false;
            }
        }
    }
}
