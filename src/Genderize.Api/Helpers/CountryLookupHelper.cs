using System.Globalization;

namespace Genderize.Api.Helpers;

public static class CountryLookupHelper
{
    private static readonly Lazy<Dictionary<string, string>> CountryNameToCode = new(BuildNameToCodeMap);
    private static readonly Lazy<Dictionary<string, string>> CountryCodeToName = new(BuildCodeToNameMap);

    public static string GetCountryName(string countryCode)
    {
        if (CountryCodeToName.Value.TryGetValue(countryCode.ToUpperInvariant(), out var countryName))
        {
            return countryName;
        }

        return countryCode.ToUpperInvariant();
    }

    public static bool TryGetCountryCode(string countryName, out string countryCode)
    {
        return CountryNameToCode.Value.TryGetValue(countryName.Trim().ToLowerInvariant(), out countryCode!);
    }

    private static Dictionary<string, string> BuildNameToCodeMap()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in BuildCodeToNameMap())
        {
            result[pair.Value.ToLowerInvariant()] = pair.Key;
        }

        result["usa"] = "US";
        result["us"] = "US";
        result["uk"] = "GB";
        result["ivory coast"] = "CI";
        result["dr congo"] = "CD";
        result["drc"] = "CD";
        result["democratic republic of the congo"] = "CD";
        result["republic of the congo"] = "CG";

        return result;
    }

    private static Dictionary<string, string> BuildCodeToNameMap()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            try
            {
                var region = new RegionInfo(culture.Name);
                result[region.TwoLetterISORegionName] = region.EnglishName;
            }
            catch
            {
                continue;
            }
        }

        result["CD"] = "Democratic Republic of the Congo";
        result["CG"] = "Republic of the Congo";

        return result;
    }
}
