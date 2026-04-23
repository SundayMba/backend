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
    private readonly IProfileQueryParser _profileQueryParser;

    public ProfileService(AppDbContext dbContext, IProfileClassificationService classificationService, IProfileQueryParser profileQueryParser)
    {
        _dbContext = dbContext;
        _classificationService = classificationService;
        _profileQueryParser = profileQueryParser;
    }

    public async Task<CreateProfileResultDto> CreateProfileAsync(string name, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(profile => profile.Name.ToLower() == NormalizeName(name), cancellationToken);

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
            Gender = classification.Gender,
            GenderProbability = classification.GenderProbability,
            Age = classification.Age,
            AgeGroup = classification.AgeGroup,
            CountryId = classification.CountryId,
            CountryName = classification.CountryName,
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
                .FirstAsync(item => item.Name.ToLower() == NormalizeName(name), cancellationToken);

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

    public async Task<PaginatedResultDto<ProfileListItemDto>> GetProfilesAsync(ProfileQueryDto query, CancellationToken cancellationToken = default)
    {
        var profiles = ApplyQuery(_dbContext.Profiles.AsNoTracking(), query);
        var total = await profiles.CountAsync(cancellationToken);

        var data = await ApplySorting(profiles, query)
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .Select(profile => ProfileMappingHelper.ToListItemDto(profile))
            .ToListAsync(cancellationToken);

        return new PaginatedResultDto<ProfileListItemDto>
        {
            Page = query.Page,
            Limit = query.Limit,
            Total = total,
            Data = data
        };
    }

    public async Task<PaginatedResultDto<ProfileListItemDto>> SearchProfilesAsync(string queryText, int page, int limit, CancellationToken cancellationToken = default)
    {
        var parsed = _profileQueryParser.Parse(queryText);
        var query = new ProfileQueryDto
        {
            Gender = parsed.Gender,
            CountryId = parsed.CountryId,
            AgeGroup = parsed.AgeGroup,
            MinAge = parsed.MinAge,
            MaxAge = parsed.MaxAge,
            MinGenderProbability = parsed.MinGenderProbability,
            MinCountryProbability = parsed.MinCountryProbability,
            SortBy = parsed.SortBy,
            Order = parsed.Order,
            Page = page,
            Limit = limit
        };

        return await GetProfilesAsync(query, cancellationToken);
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

    private static IQueryable<Profile> ApplyQuery(IQueryable<Profile> profiles, ProfileQueryDto query)
    {
        if (!string.IsNullOrWhiteSpace(query.Gender))
        {
            profiles = profiles.Where(profile => profile.Gender == query.Gender);
        }

        if (!string.IsNullOrWhiteSpace(query.CountryId))
        {
            profiles = profiles.Where(profile => profile.CountryId == query.CountryId);
        }

        if (!string.IsNullOrWhiteSpace(query.AgeGroup))
        {
            profiles = profiles.Where(profile => profile.AgeGroup == query.AgeGroup);
        }

        if (query.MinAge.HasValue)
        {
            profiles = profiles.Where(profile => profile.Age >= query.MinAge.Value);
        }

        if (query.MaxAge.HasValue)
        {
            profiles = profiles.Where(profile => profile.Age <= query.MaxAge.Value);
        }

        if (query.MinGenderProbability.HasValue)
        {
            profiles = profiles.Where(profile => profile.GenderProbability >= query.MinGenderProbability.Value);
        }

        if (query.MinCountryProbability.HasValue)
        {
            profiles = profiles.Where(profile => profile.CountryProbability >= query.MinCountryProbability.Value);
        }

        return profiles;
    }

    private static IQueryable<Profile> ApplySorting(IQueryable<Profile> profiles, ProfileQueryDto query)
    {
        var order = query.Order ?? "desc";
        var sortBy = query.SortBy ?? "created_at";

        return (sortBy, order) switch
        {
            ("age", "asc") => profiles.OrderBy(profile => profile.Age).ThenBy(profile => profile.Id),
            ("age", "desc") => profiles.OrderByDescending(profile => profile.Age).ThenBy(profile => profile.Id),
            ("gender_probability", "asc") => profiles.OrderBy(profile => profile.GenderProbability).ThenBy(profile => profile.Id),
            ("gender_probability", "desc") => profiles.OrderByDescending(profile => profile.GenderProbability).ThenBy(profile => profile.Id),
            ("created_at", "asc") => profiles.OrderBy(profile => profile.CreatedAt).ThenBy(profile => profile.Id),
            _ => profiles.OrderByDescending(profile => profile.CreatedAt).ThenBy(profile => profile.Id)
        };
    }
}
