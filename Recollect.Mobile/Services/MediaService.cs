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
            var result = await MediaPicker.Default.PickPhotoAsync();
            return result;
        }
        catch (Exception)
        {
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
