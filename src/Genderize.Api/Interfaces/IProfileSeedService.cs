namespace Genderize.Api.Interfaces;

public interface IProfileSeedService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
