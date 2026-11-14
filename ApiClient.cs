using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RaceDay.Mvc.Services;

public sealed class ApiUnavailableException() : Exception("The RaceDay API is currently unavailable. Please try again shortly.");
public sealed class ApiSessionExpiredException() : Exception("Your API session has expired.");

public class ApiClient(HttpClient client, IHttpContextAccessor accessor)
{
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private void ForwardApiCookie()
    {
        client.DefaultRequestHeaders.Remove("Cookie");
        var cookie = accessor.HttpContext?.Session.GetString(SessionKeys.ApiCookie);
        if (!string.IsNullOrWhiteSpace(cookie)) client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", cookie);
    }

    public async Task<T?> Get<T>(string path)
    {
        ForwardApiCookie();
        HttpResponseMessage response;
        try { response = await client.GetAsync(path); }
        catch (HttpRequestException) { throw new ApiUnavailableException(); }
        catch (TaskCanceledException) { throw new ApiUnavailableException(); }
        if (response.StatusCode == HttpStatusCode.Unauthorized) throw new ApiSessionExpiredException();
        if (response.StatusCode == HttpStatusCode.NotFound) return default;
        if (!response.IsSuccessStatusCode) throw new ApiUnavailableException();
        return await response.Content.ReadFromJsonAsync<T>(_json);
    }

    public async Task<(bool ok, string message, JsonElement? data)> Send(string method, string path, object? body = null)
    {
        ForwardApiCookie();
        using var request = new HttpRequestMessage(new HttpMethod(method), path);
        if (body is not null) request.Content = new StringContent(JsonSerializer.Serialize(body, _json), Encoding.UTF8, "application/json");
        HttpResponseMessage response;
        try { response = await client.SendAsync(request); }
        catch (HttpRequestException) { throw new ApiUnavailableException(); }
        catch (TaskCanceledException) { throw new ApiUnavailableException(); }
        CaptureApiCookie(path, response);
        if (response.StatusCode == HttpStatusCode.Unauthorized && path != "api/auth/login")
            throw new ApiSessionExpiredException();
        return await ReadResponse(response);
    }

    public async Task<(bool ok, string message, JsonElement? data)> Upload(string path, IFormFile file)
    {
        ForwardApiCookie();
        using var form = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var part = new StreamContent(stream);
        part.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        form.Add(part, "file", file.FileName);
        HttpResponseMessage response;
        try { response = await client.PostAsync(path, form); }
        catch (HttpRequestException) { throw new ApiUnavailableException(); }
        catch (TaskCanceledException) { throw new ApiUnavailableException(); }
        if (response.StatusCode == HttpStatusCode.Unauthorized) throw new ApiSessionExpiredException();
        return await ReadResponse(response);
    }

    private void CaptureApiCookie(string path, HttpResponseMessage response)
    {
        if (path == "api/auth/login" && response.Headers.TryGetValues("Set-Cookie", out var values))
        {
            var cookie = values.FirstOrDefault()?.Split(';')[0];
            if (cookie is not null) accessor.HttpContext?.Session.SetString(SessionKeys.ApiCookie, cookie);
        }
    }

    private async Task<(bool ok, string message, JsonElement? data)> ReadResponse(HttpResponseMessage response)
    {
        var raw = await response.Content.ReadAsStringAsync();
        JsonElement? data = null;
        var message = response.IsSuccessStatusCode ? "Request completed." : "Request failed. Please try again.";
        if (!string.IsNullOrWhiteSpace(raw) && response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            try
            {
                using var document = JsonDocument.Parse(raw);
                data = document.RootElement.Clone();
                if ((int)response.StatusCode < 500 && data.Value.ValueKind == JsonValueKind.Object &&
                    data.Value.TryGetProperty("message", out var messageElement) && messageElement.ValueKind == JsonValueKind.String)
                    message = messageElement.GetString() ?? message;
            }
            catch (JsonException)
            {
                data = null;
            }
        }
        return (response.IsSuccessStatusCode, message, data);
    }
}
