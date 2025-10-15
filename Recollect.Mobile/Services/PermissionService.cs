using Microsoft.Maui.ApplicationModel;

namespace Recollect.Mobile.Services;

public class PermissionService
{
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
            // For Android 9 and below, we need to request storage permission
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                
                if (status == PermissionStatus.Granted)
                    return true;
                    
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                return status == PermissionStatus.Granted;
            }
            
            return true; // iOS handles this differently
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Storage permission error: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> RequestAllPermissionsAsync()
    {
        var location = await RequestLocationPermissionAsync();
        var camera = await RequestCameraPermissionAsync();
        var storage = await RequestStoragePermissionAsync();
        
        return location && camera && storage;
    }
}
