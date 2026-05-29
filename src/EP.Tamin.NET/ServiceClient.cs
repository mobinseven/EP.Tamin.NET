using System.Text.Json;

namespace EP.Tamin.NET;

public sealed class ServiceClient
{
    private readonly TaminSession _session;

    internal ServiceClient(TaminSession session)
    {
        _session = session;
    }

    public Task<JsonElement> GetAllServicesAsync(IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        => GetAsync("ws-services", query, cancellationToken);

    public Task<JsonElement> GetPrescriptionTypeAsync(IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        => GetAsync("ws-prescription-type", query, cancellationToken);

    public Task<JsonElement> GetParaclinicTarefAsync(IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        => GetAsync("ws-par-taref", query, cancellationToken);

    public Task<JsonElement> GetDrugAmountAsync(IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        => GetAsync("ws-drug-amount", query, cancellationToken);

    public Task<JsonElement> GetDrugInstructionAsync(IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        => GetAsync("ws-drug-instruction", query, cancellationToken);

    private async Task<JsonElement> GetAsync(string endpoint, IReadOnlyDictionary<string, string?>? query, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _session.BuildUri(endpoint, query));
        using var response = await _session.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return ResponseHandling.Handle(response.StatusCode, response.ReasonPhrase, content);
    }
}
