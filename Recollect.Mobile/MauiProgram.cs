using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection; // AddHttpClient's backstage pass
using Recollect.Mobile.Services;
using Recollect.Mobile.Data;
using Microsoft.EntityFrameworkCore;

namespace Recollect.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()  // Unlocks Map APIs; without it, runtime ghosts lurk    
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Initialize SQLite native provider early to avoid startup crashes on Android
        try
        {
            SQLitePCL.Batteries_V2.Init();
        }
        catch { /* ignore - will be initialized by EF if already loaded */ }

        // Database - Enable for full functionality
        builder.Services.AddDbContext<RecollectDbContext>(options =>
            options.UseSqlite($"Data Source={Path.Combine(FileSystem.AppDataDirectory, "recollect.db")}"));
        
        // Services
        builder.Services.AddSingleton<PermissionService>();
        builder.Services.AddSingleton<LocationService>();
        builder.Services.AddSingleton<MediaService>();
        builder.Services.AddSingleton<AdventureService>();
        builder.Services.AddScoped<DataService>();
        builder.Services.AddHttpClient<ApiService>(client =>
        {
            // Use dynamic configuration
            var baseUrl = ConfigurationService.GetApiBaseUrl();
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        builder.Services.AddSingleton<UploadQueueService>();

        // Feature pages
        builder.Services.AddTransient<Pages.AdventuresPage>();
        builder.Services.AddTransient<Pages.MapPage>();
        builder.Services.AddTransient<Pages.MediaPage>();
        builder.Services.AddTransient<Pages.NotesPage>();
        builder.Services.AddTransient<Pages.ReviewPage>();
        builder.Services.AddTransient<Pages.SettingsPage>();

#if DEBUG
        builder.Services.AddLogging(logging => logging.AddDebug());
#endif
        return builder.Build();
    }
}