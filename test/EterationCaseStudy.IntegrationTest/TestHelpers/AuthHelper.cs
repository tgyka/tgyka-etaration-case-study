using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EterationCaseStudy.IntegrationTest;

public static class AuthHelper
{
    public static async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var resp = await client.PostAsJsonAsync("/api/v1/auth/login", new { Email = email, Password = password });
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        var token = doc.RootElement.GetProperty("token").GetString();
        return token!;
    }

    public static void UseBearer(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}

