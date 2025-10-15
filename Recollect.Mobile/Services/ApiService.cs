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
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}