using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Seed;

public sealed class SeedProfilesDocument
{
    [JsonPropertyName("profiles")]
    public IReadOnlyList<SeedProfileItem> Profiles { get; init; } = Array.Empty<SeedProfileItem>();
}
