# EP.Tamin.NET
کارآور سامانه نسخه الکترونیک تامین اجتماعی در .NET

A .NET 10 SDK implementation inspired by the Python `Mazafard/tamin-sdk` project.

## Features
- Session management with bearer token support
- Optional login flow (`ws/api/auth/login`) when username/password is provided
- Service endpoints:
  - `ws-services`
  - `ws-prescription-type`
  - `ws-par-taref`
  - `ws-drug-amount`
  - `ws-drug-instruction`
- Prescription endpoints:
  - `interface/epresc/SendEpresc` (create)
  - `interface/epresc/SendEpresc` (detail)

## Usage
```csharp
using EP.Tamin.NET;

var httpClient = new HttpClient();
var session = new TaminSession(httpClient, oauthToken: "YOUR_TOKEN");

var services = await session.Service.GetAllServicesAsync(new Dictionary<string, string?>
{
    ["serviceType"] = "17"
});
```

## Build & test
```bash
dotnet test EP.Tamin.NET.slnx
```
