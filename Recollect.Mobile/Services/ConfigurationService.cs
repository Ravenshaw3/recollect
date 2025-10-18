using Microsoft.Maui.Storage;

namespace Recollect.Mobile.Services;

public class ConfigurationService
{
    private static readonly Dictionary<string, string> _configurations = new()
    {
        // Local development
        ["local"] = "http://localhost:7001",
        
        // Tailscale (your actual Tailscale IP)
        ["tailscale"] = "http://100.82.128.95:7001", // Your Tailscale IP
        
        // Direct internet access
        ["production"] = "https://your-domain.com",
        
        // Cloudflare tunnel
        ["cloudflare"] = "https://your-tunnel.your-domain.com"
    };

    public static string GetApiBaseUrl()
    {
        // Check for environment variable first
        var envUrl = Environment.GetEnvironmentVariable("RECOLLECT_API_URL");
        if (!string.IsNullOrEmpty(envUrl))
        {
            return envUrl;
        }

        // Check user preferences
        var preferredConfig = Preferences.Get("api_configuration", "local");
        
        if (_configurations.TryGetValue(preferredConfig, out var url))
        {
            return url;
        }

        // Default to local
        return _configurations["local"];
    }

    public static void SetApiConfiguration(string configuration)
    {
        if (_configurations.ContainsKey(configuration))
        {
            Preferences.Set("api_configuration", configuration);
        }
    }

    public static List<string> GetAvailableConfigurations()
    {
        return _configurations.Keys.ToList();
    }
}
