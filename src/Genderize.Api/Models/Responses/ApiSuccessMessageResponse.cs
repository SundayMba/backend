using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Responses;

public sealed class ApiSuccessMessageResponse<T>
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = "success";

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("data")]
    public required T Data { get; init; }

    public static ApiSuccessMessageResponse<T> Create(string message, T data)
    {
        return new ApiSuccessMessageResponse<T>
        {
            Message = message,
            Data = data
        };
    }
}
