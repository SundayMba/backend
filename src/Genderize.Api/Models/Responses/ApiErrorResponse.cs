using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Responses
{
  public class ApiErrorResponse
  {
    [JsonPropertyName("message")]
    public string Message { get; set; } = String.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = "error";

    public static ApiErrorResponse Create(string message)
    {
      return new ApiErrorResponse { Message = message };
    }
  }
}
