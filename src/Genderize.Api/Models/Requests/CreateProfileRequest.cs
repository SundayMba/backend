using System.Text.Json;
using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Requests;

public sealed class CreateProfileRequest
{
    [JsonPropertyName("name")]
    public JsonElement Name { get; init; }
}
