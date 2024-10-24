using System.Text.RegularExpressions;

namespace Api.Helpers;

public static class StringHelper
{
    public static string NormalizeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var pattern = new Regex("[^a-zA-Z0-9]");
        string alphanumericOnly = pattern.Replace(name, "");
        return alphanumericOnly.ToUpperInvariant();
    }
    
    public static bool IsStringifiable(Type type)
    {
        // Check if it's a primitive type, string, DateTime, Guid, or Enum
        return type.IsPrimitive // int, bool, double, etc.
               || type == typeof(string) 
               || type == typeof(DateTime)
               || type == typeof(Guid)
               || type.IsEnum;
    }
}