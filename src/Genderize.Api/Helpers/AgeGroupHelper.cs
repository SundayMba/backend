namespace Genderize.Api.Helpers;

public static class AgeGroupHelper
{
    public static string FromAge(int age)
    {
        return age switch
        {
            >= 0 and <= 12 => "child",
            >= 13 and <= 19 => "teenager",
            >= 20 and <= 59 => "adult",
            _ => "senior"
        };
    }
}
