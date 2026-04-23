namespace Genderize.Api.Models.Dtos;

public sealed class ProfileQueryDto
{
    public string? Gender { get; init; }
    public string? CountryId { get; init; }
    public string? AgeGroup { get; init; }
    public int? MinAge { get; init; }
    public int? MaxAge { get; init; }
    public decimal? MinGenderProbability { get; init; }
    public decimal? MinCountryProbability { get; init; }
    public string? SortBy { get; init; }
    public string? Order { get; init; }
    public int Page { get; init; } = 1;
    public int Limit { get; init; } = 10;
}
