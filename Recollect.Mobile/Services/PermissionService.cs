using Microsoft.Maui.ApplicationModel;

namespace Recollect.Mobile.Services;

public class PermissionService
{
    private static bool IsAndroid => DeviceInfo.Platform == DevicePlatform.Android;
    private static int AndroidVersion => IsAndroid ? DeviceInfo.Version.Major : 0;

    public async Task<bool> RequestLocationPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            
            if (status == PermissionStatus.Granted)
                return true;
                
            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.Android)
            {
                // On Android, we can ask the user to go to settings
                return false;
            }
            
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Location permission error: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> RequestCameraPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            
            if (status == PermissionStatus.Granted)
                return true;
                
            status = await Permissions.RequestAsync<Permissions.Camera>();
            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Camera permission error: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> RequestStoragePermissionAsync()
    {
        try
        {
            // For Android 12 and below, request storage permissions
            if (IsAndroid && AndroidVersion <= 12)
            {
                var read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                var write = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                
                if (read == PermissionStatus.Granted && write == PermissionStatus.Granted)
                    return true;
                    
                read = await Permissions.RequestAsync<Permissions.StorageRead>();
                write = await Permissions.RequestAsync<Permissions.StorageWrite>();
                return read == PermissionStatus.Granted && write == PermissionStatus.Granted;
            }
            
            return true; // iOS handles this differently
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Storage permission error: {ex.Message}");
            return false;
        }
    }

    // For gallery/media picker access
    public async Task<bool> RequestMediaPickerPermissionsAsync()
    {
        try
        {
            // Android 13+ uses scoped media picker; no runtime permission needed
            if (IsAndroid && AndroidVersion >= 13)
            {
                return true;
            }
            // Older Android needs storage read
            var read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (read == PermissionStatus.Granted) return true;
            read = await Permissions.RequestAsync<Permissions.StorageRead>();
            return read == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Media picker permission error: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> RequestAllPermissionsAsync()
    {
        var location = await RequestLocationPermissionAsync();
        var camera = await RequestCameraPermissionAsync();
        var storage = await RequestStoragePermissionAsync();
        var mediaPicker = await RequestMediaPickerPermissionsAsync();
        
        return location && camera && storage && mediaPicker;
    }
}
