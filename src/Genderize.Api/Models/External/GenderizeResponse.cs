namespace Genderize.Api.Models.External;

public class GenderizeResponse
{
    public int Count { get; set; }
    public string? Gender { get; set; }
    public string? Name { get; set; }
    public double Probability { get; set; }
}