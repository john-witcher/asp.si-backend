using System.Diagnostics;

namespace Api.Helpers;

public static class ProcessElevator
{
    /// <summary>
    /// Starts an external executable with elevated permissions.
    /// </summary>
    /// <param name="exePath">The path to the executable.</param>
    /// <param name="arguments">The arguments to pass to the executable.</param>
    /// <returns>The Process object that was started.</returns>
    public static Process? StartProcessWithElevation(string exePath, string arguments = "")
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
            return Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            // Handle exceptions such as when the user cancels the elevation request
            Console.WriteLine("Failed to start process with elevation: " + ex.Message);
            return null;
        }
    }
}