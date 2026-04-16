namespace Genderize.Api.Helpers;

public sealed class InvalidInputException : ApiException
{
    public InvalidInputException(string message)
        : base(StatusCodes.Status422UnprocessableEntity, message)
    {
    }
}
