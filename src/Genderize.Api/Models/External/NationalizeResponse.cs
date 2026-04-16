using System.Text.Json.Serialization;

namespace Genderize.Api.Models.External;

public sealed class NationalizeResponse
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("country")]
    public IReadOnlyList<NationalizeCountryResponse> Country { get; init; } = Array.Empty<NationalizeCountryResponse>();
}
