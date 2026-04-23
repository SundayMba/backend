using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Responses;

public sealed class ApiPaginatedResponse<T>
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = "success";

    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("limit")]
    public int Limit { get; init; }

    [JsonPropertyName("total")]
    public int Total { get; init; }

    [JsonPropertyName("data")]
    public required IReadOnlyList<T> Data { get; init; }

    public static ApiPaginatedResponse<T> Create(int page, int limit, int total, IReadOnlyList<T> data)
    {
        return new ApiPaginatedResponse<T>
        {
            Page = page,
            Limit = limit,
            Total = total,
            Data = data
        };
    }
}
