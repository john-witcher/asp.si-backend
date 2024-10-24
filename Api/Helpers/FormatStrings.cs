using System.Text.RegularExpressions;

namespace Api.Helpers;

public static class FormatStrings
{
    public static string NormalizeName(string name)
    {
            if (string.IsNullOrEmpty(name)) return name;

            var pattern = new Regex("[^a-zA-Z0-9]");
            string alphanumericOnly = pattern.Replace(name, "");
            return alphanumericOnly.ToUpperInvariant();
        }
}