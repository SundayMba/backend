namespace Genderize.Api.Helpers;

public sealed class InvalidQueryParametersException : ApiException
{
    public InvalidQueryParametersException()
        : base(StatusCodes.Status400BadRequest, "Invalid query parameters")
    {
    }
}
