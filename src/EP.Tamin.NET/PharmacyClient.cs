using System.Text;
using System.Text.Json;

namespace EP.Tamin.NET;

/// <summary>
/// Provides pharmacy dispensing operations (Section 12).
/// </summary>
public sealed class PharmacyClient
{
    private const string Prefix = "darman";
    private readonly TaminSession _session;

    internal PharmacyClient(TaminSession session)
    {
        _session = session;
    }

    /// <summary>Checks patient entitlement before dispensing (Section 12.2).</summary>
    public Task<JsonElement> CheckEntitlementAsync(CheckEntitlementRequest request, CancellationToken cancellationToken = default)
        => PostAsync("check-entitlement", request, cancellationToken);

    /// <summary>Registers a paper prescription in the pharmacy system (Section 12.3).</summary>
    public Task<JsonElement> RegisterPaperPrescriptionAsync(RegisterPaperPrescriptionRequest request, CancellationToken cancellationToken = default)
        => PostAsync("register-paper", request, cancellationToken);

    /// <summary>Fetches available prescriptions for dispensing (Section 12.4).</summary>
    public Task<JsonElement> GetPrescriptionListAsync(IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        => GetAsync("prescription-list", query, cancellationToken);

    /// <summary>Fetches prescription item-level details (Section 12.5).</summary>
    public Task<JsonElement> GetPrescriptionDetailsAsync(string prescriptionId, string trackingCode, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?> { ["prescription_id"] = prescriptionId, ["tracking_code"] = trackingCode };
        return GetAsync("prescription-details", query, cancellationToken);
    }

    /// <summary>Returns or refers a prescription to the doctor for correction (Section 12.6).</summary>
    public Task<JsonElement> ReferPrescriptionToDoctorAsync(ReferPrescriptionRequest request, CancellationToken cancellationToken = default)
        => PostAsync("refer-to-doctor", request, cancellationToken);

    /// <summary>Checks dispensing warnings before final delivery (Section 12.7).</summary>
    public Task<JsonElement> CheckPrescriptionWarningsAsync(CheckWarningRequest request, CancellationToken cancellationToken = default)
        => PostAsync("check-warnings", request, cancellationToken);

    /// <summary>Registers delivery of paper prescription items (Section 12.8).</summary>
    public Task<JsonElement> DispensePaperPrescriptionAsync(DispensePrescriptionRequest request, CancellationToken cancellationToken = default)
        => PostAsync("dispense-paper", request, cancellationToken);

    /// <summary>Registers delivery of electronic prescription items (Section 12.9).</summary>
    public Task<JsonElement> DispenseElectronicPrescriptionAsync(DispensePrescriptionRequest request, CancellationToken cancellationToken = default)
        => PostAsync("dispense-electronic", request, cancellationToken);

    /// <summary>
    /// Registers delivery when a warning exists and the workflow allows continuation (Section 12.10).
    /// </summary>
    public Task<JsonElement> DispenseWithWarningAsync(DispensePrescriptionRequest request, CancellationToken cancellationToken = default)
        => PostAsync("dispense-with-warning", request, cancellationToken);

    /// <summary>Registers a drug authenticity or tracking code (Section 12.11).</summary>
    public Task<JsonElement> RegisterDrugAuthenticityCodeAsync(DrugAuthenticityRequest request, CancellationToken cancellationToken = default)
        => PostAsync("register-authenticity-code", request, cancellationToken);

    /// <summary>Activates an authenticity code after delivery (Section 12.12).</summary>
    public Task<JsonElement> ActivateDrugAuthenticityCodeAsync(DrugAuthenticityRequest request, CancellationToken cancellationToken = default)
        => PostAsync("activate-authenticity-code", request, cancellationToken);

    /// <summary>Supports a two-step delivery process where required (Section 12.13).</summary>
    public Task<JsonElement> TwoStepElectronicDispensingAsync(DispensePrescriptionRequest request, CancellationToken cancellationToken = default)
        => PostAsync("two-step-dispense", request, cancellationToken);

    /// <summary>Shows an activated prescription or drug barcode (Section 12.14).</summary>
    public Task<JsonElement> GetActivatedBarcodeAsync(string prescriptionId, string trackingCode, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?> { ["prescription_id"] = prescriptionId, ["tracking_code"] = trackingCode };
        return GetAsync("activated-barcode", query, cancellationToken);
    }

    /// <summary>Shows price and share details for the dispensing (Section 12.15).</summary>
    public Task<JsonElement> GetPriceAsync(string prescriptionId, string trackingCode, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?> { ["prescription_id"] = prescriptionId, ["tracking_code"] = trackingCode };
        return GetAsync("price", query, cancellationToken);
    }

    /// <summary>Deletes or cancels a dispensing operation where allowed (Section 12.16).</summary>
    public Task<JsonElement> DeleteDispensingRecordAsync(DeletePrescriptionRequest request, CancellationToken cancellationToken = default)
        => PostAsync("delete-dispensing", request, cancellationToken);

    private async Task<JsonElement> GetAsync(string endpoint, IReadOnlyDictionary<string, string?>? query, CancellationToken cancellationToken)
    {
        using var request = _session.CreateRequest(HttpMethod.Get, _session.BuildUri($"{Prefix}/{endpoint}", query));
        using var response = await _session.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return ResponseHandling.Handle(response.StatusCode, response.ReasonPhrase, content);
    }

    private async Task<JsonElement> PostAsync<TPayload>(string endpoint, TPayload payload, CancellationToken cancellationToken)
    {
        using var request = _session.CreateRequest(HttpMethod.Post, _session.BuildUri($"{Prefix}/{endpoint}"));
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _session.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return ResponseHandling.Handle(response.StatusCode, response.ReasonPhrase, content);
    }
}
