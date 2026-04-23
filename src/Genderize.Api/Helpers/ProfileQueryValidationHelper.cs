using Genderize.Api.Models.Dtos;

namespace Genderize.Api.Helpers;

public static class ProfileQueryValidationHelper
{
    public static ProfileQueryDto Parse(
        string? gender,
        string? ageGroup,
        string? countryId,
        string? minAge,
        string? maxAge,
        string? minGenderProbability,
        string? minCountryProbability,
        string? sortBy,
        string? order,
        string? page,
        string? limit)
    {
        var query = new ProfileQueryDto
        {
            Gender = NormalizeOptionalLower(gender),
            AgeGroup = NormalizeOptionalLower(ageGroup),
            CountryId = NormalizeOptionalUpper(countryId),
            MinAge = ParseNullableInt(minAge),
            MaxAge = ParseNullableInt(maxAge),
            MinGenderProbability = ParseNullableDecimal(minGenderProbability),
            MinCountryProbability = ParseNullableDecimal(minCountryProbability),
            SortBy = NormalizeOptionalLower(sortBy),
            Order = NormalizeOptionalLower(order),
            Page = ParseNullableInt(page) ?? 1,
            Limit = ParseNullableInt(limit) ?? 10
        };

        Validate(query);
        return query;
    }

    public static void Validate(ProfileQueryDto query)
    {
        if (query.Page < 1 || query.Limit < 1 || query.Limit > 50)
        {
            throw new InvalidQueryParametersException();
        }

        if (query.MinAge is < 0 || query.MaxAge is < 0)
        {
            throw new InvalidQueryParametersException();
        }

        if (query.MinAge.HasValue && query.MaxAge.HasValue && query.MinAge > query.MaxAge)
        {
            throw new InvalidQueryParametersException();
        }

        if (query.MinGenderProbability is < 0 or > 1 || query.MinCountryProbability is < 0 or > 1)
        {
            throw new InvalidQueryParametersException();
        }

        if (query.Gender is not null && query.Gender is not ("male" or "female"))
        {
            throw new InvalidQueryParametersException();
        }

        if (query.AgeGroup is not null && query.AgeGroup is not ("child" or "teenager" or "adult" or "senior"))
        {
            throw new InvalidQueryParametersException();
        }

        if (query.SortBy is not null && query.SortBy is not ("age" or "created_at" or "gender_probability"))
        {
            throw new InvalidQueryParametersException();
        }

        if (query.Order is not null && query.Order is not ("asc" or "desc"))
        {
            throw new InvalidQueryParametersException();
        }

        if (query.CountryId is not null && query.CountryId.Length != 2)
        {
            throw new InvalidQueryParametersException();
        }
    }

    private static int? ParseNullableInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!int.TryParse(value.Trim(), out var result))
        {
            throw new InvalidInputException("Invalid query parameters");
        }

        return result;
    }

    private static decimal? ParseNullableDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!decimal.TryParse(value.Trim(), out var result))
        {
            throw new InvalidInputException("Invalid query parameters");
        }

        return result;
    }

    private static string? NormalizeOptionalLower(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
    }

    private static string? NormalizeOptionalUpper(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
