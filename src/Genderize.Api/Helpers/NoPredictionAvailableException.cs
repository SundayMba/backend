namespace Genderize.Api.Helpers;

public sealed class NoPredictionAvailableException : ApiException
{
    public NoPredictionAvailableException(string message)
        : base(StatusCodes.Status404NotFound, message)
    {
    }
}
