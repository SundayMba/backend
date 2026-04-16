namespace Genderize.Api.Models.Dtos;

public sealed class CreateProfileResultDto
{
    public required ProfileDetailsDto Profile { get; init; }
    public bool AlreadyExists { get; init; }
}
