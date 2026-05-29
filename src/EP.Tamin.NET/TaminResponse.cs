using System.Text.Json.Serialization;

namespace EP.Tamin.NET;

/// <summary>
/// Standard API response wrapper matching the EP.Tamin API envelope:
/// <c>{ success, statusCode, message, data, trackingCode, correlationId }</c>.
/// </summary>
/// <typeparam name="T">The type of the <c>data</c> payload.</typeparam>
public sealed class TaminResponse<T>
{
    /// <summary>Whether the request succeeded.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>API-level status code string.</summary>
    [JsonPropertyName("statusCode")]
    public string? StatusCode { get; init; }

    /// <summary>Human-readable message.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>Response payload.</summary>
    [JsonPropertyName("data")]
    public T? Data { get; init; }

    /// <summary>Tracking code assigned by the API.</summary>
    [JsonPropertyName("trackingCode")]
    public string? TrackingCode { get; init; }

    /// <summary>Correlation ID echoed from the request.</summary>
    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }

    /// <summary>Validation or business-rule errors (present on failure).</summary>
    [JsonPropertyName("errors")]
    public IReadOnlyList<string>? Errors { get; init; }
}

/// <summary>Status values for a prescription through its lifecycle.</summary>
public enum PrescriptionStatus
{
    /// <summary>Created locally but not yet submitted.</summary>
    Draft,
    /// <summary>Sent to the official API.</summary>
    Submitted,
    /// <summary>Accepted by the official API.</summary>
    Accepted,
    /// <summary>Rejected by the official API.</summary>
    Rejected,
    /// <summary>Accepted conditionally or requires user attention.</summary>
    Warning,
    /// <summary>Changed after original registration.</summary>
    Edited,
    /// <summary>Deleted or cancelled where allowed.</summary>
    Deleted,
    /// <summary>Waiting for retry after temporary failure.</summary>
    PendingSync,
    /// <summary>Submission failed and requires support review.</summary>
    Failed,
}
