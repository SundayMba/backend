using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Dtos;

public sealed class ProfileDetailsDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("gender")]
    public required string Gender { get; init; }

    [JsonPropertyName("gender_probability")]
    public decimal GenderProbability { get; init; }

    [JsonPropertyName("age")]
    public int Age { get; init; }

    [JsonPropertyName("age_group")]
    public required string AgeGroup { get; init; }

    [JsonPropertyName("country_id")]
    public required string CountryId { get; init; }

    [JsonPropertyName("country_name")]
    public required string CountryName { get; init; }

    [JsonPropertyName("country_probability")]
    public decimal CountryProbability { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }
}
