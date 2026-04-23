namespace Genderize.Api.Entities;

public sealed class Profile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public decimal GenderProbability { get; set; }
    public int Age { get; set; }
    public string AgeGroup { get; set; } = string.Empty;
    public string CountryId { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public decimal CountryProbability { get; set; }
    public DateTime CreatedAt { get; set; }
}
