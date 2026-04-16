using System.Text.Json;
using Genderize.Api.Models.Requests;

namespace Genderize.Api.Helpers;

public static class ProfileRequestValidationHelper
{
    public static string ValidateName(CreateProfileRequest? request)
    {
        if (request is null || request.Name.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            throw new BadHttpRequestException("The name field is required", StatusCodes.Status400BadRequest);
        }

        if (request.Name.ValueKind != JsonValueKind.String)
        {
            throw new InvalidInputException("The name field must be a string");
        }

        var name = request.Name.GetString();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadHttpRequestException("The name field cannot be empty", StatusCodes.Status400BadRequest);
        }

        var normalized = name.Trim();

        if (!NameValidationHelper.IsHumanReadableName(normalized))
        {
            throw new InvalidInputException("The name field must be a valid string");
        }

        return normalized;
    }
}
