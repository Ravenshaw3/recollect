using Recollect.Mobile.Services;

namespace Recollect.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Ask for required permissions on first load
        this.Loaded += async (_, __) =>
        {
            try
            {
                var services = Application.Current?.Handler?.MauiContext?.Services;
                var permissionService = services?.GetService(typeof(PermissionService)) as PermissionService;
                if (permissionService != null)
                {
                    var granted = await permissionService.RequestAllPermissionsAsync();
                    if (!granted)
                    {
                        await DisplayAlert(
                            "Permissions required",
                            "Please grant Location, Camera, and Storage permissions for full functionality.",
                            "OK");
                    }
                }
            }
            catch { /* best-effort; avoid blocking app startup */ }
        };
    }
}
