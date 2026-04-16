using Genderize.Api.Models.Dtos;

namespace Genderize.Api.Interfaces;

public interface IProfileClassificationService
{
    Task<ProfileClassificationDto> ClassifyAsync(string name, CancellationToken cancellationToken = default);
}
