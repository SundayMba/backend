using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Models.Dtos;
using Genderize.Api.Models.Requests;
using Genderize.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Genderize.Api.Controllers;

[ApiController]
[Route("api/profiles")]
public sealed class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiSuccessResponse<ProfileDetailsDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiSuccessMessageResponse<ProfileDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest? request, CancellationToken cancellationToken)
    {
        var name = ProfileRequestValidationHelper.ValidateName(request);
        var result = await _profileService.CreateProfileAsync(name, cancellationToken);

        if (result.AlreadyExists)
        {
            return Ok(ApiSuccessMessageResponse<ProfileDetailsDto>.Create("Profile already exists", result.Profile));
        }

        return StatusCode(StatusCodes.Status201Created, ApiSuccessResponse<ProfileDetailsDto>.Create(result.Profile));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiSuccessResponse<ProfileDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(Guid id, CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetProfileAsync(id, cancellationToken);
        return Ok(ApiSuccessResponse<ProfileDetailsDto>.Create(profile));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiPaginatedResponse<ProfileListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfiles(
        [FromQuery] string? gender,
        [FromQuery(Name = "age_group")] string? ageGroup,
        [FromQuery(Name = "country_id")] string? countryId,
        [FromQuery(Name = "min_age")] string? minAge,
        [FromQuery(Name = "max_age")] string? maxAge,
        [FromQuery(Name = "min_gender_probability")] string? minGenderProbability,
        [FromQuery(Name = "min_country_probability")] string? minCountryProbability,
        [FromQuery(Name = "sort_by")] string? sortBy,
        [FromQuery] string? order,
        [FromQuery] string? page,
        [FromQuery] string? limit,
        CancellationToken cancellationToken)
    {
        var query = ProfileQueryValidationHelper.Parse(
            gender,
            ageGroup,
            countryId,
            minAge,
            maxAge,
            minGenderProbability,
            minCountryProbability,
            sortBy,
            order,
            page,
            limit);

        var result = await _profileService.GetProfilesAsync(query, cancellationToken);

        return Ok(ApiPaginatedResponse<ProfileListItemDto>.Create(result.Page, result.Limit, result.Total, result.Data));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiPaginatedResponse<ProfileListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchProfiles([FromQuery(Name = "q")] string? query, [FromQuery] string? page, [FromQuery] string? limit, CancellationToken cancellationToken)
    {
        if (query is null)
        {
            return BadRequest(ApiErrorResponse.Create("The q query parameter is required"));
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(ApiErrorResponse.Create("The q query parameter cannot be empty"));
        }

        var pageValue = ProfileQueryValidationHelper.Parse(null, null, null, null, null, null, null, null, null, page, limit);
        var result = await _profileService.SearchProfilesAsync(query.Trim(), pageValue.Page, pageValue.Limit, cancellationToken);

        return Ok(ApiPaginatedResponse<ProfileListItemDto>.Create(result.Page, result.Limit, result.Total, result.Data));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfile(Guid id, CancellationToken cancellationToken)
    {
        await _profileService.DeleteProfileAsync(id, cancellationToken);
        return NoContent();
    }
}
