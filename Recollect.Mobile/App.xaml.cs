using Recollect.Mobile.Data;
using Recollect.Mobile.Services;
using Microsoft.EntityFrameworkCore;

namespace Recollect.Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

    protected override Window CreateWindow(IActivationState? activationState)
    {
        try
        {
            // Minimal startup - no database initialization
            return new Window(new AppShell());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"App startup error: {ex.Message}");
            return new Window(new AppShell());
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            // Delay initialization to avoid startup issues
            await Task.Delay(1000);
            
            using var scope = Handler?.MauiContext?.Services?.CreateScope();
            if (scope?.ServiceProvider != null)
            {
                var context = scope.ServiceProvider.GetRequiredService<RecollectDbContext>();
                await context.Database.EnsureCreatedAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error but don't crash the app
            System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
        }
    }
}