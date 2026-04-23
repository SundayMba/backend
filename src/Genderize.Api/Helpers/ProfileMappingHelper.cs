using Genderize.Api.Entities;
using Genderize.Api.Models.Dtos;

namespace Genderize.Api.Helpers;

public static class ProfileMappingHelper
{
    public static ProfileDetailsDto ToDetailsDto(Profile profile)
    {
        return new ProfileDetailsDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Gender = profile.Gender,
            GenderProbability = profile.GenderProbability,
            Age = profile.Age,
            AgeGroup = profile.AgeGroup,
            CountryId = profile.CountryId,
            CountryName = profile.CountryName,
            CountryProbability = profile.CountryProbability,
            CreatedAt = profile.CreatedAt
        };
    }

    public static ProfileListItemDto ToListItemDto(Profile profile)
    {
        return new ProfileListItemDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Gender = profile.Gender,
            GenderProbability = profile.GenderProbability,
            Age = profile.Age,
            AgeGroup = profile.AgeGroup,
            CountryId = profile.CountryId,
            CountryName = profile.CountryName,
            CountryProbability = profile.CountryProbability,
            CreatedAt = profile.CreatedAt
        };
    }
}
