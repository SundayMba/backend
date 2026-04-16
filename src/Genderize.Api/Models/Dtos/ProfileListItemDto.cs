using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Dtos;

public sealed class ProfileListItemDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("gender")]
    public required string Gender { get; init; }

    [JsonPropertyName("age")]
    public int Age { get; init; }

    [JsonPropertyName("age_group")]
    public required string AgeGroup { get; init; }

    [JsonPropertyName("country_id")]
    public required string CountryId { get; init; }
}
