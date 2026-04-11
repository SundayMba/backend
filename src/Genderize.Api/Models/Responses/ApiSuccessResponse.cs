using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Responses
{
  public class ApiSuccessResponse<T>
  {
    [JsonPropertyName("status")]
    public string Status { get; set; } = "success";
    
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;

    public static ApiSuccessResponse<T> Create(T data)
    {
      return new ApiSuccessResponse<T> { Data = data };
    }
  }
}
