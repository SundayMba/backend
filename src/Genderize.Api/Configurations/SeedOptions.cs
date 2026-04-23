namespace Genderize.Api.Configurations;

public sealed class SeedOptions
{
    public const string SectionName = "SeedData";

    public string FilePath { get; set; } = "data/seed_profiles.json";
    public bool Enabled { get; set; } = true;
}
