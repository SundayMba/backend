namespace Genderize.Api.Helpers;

public sealed class NameValidationResult
{
    private NameValidationResult(bool isValid, string? normalizedName, int statusCode, string message)
    {
        IsValid = isValid;
        NormalizedName = normalizedName;
        StatusCode = statusCode;
        Message = message;
    }

    public bool IsValid { get; }

    public string? NormalizedName { get; }

    public int StatusCode { get; }

    public string Message { get; }

    public static NameValidationResult Valid(string normalizedName)
    {
        return new NameValidationResult(true, normalizedName, StatusCodes.Status200OK, string.Empty);
    }

    public static NameValidationResult Invalid(int statusCode, string message)
    {
        return new NameValidationResult(false, null, statusCode, message);
    }
}
