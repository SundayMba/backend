using System.Net.Http.Json;
using System.Text.Json;
using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Models.Dtos;
using Genderize.Api.Models.External;

namespace Genderize.Api.Services;

public sealed class GenderizeService : IGenderizeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GenderizeService> _logger;

    public GenderizeService(IHttpClientFactory httpClientFactory, ILogger<GenderizeService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ClassifyResultDto> ClassifyNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientNames.GenderizeApi);
            using var response = await httpClient.GetAsync($"?name={Uri.EscapeDataString(name)}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Genderize API returned non-success status code: {StatusCode}", response.StatusCode);
                throw new UpstreamServiceException("Failed to retrieve data from external service");
            }

            var genderizeResponse = await response.Content.ReadFromJsonAsync<GenderizeResponse>(cancellationToken: cancellationToken);

            if (genderizeResponse is null)
            {
                _logger.LogError("Genderize API returned an empty or invalid response.");
                throw new UpstreamServiceException("Invalid response from external service");
            }

            if (string.IsNullOrWhiteSpace(genderizeResponse.Gender) || genderizeResponse.Count == 0)
            {
                throw new NoPredictionAvailableException("No prediction available for the provided name");
            }

            return new ClassifyResultDto
            {
                Name = name.Trim(),
                Gender = genderizeResponse.Gender!,
                Probability = genderizeResponse.Probability,
                SampleSize = genderizeResponse.Count,
                IsConfident = ConfidenceEvaluator.IsConfident(genderizeResponse.Probability, genderizeResponse.Count),
                ProcessedAt = DateTime.UtcNow
            };
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Error occurred while calling Genderize API.");
            throw new UpstreamServiceException("Failed to retrieve data from external service", exception);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(exception, "The request to Genderize API timed out.");
            throw new UpstreamServiceException("The external service timed out", exception);
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "Genderize API returned malformed JSON.");
            throw new UpstreamServiceException("Invalid response from external service", exception);
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "Genderize API returned an unsupported content type.");
            throw new UpstreamServiceException("Invalid response from external service", exception);
        }
    }
}
