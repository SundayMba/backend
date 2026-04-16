namespace Genderize.Api.Models.Dtos;

public sealed class ProfileQueryDto
{
    public string? Gender { get; init; }
    public string? CountryId { get; init; }
    public string? AgeGroup { get; init; }
}
