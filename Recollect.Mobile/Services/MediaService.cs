using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

namespace Recollect.Mobile.Services;

public class MediaService
{
    private readonly PermissionService _permissionService;

    public MediaService(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<FileResult?> CapturePhotoAsync()
    {
        try
        {
            // Request camera permission first
            var hasPermission = await _permissionService.RequestCameraPermissionAsync();
            if (!hasPermission)
            {
                System.Diagnostics.Debug.WriteLine("Camera permission denied");
                return null;
            }

            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                return photo;
            }
            else
            {
                // Fallback to picking from gallery
                return await MediaPicker.Default.PickPhotoAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Photo capture error: {ex.Message}");
            return null;
        }
    }

    public async Task<FileResult?> CaptureVideoAsync()
    {
        try
        {
            // Request camera permission first
            var hasPermission = await _permissionService.RequestCameraPermissionAsync();
            if (!hasPermission)
            {
                System.Diagnostics.Debug.WriteLine("Camera permission denied");
                return null;
            }

            if (MediaPicker.Default.IsCaptureSupported)
            {
                var video = await MediaPicker.Default.CaptureVideoAsync();
                return video;
            }
            else
            {
                // Fallback to picking from gallery
                return await MediaPicker.Default.PickVideoAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Video capture error: {ex.Message}");
            return null;
        }
    }

    public async Task<FileResult?> PickFromGalleryAsync()
    {
        try
        {
            // Use FilePicker to avoid runtime media permission requirements on newer Android
            var imageTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "image/*" } },
                { DevicePlatform.iOS, new[] { "public.image" } },
                { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" } },
                { DevicePlatform.MacCatalyst, new[] { "public.image" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a photo",
                FileTypes = imageTypes
            });

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Gallery pick error: {ex.Message}");
            return null;
        }
    }

    public async Task<FileResult?> PickVideoFromGalleryAsync()
    {
        try
        {
            var result = await MediaPicker.Default.PickVideoAsync();
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
