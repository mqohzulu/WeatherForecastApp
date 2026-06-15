namespace ChinaImportPlatform.Api.Common;

/// <summary>Domain/validation error that the global handler maps to a 4xx response.</summary>
public class AppException : Exception
{
    public int StatusCode { get; }

    public AppException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}

/// <summary>Thrown when a requested resource does not exist (mapped to 404).</summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }
}
