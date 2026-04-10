using System.Text.Json.Serialization;

namespace Genderize.Api.Models.Dtos
{
  public class ClassifyResultDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("probability")]
    public double Probability { get; set; }

    [JsonPropertyName("sample_size")]
    public int SampleSize { get; set; }

    [JsonPropertyName("is_confident")]
    public bool IsConfident { get; set; }

    [JsonPropertyName("processed_at")]
    public DateTime ProcessedAt { get; set; }
}
}