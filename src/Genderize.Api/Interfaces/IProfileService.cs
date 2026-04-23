using Genderize.Api.Models.Dtos;

namespace Genderize.Api.Interfaces;

public interface IProfileService
{
    Task<CreateProfileResultDto> CreateProfileAsync(string name, CancellationToken cancellationToken = default);
    Task<ProfileDetailsDto> GetProfileAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResultDto<ProfileListItemDto>> GetProfilesAsync(ProfileQueryDto query, CancellationToken cancellationToken = default);
    Task<PaginatedResultDto<ProfileListItemDto>> SearchProfilesAsync(string queryText, int page, int limit, CancellationToken cancellationToken = default);
    Task DeleteProfileAsync(Guid id, CancellationToken cancellationToken = default);
}
