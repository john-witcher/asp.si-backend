using System.Text;

namespace Api.Helpers;

public static class Encryption
{
    public static string EncodeToken(string token)
    {
        var encodingBytes = Encoding.UTF8.GetBytes(token);
        return Convert.ToBase64String(encodingBytes);
    }
    
    public static string DecodeToken(string token)
    {
        try
        {
            var decodedBytes = Convert.FromBase64String(token);
            return Encoding.UTF8.GetString(decodedBytes);
        }
        catch (FormatException)
        {
            throw new ArgumentException("The provided token is not a valid Base64 string.");
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}