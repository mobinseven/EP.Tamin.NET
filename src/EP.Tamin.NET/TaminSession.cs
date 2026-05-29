using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EP.Tamin.NET;

public sealed class TaminSession
{
    private const string DefaultUrl = "https://ep-test.tamin.ir/api/";

    public HttpClient HttpClient { get; }
    public Uri BaseUri { get; }
    public ServiceClient Service { get; }
    public PrescriptionClient Prescription { get; }

    public TaminSession(HttpClient httpClient, string? oauthToken = null, Uri? baseUri = null, bool needToken = true)
    {
        HttpClient = httpClient;
        BaseUri = EnsureTrailingSlash(baseUri ?? new Uri(DefaultUrl));

        if (needToken && string.IsNullOrWhiteSpace(oauthToken))
            throw new AuthTokenNotSuppliedException();

        if (!string.IsNullOrWhiteSpace(oauthToken))
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

        Service = new ServiceClient(this);
        Prescription = new PrescriptionClient(this);
    }

    public static async Task<TaminSession> CreateAsync(
        HttpClient httpClient,
        string? oauthToken = null,
        Uri? baseUri = null,
        string? username = null,
        string? password = null,
        bool needToken = true,
        CancellationToken cancellationToken = default)
    {
        var normalizedBaseUri = EnsureTrailingSlash(baseUri ?? new Uri(DefaultUrl));

        if (needToken && string.IsNullOrWhiteSpace(oauthToken))
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new AuthTokenNotSuppliedException();

            oauthToken = await LoginAsync(httpClient, normalizedBaseUri, username, password, cancellationToken).ConfigureAwait(false);
        }

        return new TaminSession(httpClient, oauthToken, normalizedBaseUri, needToken);
    }

    internal Uri BuildUri(string endpoint, IReadOnlyDictionary<string, string?>? query = null)
    {
        var absolute = new Uri(BaseUri, endpoint.TrimStart('/'));
        if (query is null || query.Count == 0)
            return absolute;

        var queryString = string.Join("&", query
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value!)}"));

        if (string.IsNullOrEmpty(queryString))
            return absolute;

        var separator = absolute.Query.Length == 0 ? "?" : "&";
        return new Uri($"{absolute}{separator}{queryString}");
    }

    private static async Task<string> LoginAsync(HttpClient httpClient, Uri baseUri, string username, string password, CancellationToken cancellationToken)
    {
        var uri = new Uri(baseUri, "ws/api/auth/login");
        using var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(new { client_id = username, secret = password }), Encoding.UTF8, "application/json")
        };

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        using var doc = JsonDocument.Parse(content);

        if (response.IsSuccessStatusCode)
        {
            if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.String)
                return data.GetString()!;
        }

        throw new UserLoginException(
            doc.RootElement.TryGetProperty("data", out var dataNode) ? dataNode.ToString() : null,
            doc.RootElement.TryGetProperty("status", out var statusNode) && statusNode.TryGetInt32(out var status) ? status : null,
            doc.RootElement.TryGetProperty("family", out var familyNode) ? familyNode.ToString() : null,
            doc.RootElement.TryGetProperty("reason", out var reasonNode) ? reasonNode.ToString() : null);
    }

    private static Uri EnsureTrailingSlash(Uri uri)
    {
        if (uri.AbsoluteUri.EndsWith('/'))
            return uri;

        return new Uri($"{uri.AbsoluteUri}/");
    }
}
