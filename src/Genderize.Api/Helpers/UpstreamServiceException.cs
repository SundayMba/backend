namespace Genderize.Api.Helpers;

public sealed class UpstreamServiceException : ApiException
{
    public UpstreamServiceException(string message, Exception? innerException = null)
        : base(StatusCodes.Status502BadGateway, message, innerException)
    {
    }
}
