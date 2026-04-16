using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Responses;

public sealed class ApiCollectionResponse<T>
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = "success";

    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("data")]
    public required T Data { get; init; }

    public static ApiCollectionResponse<T> Create(int count, T data)
    {
        return new ApiCollectionResponse<T>
        {
            Count = count,
            Data = data
        };
    }
}
