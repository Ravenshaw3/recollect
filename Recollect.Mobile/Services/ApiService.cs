using Recollect.Mobile.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace Recollect.Mobile.Services;

public class ApiService
{
    private readonly HttpClient http;

    public ApiService(HttpClient client) => http = client;

    public async Task<bool> UploadAdventureAsync(Adventure adventure, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(adventure);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await http.PostAsync("/api/adventures", content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse>(responseContent);
                return result?.Success == true;
            }
            
            return false;
        }
        catch (HttpRequestException)
        {
            // Log error
            return false;
        }
    }

    public async Task<List<Adventure>?> GetAllAdventuresAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await http.GetAsync("/api/adventures", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Adventure>>(content);
            }
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<Adventure?> GetAdventureAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await http.GetAsync($"/api/adventures/{id}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Adventure>(content);
            }
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAdventureAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await http.DeleteAsync($"/api/adventures/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public async Task<bool> UploadAudioAsync(int adventureId, Stream audioStream, string fileName, double? latitude = null, double? longitude = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            var streamContent = new StreamContent(audioStream);
            // Let server detect; fallback to generic audio
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/*");
            form.Add(streamContent, name: "file", fileName: fileName);

            // Backend endpoint path
            var url = "/api/media/upload-audio";
            // Optional context as headers (if needed later)
            var query = new List<string> { $"adventureId={adventureId}" };
            if (latitude.HasValue) query.Add($"lat={latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            if (longitude.HasValue) query.Add($"lng={longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            url += $"?{string.Join("&", query)}";

            var response = await http.PostAsync(url, form, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public async Task<bool> UploadPhotoAsync(int adventureId, Stream stream, string fileName, double? latitude = null, double? longitude = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/*");
            form.Add(streamContent, name: "file", fileName: fileName);

            var url = "/api/media/upload-photo";
            var query = new List<string> { $"adventureId={adventureId}" };
            if (latitude.HasValue) query.Add($"lat={latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            if (longitude.HasValue) query.Add($"lng={longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            url += $"?{string.Join("&", query)}";

            var response = await http.PostAsync(url, form, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public async Task<bool> UploadVideoAsync(int adventureId, Stream stream, string fileName, double? latitude = null, double? longitude = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("video/*");
            form.Add(streamContent, name: "file", fileName: fileName);

            var url = "/api/media/upload-video";
            var query = new List<string> { $"adventureId={adventureId}" };
            if (latitude.HasValue) query.Add($"lat={latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            if (longitude.HasValue) query.Add($"lng={longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            url += $"?{string.Join("&", query)}";

            var response = await http.PostAsync(url, form, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}