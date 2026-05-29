namespace EP.Tamin.NET;

public sealed class Prescription
{
    public required Dictionary<string, object?> Values { get; init; }
}

public sealed record DocEprsc(string DocType = "1");

public enum PrescriptionType
{
    Drug = 1,
    Paraclinic = 2,
    Visit = 3,
    VisitService = 4,
    Service = 5,
}
