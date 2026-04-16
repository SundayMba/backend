using System.Net.Http.Json;
using System.Text.Json;
using Genderize.Api.Helpers;
using Genderize.Api.Interfaces;
using Genderize.Api.Models.Dtos;
using Genderize.Api.Models.External;

namespace Genderize.Api.Services;

public sealed class ProfileClassificationService : IProfileClassificationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ProfileClassificationService> _logger;

    public ProfileClassificationService(IHttpClientFactory httpClientFactory, ILogger<ProfileClassificationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ProfileClassificationDto> ClassifyAsync(string name, CancellationToken cancellationToken = default)
    {
        var genderize = await GetGenderizeAsync(name, cancellationToken);
        var agify = await GetAgifyAsync(name, cancellationToken);
        var nationalize = await GetNationalizeAsync(name, cancellationToken);

        var topCountry = nationalize.Country
            .Where(country => !string.IsNullOrWhiteSpace(country.CountryId))
            .OrderByDescending(country => country.Probability)
            .FirstOrDefault();

        if (topCountry is null)
        {
            throw new UpstreamServiceException("Nationalize returned an invalid response");
        }

        return new ProfileClassificationDto
        {
            Gender = genderize.Gender!,
            GenderProbability = Convert.ToDecimal(genderize.Probability),
            SampleSize = genderize.Count,
            Age = agify.Age!.Value,
            AgeGroup = AgeGroupHelper.FromAge(agify.Age.Value),
            CountryId = topCountry.CountryId!,
            CountryProbability = topCountry.Probability
        };
    }

    private async Task<GenderizeResponse> GetGenderizeAsync(string name, CancellationToken cancellationToken)
    {
        var response = await SendAsync<GenderizeResponse>(HttpClientNames.GenderizeApi, name, "Genderize", cancellationToken);

        if (string.IsNullOrWhiteSpace(response.Gender) || response.Count == 0)
        {
            throw new UpstreamServiceException("Genderize returned an invalid response");
        }

        return response;
    }

    private async Task<AgifyResponse> GetAgifyAsync(string name, CancellationToken cancellationToken)
    {
        var response = await SendAsync<AgifyResponse>(HttpClientNames.AgifyApi, name, "Agify", cancellationToken);

        if (response.Age is null)
        {
            throw new UpstreamServiceException("Agify returned an invalid response");
        }

        return response;
    }

    private async Task<NationalizeResponse> GetNationalizeAsync(string name, CancellationToken cancellationToken)
    {
        var response = await SendAsync<NationalizeResponse>(HttpClientNames.NationalizeApi, name, "Nationalize", cancellationToken);

        if (response.Country.Count == 0)
        {
            throw new UpstreamServiceException("Nationalize returned an invalid response");
        }

        return response;
    }

    private async Task<T> SendAsync<T>(string clientName, string name, string externalApiName, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(clientName);
            using var response = await client.GetAsync($"?name={Uri.EscapeDataString(name)}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("{ExternalApi} returned non-success status code: {StatusCode}", externalApiName, response.StatusCode);
                throw new UpstreamServiceException($"{externalApiName} returned an invalid response");
            }

            var payload = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

            if (payload is null)
            {
                throw new UpstreamServiceException($"{externalApiName} returned an invalid response");
            }

            return payload;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Failed to call {ExternalApi}.", externalApiName);
            throw new UpstreamServiceException($"{externalApiName} returned an invalid response", exception);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(exception, "{ExternalApi} timed out.", externalApiName);
            throw new UpstreamServiceException($"{externalApiName} returned an invalid response", exception);
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "{ExternalApi} returned malformed JSON.", externalApiName);
            throw new UpstreamServiceException($"{externalApiName} returned an invalid response", exception);
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "{ExternalApi} returned unsupported content.", externalApiName);
            throw new UpstreamServiceException($"{externalApiName} returned an invalid response", exception);
        }
    }
}
