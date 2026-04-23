using Genderize.Api.Models.Dtos;

namespace Genderize.Api.Interfaces;

public interface IProfileQueryParser
{
    ProfileQueryDto Parse(string query);
}
