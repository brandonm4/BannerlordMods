using System.IO;

namespace TournamentsXPanded
{
    public class ErrorLog
    {
        public static string LogPath { get; set; }

        public static void Log(string text)
        {
            if (!string.IsNullOrEmpty(LogPath))
            {
                using (var sw = new StreamWriter(LogPath, true))
                {
                    sw.WriteLine(text);
                }
            }
        }
    }
}