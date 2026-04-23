using System.Text.RegularExpressions;
using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Models.Dtos;

namespace Genderize.Api.Services;

public sealed partial class ProfileQueryParser : IProfileQueryParser
{
    public ProfileQueryDto Parse(string query)
    {
        var normalized = Normalize(query);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new UnableToInterpretQueryException();
        }

        var hasMale = ContainsWord(normalized, "male") || ContainsWord(normalized, "males");
        var hasFemale = ContainsWord(normalized, "female") || ContainsWord(normalized, "females");

        string? gender = null;
        if (hasMale && !hasFemale)
        {
            gender = "male";
        }
        else if (hasFemale && !hasMale)
        {
            gender = "female";
        }

        var ageGroup = ParseAgeGroup(normalized);
        var (minAge, maxAge) = ParseAgeRange(normalized);
        var countryId = ParseCountry(normalized);

        var result = new ProfileQueryDto
        {
            Gender = gender,
            AgeGroup = ageGroup,
            MinAge = minAge,
            MaxAge = maxAge,
            CountryId = countryId,
            Page = 1,
            Limit = 10
        };

        if (gender is null && ageGroup is null && minAge is null && maxAge is null && countryId is null)
        {
            throw new UnableToInterpretQueryException();
        }

        ProfileQueryValidationHelper.Validate(result);
        return result;
    }

    private static string Normalize(string query)
    {
        return MultiSpaceRegex().Replace(query.Trim().ToLowerInvariant(), " ");
    }

    private static string? ParseAgeGroup(string query)
    {
        if (ContainsAny(query, "child", "children"))
        {
            return "child";
        }

        if (ContainsAny(query, "teen", "teens", "teenager", "teenagers"))
        {
            return "teenager";
        }

        if (ContainsAny(query, "adult", "adults"))
        {
            return "adult";
        }

        if (ContainsAny(query, "senior", "seniors"))
        {
            return "senior";
        }

        return null;
    }

    private static (int? MinAge, int? MaxAge) ParseAgeRange(string query)
    {
        int? minAge = null;
        int? maxAge = null;

        if (ContainsWord(query, "young"))
        {
            minAge = 16;
            maxAge = 24;
        }

        var betweenMatch = BetweenRegex().Match(query);
        if (betweenMatch.Success)
        {
            minAge = int.Parse(betweenMatch.Groups[1].Value);
            maxAge = int.Parse(betweenMatch.Groups[2].Value);
        }

        var aboveMatch = AboveRegex().Match(query);
        if (aboveMatch.Success)
        {
            minAge = int.Parse(aboveMatch.Groups[1].Value);
        }

        var belowMatch = BelowRegex().Match(query);
        if (belowMatch.Success)
        {
            maxAge = int.Parse(belowMatch.Groups[1].Value);
        }

        return (minAge, maxAge);
    }

    private static string? ParseCountry(string query)
    {
        var fromMatch = FromCountryRegex().Match(query);
        if (!fromMatch.Success)
        {
            return null;
        }

        var countryText = fromMatch.Groups[1].Value.Trim();
        if (string.IsNullOrWhiteSpace(countryText))
        {
            return null;
        }

        return CountryLookupHelper.TryGetCountryCode(countryText, out var code) ? code : null;
    }

    private static bool ContainsAny(string query, params string[] candidates)
    {
        return candidates.Any(candidate => ContainsWord(query, candidate));
    }

    private static bool ContainsWord(string query, string word)
    {
        return Regex.IsMatch(query, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultiSpaceRegex();

    [GeneratedRegex(@"\bbetween\s+(\d+)\s+and\s+(\d+)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BetweenRegex();

    [GeneratedRegex(@"\b(?:above|over|older than|at least)\s+(\d+)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex AboveRegex();

    [GeneratedRegex(@"\b(?:below|under|younger than|at most)\s+(\d+)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BelowRegex();

    [GeneratedRegex(@"\bfrom\s+([a-z .'-]+?)(?:\s+(?:above|below|under|over|older than|younger than|at least|at most|between)\b|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex FromCountryRegex();
}
