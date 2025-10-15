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
            .UseMauiMaps()  // Unlocks Map APIs; without it, runtime ghosts lurk    
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Database - Comment out for minimal version
        // builder.Services.AddDbContext<RecollectDbContext>(options =>
        //     options.UseSqlite($"Data Source={Path.Combine(FileSystem.AppDataDirectory, "recollect.db")}"));
        
        // Services - Minimal for testing
        builder.Services.AddSingleton<PermissionService>();
        // builder.Services.AddSingleton<LocationService>();
        // builder.Services.AddSingleton<MediaService>();
        // builder.Services.AddSingleton<AdventureService>();
        // builder.Services.AddScoped<DataService>();
        // builder.Services.AddHttpClient<ApiService>(client => client.BaseAddress = new Uri("https://localhost:7001"));

#if DEBUG
        builder.Services.AddLogging(logging => logging.AddDebug());
#endif
        return builder.Build();
    }
}