namespace API.Exceptions;

public class ApiException : Exception
{
    public int? StatusCode { get; }
    public string? ErrorCode { get; }

    public ApiException(string message, int? statusCode = null, string? errorCode = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
