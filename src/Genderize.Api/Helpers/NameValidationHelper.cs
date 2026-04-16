using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace Genderize.Api.Helpers;

public static partial class NameValidationHelper
{
    private static readonly HashSet<string> ReservedNonNameValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "true",
        "false",
        "null",
        "undefined"
    };

    public static NameValidationResult Validate(StringValues rawValues)
    {
        if (rawValues.Count > 1)
        {
            return NameValidationResult.Invalid(
                StatusCodes.Status422UnprocessableEntity,
                "The name query parameter must contain a single value");
        }

        var value = rawValues.ToString();

        if (string.IsNullOrWhiteSpace(value))
        {
            return NameValidationResult.Invalid(
                StatusCodes.Status400BadRequest,
                "The name query parameter cannot be empty");
        }

        var normalizedValue = value.Trim();

        if (!IsHumanReadableName(normalizedValue))
        {
            return NameValidationResult.Invalid(
                StatusCodes.Status422UnprocessableEntity,
                "The name query parameter must be a valid string");
        }

        return NameValidationResult.Valid(normalizedValue);
    }

    public static bool IsHumanReadableName(string value)
    {
        return !ReservedNonNameValues.Contains(value) && NamePattern().IsMatch(value);
    }

    [GeneratedRegex(@"^\p{L}+(?:[ '-]\p{L}+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex NamePattern();
}
