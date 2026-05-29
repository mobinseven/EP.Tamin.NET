using System.Text;
using System.Text.Json;

namespace EP.Tamin.NET;

public sealed class PrescriptionClient
{
    private const string Prefix = "interface/epresc/SendEpresc";
    private readonly TaminSession _session;

    internal PrescriptionClient(TaminSession session)
    {
        _session = session;
    }

    public Task<JsonElement> GetPrescriptionDetailAsync(IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        => GetAsync(string.Empty, query, cancellationToken);

    public Task<JsonElement> CreatePrescriptionAsync<TPayload>(TPayload payload, CancellationToken cancellationToken = default)
        => PostAsync(string.Empty, payload, cancellationToken);

    private async Task<JsonElement> GetAsync(string endpoint, IReadOnlyDictionary<string, string?>? query, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BuildPrescriptionUri(endpoint, query));
        using var response = await _session.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return ResponseHandling.Handle(response.StatusCode, response.ReasonPhrase, content);
    }

    private async Task<JsonElement> PostAsync<TPayload>(string endpoint, TPayload payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildPrescriptionUri(endpoint))
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        using var response = await _session.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return ResponseHandling.Handle(response.StatusCode, response.ReasonPhrase, content);
    }

    private Uri BuildPrescriptionUri(string endpoint, IReadOnlyDictionary<string, string?>? query = null)
    {
        var relativeEndpoint = string.IsNullOrWhiteSpace(endpoint) ? Prefix : $"{Prefix}/{endpoint.TrimStart('/')}";
        return _session.BuildUri(relativeEndpoint, query);
    }
}
