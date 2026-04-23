using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Seed;

public sealed class SeedProfileItem
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; init; } = string.Empty;

    [JsonPropertyName("gender_probability")]
    public decimal GenderProbability { get; init; }

    [JsonPropertyName("age")]
    public int Age { get; init; }

    [JsonPropertyName("age_group")]
    public string AgeGroup { get; init; } = string.Empty;

    [JsonPropertyName("country_id")]
    public string CountryId { get; init; } = string.Empty;

    [JsonPropertyName("country_name")]
    public string CountryName { get; init; } = string.Empty;

    [JsonPropertyName("country_probability")]
    public decimal CountryProbability { get; init; }
}
