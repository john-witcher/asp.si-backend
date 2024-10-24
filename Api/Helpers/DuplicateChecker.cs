using System.Diagnostics;

namespace Api.Helpers;

public class DuplicateChecker
{
    public static void CheckFor(string exePath, string arguments = "")
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = arguments,
            UseShellExecute = true, // Required to run as admin
            Verb = "runas", // This verb triggers elevation request
            WindowStyle = ProcessWindowStyle.Normal, // Or Hidden, if you don't want to show the console window
        };

        try
        {
            // return Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            // Handle exceptions such as when the user cancels the elevation request
            Console.WriteLine("Failed to start process with elevation: " + ex.Message);
            // return null;
        }
    }
}