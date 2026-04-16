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
    [ProducesResponseType(typeof(ApiCollectionResponse<IReadOnlyList<ProfileListItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfiles([FromQuery] string? gender, [FromQuery(Name = "country_id")] string? countryId, [FromQuery(Name = "age_group")] string? ageGroup, CancellationToken cancellationToken)
    {
        var result = await _profileService.GetProfilesAsync(
            new ProfileQueryDto
            {
                Gender = gender,
                CountryId = countryId,
                AgeGroup = ageGroup
            },
            cancellationToken);

        return Ok(ApiCollectionResponse<IReadOnlyList<ProfileListItemDto>>.Create(result.Count, result));
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
