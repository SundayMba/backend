using System.Net.Http.Json;
using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Models.Dtos;
using Genderize.Api.Models.External;


namespace Genderize.Api.Services;

public class GenderizeService : IGenderizeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GenderizeService> _logger;

    public GenderizeService(HttpClient httpClient, ILogger<GenderizeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ClassifyResultDto?> ClassifyNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"?name={Uri.EscapeDataString(name)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Genderize API returned non-success status code: {StatusCode}", response.StatusCode);
            throw new HttpRequestException("Failed to retrieve data from external service.");
        }

        var genderizeResponse = await response.Content.ReadFromJsonAsync<GenderizeResponse>(cancellationToken: cancellationToken);

        if (genderizeResponse is null)
        {
            _logger.LogError("Genderize API returned an empty or invalid response.");
            throw new InvalidOperationException("Invalid response from external service.");
        }

        if (genderizeResponse.Gender is null || genderizeResponse.Count == 0)
        {
            return null;
        }

        return new ClassifyResultDto
        {
            Name = name.Trim(),
            Gender = genderizeResponse.Gender,
            Probability = genderizeResponse.Probability,
            SampleSize = genderizeResponse.Count,
            IsConfident = ConfidenceEvaluator.IsConfident(genderizeResponse.Probability, genderizeResponse.Count),
            ProcessedAt = DateTime.UtcNow
        };
    }
}