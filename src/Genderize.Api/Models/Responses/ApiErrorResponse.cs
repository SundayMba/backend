namespace Genderize.Api.Models.Responses
{
  public class ApiErrorResponse
  {
    public string Message { get; set; } = String.Empty;
    public string Status { get; set; } = "error";
  }
}