using Genderize.Api.Models.Dtos;
namespace Genderize.Api.Interfaces;

public interface IGenderizeService
{
  Task<ClassifyResultDto> ClassifyNameAsync(string name, CancellationToken cancellationToken = default);
}
