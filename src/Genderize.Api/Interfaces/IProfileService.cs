using Genderize.Api.Models.Dtos;

namespace Genderize.Api.Interfaces;

public interface IProfileService
{
    Task<CreateProfileResultDto> CreateProfileAsync(string name, CancellationToken cancellationToken = default);
    Task<ProfileDetailsDto> GetProfileAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProfileListItemDto>> GetProfilesAsync(ProfileQueryDto query, CancellationToken cancellationToken = default);
    Task DeleteProfileAsync(Guid id, CancellationToken cancellationToken = default);
}
