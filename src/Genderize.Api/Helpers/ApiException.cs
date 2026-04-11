namespace Genderize.Api.Helpers;

public abstract class ApiException : Exception
{
    protected ApiException(int statusCode, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
