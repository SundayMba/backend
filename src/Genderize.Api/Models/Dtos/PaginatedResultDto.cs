namespace Genderize.Api.Models.Dtos;

public sealed class PaginatedResultDto<T>
{
    public int Page { get; init; }
    public int Limit { get; init; }
    public int Total { get; init; }
    public required IReadOnlyList<T> Data { get; init; }
}
