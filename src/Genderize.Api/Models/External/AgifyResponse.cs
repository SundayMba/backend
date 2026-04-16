using System.Text.Json.Serialization;

namespace Genderize.Api.Models.External;

public sealed class AgifyResponse
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("age")]
    public int? Age { get; init; }

    [JsonPropertyName("count")]
    public int Count { get; init; }
}
