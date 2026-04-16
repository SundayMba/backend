using System.Text.Json.Serialization;

namespace Genderize.Api.Models.External;

public sealed class NationalizeCountryResponse
{
    [JsonPropertyName("country_id")]
    public string? CountryId { get; init; }

    [JsonPropertyName("probability")]
    public decimal Probability { get; init; }
}
