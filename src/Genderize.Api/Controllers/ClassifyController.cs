using Genderize.Api.Interfaces;
using Genderize.Api.Models.Dtos;
using Genderize.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Genderize.Api.Controllers;

[ApiController]
[Route("api")]
public class ClassifyController : ControllerBase
{
    private readonly IGenderizeService _genderizeService;
    private readonly ILogger<ClassifyController> _logger;

    public ClassifyController(
        IGenderizeService genderizeService,
        ILogger<ClassifyController> logger)
    {
        _genderizeService = genderizeService;
        _logger = logger;
    }

    [HttpGet("classify")]
    [ProducesResponseType(typeof(ApiSuccessResponse<ClassifyResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Classify([FromQuery] string? name, CancellationToken cancellationToken)
    {
        if (name is null)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = "The name query parameter is required"
            });
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = "The name query parameter cannot be empty"
            });
        }

        var trimmedName = name.Trim();

        if (trimmedName.Any(char.IsDigit))
        {
            return UnprocessableEntity(new ApiErrorResponse
            {
                Message = "The name query parameter must be a valid string"
            });
        }

        try
        {
            var result = await _genderizeService.ClassifyNameAsync(trimmedName, cancellationToken);

            if (result is null)
            {
                return UnprocessableEntity(new ApiErrorResponse
                {
                    Message = "No prediction available for the provided name"
                });
            }

            var response = new ApiSuccessResponse<ClassifyResultDto>
            {
                Data = result
            };

            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error occurred while calling Genderize API.");

            return StatusCode(StatusCodes.Status502BadGateway, new ApiErrorResponse
            {
                Message = "Failed to retrieve data from external service"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An unexpected error occurred"
            });
        }
    }
}