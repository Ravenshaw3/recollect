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
            // Initialize database for full functionality
            _ = Task.Run(InitializeDatabaseAsync);
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
                // Persist data across app updates: apply migrations if present, otherwise ensure created
                try
                {
                    var pending = await context.Database.GetPendingMigrationsAsync();
                    if (pending.Any())
                    {
                        await context.Database.MigrateAsync();
                    }
                    else
                    {
                        await context.Database.EnsureCreatedAsync();
                    }
                }
                catch
                {
                    // Fallback to EnsureCreated if migrations API fails for any reason
                    await context.Database.EnsureCreatedAsync();
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't crash the app
            System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
        }
    }
}