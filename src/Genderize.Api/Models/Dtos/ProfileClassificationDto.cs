namespace Genderize.Api.Models.Dtos;

public sealed class ProfileClassificationDto
{
    public required string Gender { get; init; }
    public decimal GenderProbability { get; init; }
    public int SampleSize { get; init; }
    public int Age { get; init; }
    public required string AgeGroup { get; init; }
    public required string CountryId { get; init; }
    public decimal CountryProbability { get; init; }
}
