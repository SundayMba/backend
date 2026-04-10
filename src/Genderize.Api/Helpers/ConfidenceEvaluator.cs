namespace Genderize.Api.Helpers;

public static class ConfidenceEvaluator
{
  public static bool IsConfident(double probability, int sampleSize)
  {
    return probability >= 0.7 && sampleSize >= 100;
  }
}