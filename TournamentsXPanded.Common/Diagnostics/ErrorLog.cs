using System;
using System.IO;
using System.Linq;
using TaleWorlds.Library;

namespace XPanded.Common.Diagnostics
{
    public static class ErrorLog
    {
        public static string LogPath { get; set; }

        public static void Log(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(LogPath))
                {
                    using (var sw = new StreamWriter(LogPath, true))
                    {
                        string version = ModuleInfo.GetModules().Where(x => x.Name == "Tournaments XPanded").FirstOrDefault().Version.ToString();
                        sw.WriteLine(string.Concat(version, " ", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), "\n", text));
                    }
                }
            }
            catch {
                //Something has gone horribly wrong.
            }
        }
    }
}