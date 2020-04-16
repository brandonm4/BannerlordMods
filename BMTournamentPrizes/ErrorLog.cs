using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
