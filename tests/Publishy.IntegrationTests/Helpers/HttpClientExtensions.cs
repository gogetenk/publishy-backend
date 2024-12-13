using System.Net.Http.Json;
using System.Text.Json;

namespace Publishy.IntegrationTests.Helpers;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T?> GetFromJsonAsync<T>(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public static async Task<T?> PostAsJsonAsync<T>(this HttpClient client, string url, object data)
    {
        var response = await client.PostAsJsonAsync(url, data, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public static async Task<T?> PutAsJsonAsync<T>(this HttpClient client, string url, object data)
    {
        var response = await client.PutAsJsonAsync(url, data, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }
}