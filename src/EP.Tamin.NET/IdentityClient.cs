using System.Text;
using System.Text.Json;

namespace EP.Tamin.NET;

/// <summary>
/// Provides patient identity verification and treatment eligibility operations (Section 7).
/// </summary>
public sealed class IdentityClient
{
    private readonly TaminSession _session;

    internal IdentityClient(TaminSession session)
    {
        _session = session;
    }

    /// <summary>
    /// Verifies a patient's identity before issuing a prescription (Section 7.1).
    /// </summary>
    /// <param name="request">Identity verification parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<JsonElement> VerifyIdentityAsync(VerifyIdentityRequest request, CancellationToken cancellationToken = default)
        => PostAsync("ws-verify-identity", request, cancellationToken);

    /// <summary>
    /// Checks whether a patient has active treatment coverage (Section 7.2).
    /// </summary>
    /// <param name="request">Entitlement check parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<JsonElement> CheckEntitlementAsync(CheckEntitlementRequest request, CancellationToken cancellationToken = default)
        => PostAsync("ws-check-entitlement", request, cancellationToken);

    private async Task<JsonElement> PostAsync<TPayload>(string endpoint, TPayload payload, CancellationToken cancellationToken)
    {
        using var request = _session.CreateRequest(HttpMethod.Post, _session.BuildUri(endpoint));
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _session.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return ResponseHandling.Handle(response.StatusCode, response.ReasonPhrase, content);
    }
}
