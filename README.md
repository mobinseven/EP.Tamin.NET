# EP.Tamin.NET
کارآور سامانه نسخه الکترونیک تامین اجتماعی در .NET

A .NET 10 SDK for the [EP.Tamin](https://ep.tamin.ir) electronic prescription API (Social Security Insurance of Iran).
Inspired by the Python [`Mazafard/tamin-sdk`](https://github.com/Mazafard/tamin-sdk).

## Features

| Domain | Operations |
|---|---|
| **Authentication** | Login (token or username/password), RefreshToken, ValidateToken, OTP support |
| **Identity & Eligibility** | VerifyIdentity, CheckEntitlement |
| **E-Prescription Writing** | Visit, Drug, Paraclinic, MedicalService, Referral, Physiotherapy |
| **Prescription Query** | GetRegisteredPrescription, GetReferralPrescription, GetPrescriptionList |
| **Prescription Mutation** | EditElectronicPrescription, DeleteElectronicPrescription |
| **Reference Data** | GetDrugList, GetServiceList, GetAllowedCount, GetPrice, GetPrescriptionType, GetParaclinicTaref, GetDrugInstruction |
| **Warning Services** | CheckPrescriptionWarning |
| **Pharmacy Dispensing** | 16 operations — entitlement check, paper/electronic dispense, barcode registration/activation, warnings, price, and deletion |
| **Paraclinic Dispensing** | 10 operations — entitlement check, paper/electronic service delivery, warnings, price, and deletion |

## Installation

```bash
dotnet add package EP.Tamin.NET
```

## Basic Usage

```csharp
using EP.Tamin.NET;

// With a pre-obtained token
var session = new TaminSession(new HttpClient(), oauthToken: "YOUR_TOKEN", clientId: "YOUR_CLIENT_ID");

// Or let the SDK log in for you (with optional OTP)
var session = await TaminSession.CreateAsync(
    new HttpClient(),
    username: "your-username",
    password: "your-password",
    otp: "123456",            // optional
    clientId: "YOUR_CLIENT_ID");

// Reference data
var services = await session.Service.GetAllServicesAsync();
var drugs    = await session.Service.GetDrugListAsync(searchText: "amoxicillin", activeOnly: true);
var price    = await session.Service.GetPriceAsync("DR001", "drug", quantity: 2);

// Identity & eligibility
var identity    = await session.Identity.VerifyIdentityAsync(new VerifyIdentityRequest { NationalId = "1234567890" });
var entitlement = await session.Identity.CheckEntitlementAsync(new CheckEntitlementRequest
{
    NationalId  = "1234567890",
    ProviderId  = "clinic-id",
    VisitDate   = "2024-06-01",
    ServiceType = "clinic"
});

// Register a drug prescription
var result = await session.Prescription.RegisterDrugPrescriptionAsync(new RegisterDrugPrescriptionRequest
{
    DoctorId          = "doctor-id",
    PatientNationalId = "1234567890",
    VisitDate         = "2024-06-01",
    DrugItems         =
    [
        new DrugItem { DrugCode = "DR001", Quantity = 2, DosageInstruction = "twice daily" }
    ]
});
```

## Dependency Injection

```csharp
// Program.cs / Startup.cs
builder.Services.AddTaminClient(o =>
{
    o.BaseUrl    = "https://ep-test.tamin.ir/api/"; // or production URL
    o.ClientId   = "YOUR_CLIENT_ID";
    o.OAuthToken = "YOUR_TOKEN";                    // or set Username + Password
});

// Inject TaminSession anywhere:
public class PrescriptionService(TaminSession tamin) { ... }
```

## Build & test

```bash
dotnet test EP.Tamin.NET.slnx
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## License

[MIT](LICENSE)

