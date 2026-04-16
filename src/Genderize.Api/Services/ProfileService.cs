using Genderize.Api.Data;
using Genderize.Api.Entities;
using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Genderize.Api.Services;

public sealed class ProfileService : IProfileService
{
    private readonly AppDbContext _dbContext;
    private readonly IProfileClassificationService _classificationService;

    public ProfileService(AppDbContext dbContext, IProfileClassificationService classificationService)
    {
        _dbContext = dbContext;
        _classificationService = classificationService;
    }

    public async Task<CreateProfileResultDto> CreateProfileAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeName(name);
        var existing = await _dbContext.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(profile => profile.NormalizedName == normalizedName, cancellationToken);

        if (existing is not null)
        {
            return new CreateProfileResultDto
            {
                AlreadyExists = true,
                Profile = ProfileMappingHelper.ToDetailsDto(existing)
            };
        }

        var classification = await _classificationService.ClassifyAsync(name, cancellationToken);

        var profile = new Profile
        {
            Id = Uuid7Helper.Create(),
            Name = name,
            NormalizedName = normalizedName,
            Gender = classification.Gender,
            GenderProbability = classification.GenderProbability,
            SampleSize = classification.SampleSize,
            Age = classification.Age,
            AgeGroup = classification.AgeGroup,
            CountryId = classification.CountryId,
            CountryProbability = classification.CountryProbability,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Profiles.Add(profile);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            var duplicate = await _dbContext.Profiles
                .AsNoTracking()
                .FirstAsync(item => item.NormalizedName == normalizedName, cancellationToken);

            return new CreateProfileResultDto
            {
                AlreadyExists = true,
                Profile = ProfileMappingHelper.ToDetailsDto(duplicate)
            };
        }

        return new CreateProfileResultDto
        {
            AlreadyExists = false,
            Profile = ProfileMappingHelper.ToDetailsDto(profile)
        };
    }

    public async Task<ProfileDetailsDto> GetProfileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (profile is null)
        {
            throw new NotFoundException("Profile not found");
        }

        return ProfileMappingHelper.ToDetailsDto(profile);
    }

    public async Task<IReadOnlyList<ProfileListItemDto>> GetProfilesAsync(ProfileQueryDto query, CancellationToken cancellationToken = default)
    {
        var profiles = _dbContext.Profiles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Gender))
        {
            var gender = query.Gender.Trim().ToLowerInvariant();
            profiles = profiles.Where(profile => profile.Gender.ToLower() == gender);
        }

        if (!string.IsNullOrWhiteSpace(query.CountryId))
        {
            var countryId = query.CountryId.Trim().ToUpperInvariant();
            profiles = profiles.Where(profile => profile.CountryId.ToUpper() == countryId);
        }

        if (!string.IsNullOrWhiteSpace(query.AgeGroup))
        {
            var ageGroup = query.AgeGroup.Trim().ToLowerInvariant();
            profiles = profiles.Where(profile => profile.AgeGroup.ToLower() == ageGroup);
        }

        return await profiles
            .OrderBy(profile => profile.CreatedAt)
            .Select(profile => ProfileMappingHelper.ToListItemDto(profile))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteProfileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.Profiles
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (profile is null)
        {
            throw new NotFoundException("Profile not found");
        }

        _dbContext.Profiles.Remove(profile);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeName(string name)
    {
        return name.Trim().ToLowerInvariant();
    }
}
