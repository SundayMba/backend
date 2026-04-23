namespace Genderize.Api.Helpers;

public sealed class UnableToInterpretQueryException : ApiException
{
    public UnableToInterpretQueryException()
        : base(StatusCodes.Status400BadRequest, "Unable to interpret query")
    {
    }
}
