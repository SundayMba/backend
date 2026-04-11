using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Models.Dtos;
using Genderize.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Genderize.Api.Controllers;

[ApiController]
[Route("api/classify")]
public sealed class ClassifyController : ControllerBase
{
    private readonly IGenderizeService _genderizeService;

    public ClassifyController(IGenderizeService genderizeService)
    {
        _genderizeService = genderizeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiSuccessResponse<ClassifyResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Classify(CancellationToken cancellationToken)
    {
        if (!Request.Query.TryGetValue("name", out var rawName))
        {
            return BadRequest(ApiErrorResponse.Create("The name query parameter is required"));
        }

        var validationResult = NameValidationHelper.Validate(rawName);

        if (!validationResult.IsValid)
        {
            return StatusCode(validationResult.StatusCode, ApiErrorResponse.Create(validationResult.Message));
        }

        var result = await _genderizeService.ClassifyNameAsync(validationResult.NormalizedName!, cancellationToken);

        return Ok(ApiSuccessResponse<ClassifyResultDto>.Create(result));
    }
}
