using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string pathxml = "E:\\Users\\Brandon\\OneDrive - Mathis Consulting, LLC\\Development\\BannerLord\\BrandonMods\\TournamentXPanded.Configurator\\Languages\\settings-en.xml";
            string pathcs = "E:\\Users\\Brandon\\OneDrive - Mathis Consulting, LLC\\Development\\BannerLord\\BrandonMods\\TournamentsXPanded.Settings\\TournamentXPSettings.cs";
            string pathoutput = "e:\\settings.cs";

            var xmlstrings = File.ReadAllLines(pathxml);
            var csfile = File.ReadAllLines(pathcs);

            using (var sw = new StreamWriter(pathoutput, false))
            {
                foreach(var line in csfile)
                {
                    if (line.Trim().StartsWith("[SettingProperty"))
                    {
                        var parts = line.Split('"');
                    }
                    else
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }
    }
}
