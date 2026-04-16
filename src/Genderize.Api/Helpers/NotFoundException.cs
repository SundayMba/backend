namespace Genderize.Api.Helpers;

public sealed class NotFoundException : ApiException
{
    public NotFoundException(string message)
        : base(StatusCodes.Status404NotFound, message)
    {
    }
}
