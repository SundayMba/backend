using System.Text.Json;
using Genderize.Api.Configurations;
using Genderize.Api.Data;
using Genderize.Api.Entities;
using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Models.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Genderize.Api.Services;

public sealed class ProfileSeedService : IProfileSeedService
{
    private readonly AppDbContext _dbContext;
    private readonly SeedOptions _seedOptions;
    private readonly ILogger<ProfileSeedService> _logger;
    private readonly IWebHostEnvironment _environment;

    public ProfileSeedService(
        AppDbContext dbContext,
        IOptions<SeedOptions> seedOptions,
        ILogger<ProfileSeedService> logger,
        IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _seedOptions = seedOptions.Value;
        _logger = logger;
        _environment = environment;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_seedOptions.Enabled)
        {
            return;
        }

        var filePath = Path.IsPathRooted(_seedOptions.FilePath)
            ? _seedOptions.FilePath
            : Path.Combine(_environment.ContentRootPath, _seedOptions.FilePath);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Seed file not found at {FilePath}. Skipping profile seed.", filePath);
            return;
        }

        await using var stream = File.OpenRead(filePath);
        var document = await JsonSerializer.DeserializeAsync<SeedProfilesDocument>(stream, cancellationToken: cancellationToken);

        if (document is null || document.Profiles.Count == 0)
        {
            _logger.LogWarning("Seed file at {FilePath} did not contain profiles.", filePath);
            return;
        }

        var existingNames = await _dbContext.Profiles
            .AsNoTracking()
            .Select(profile => profile.Name.ToLower())
            .ToListAsync(cancellationToken);

        var existingNameSet = existingNames.ToHashSet();
        var profilesToInsert = new List<Profile>();

        foreach (var seedProfile in document.Profiles)
        {
            var normalizedName = seedProfile.Name.Trim().ToLowerInvariant();
            if (existingNameSet.Contains(normalizedName))
            {
                continue;
            }

            profilesToInsert.Add(new Profile
            {
                Id = Uuid7Helper.Create(),
                Name = seedProfile.Name.Trim(),
                Gender = seedProfile.Gender.Trim().ToLowerInvariant(),
                GenderProbability = seedProfile.GenderProbability,
                Age = seedProfile.Age,
                AgeGroup = seedProfile.AgeGroup.Trim().ToLowerInvariant(),
                CountryId = seedProfile.CountryId.Trim().ToUpperInvariant(),
                CountryName = seedProfile.CountryName.Trim(),
                CountryProbability = seedProfile.CountryProbability,
                CreatedAt = DateTime.UtcNow
            });

            existingNameSet.Add(normalizedName);
        }

        if (profilesToInsert.Count == 0)
        {
            return;
        }

        _dbContext.Profiles.AddRange(profilesToInsert);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
