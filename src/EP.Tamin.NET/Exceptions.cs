using System.Net;

namespace EP.Tamin.NET;

public class AuthTokenNotSuppliedException : Exception
{
    public AuthTokenNotSuppliedException() : base("Authorization token not supplied") { }
}

public class UserLoginException : Exception
{
    public int? Status { get; }
    public string? Family { get; }
    public string? ReasonText { get; }

    public UserLoginException(string? data, int? status, string? family, string? reason)
        : base(data)
    {
        Status = status;
        Family = family;
        ReasonText = reason;
    }
}

public class ConnectionError : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ReasonPhrase { get; }
    public string? Content { get; }

    public ConnectionError(HttpStatusCode? statusCode, string? reasonPhrase, string? content, string? message = null)
        : base(message ?? "Failed.")
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
        Content = content;
    }
}

public class Redirection : ConnectionError { public Redirection(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class ClientError : ConnectionError { public ClientError(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class BadRequest : ClientError { public BadRequest(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class UnauthorizedAccess : ClientError { public UnauthorizedAccess(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class ForbiddenAccess : ClientError { public ForbiddenAccess(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class ResourceNotFound : ClientError { public ResourceNotFound(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class MethodNotAllowed : ClientError { public MethodNotAllowed(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class ResourceConflict : ClientError { public ResourceConflict(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class ResourceGone : ClientError { public ResourceGone(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class ResourceInvalid : ClientError { public ResourceInvalid(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
public class ServerError : ConnectionError { public ServerError(HttpStatusCode? s, string? r, string? c) : base(s, r, c) { } }
