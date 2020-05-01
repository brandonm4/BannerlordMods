using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TournamentXPanded.Configurator.Localization;

namespace TournamentXPanded.Configurator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 frm = new Form1();
            frm.TopLevel = true;
            frm.BringToFront();
            frm.Focus();
            string path = "";
            if (args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mount and Blade II Bannerlord", "Configs", "TournamentsXPanded", "tournamentxpsettings.json");
            }

            DirectoryInfo di = new DirectoryInfo(Assembly.GetEntryAssembly().Location);
            LocalizedTextManager.LoadLocalizationXmls(di.Parent.Parent.Parent.Parent.Parent.FullName);
            frm.SetupLanguageDropdown();
            frm.LoadConfig(path);


#if DEBUG
            string pathxml = "E:\\Users\\Brandon\\OneDrive - Mathis Consulting, LLC\\Development\\BannerLord\\BrandonMods\\TournamentXPanded.Configurator\\Languages\\settings-en.xml";
            string pathcs = "E:\\Users\\Brandon\\OneDrive - Mathis Consulting, LLC\\Development\\BannerLord\\BrandonMods\\TournamentsXPanded.Settings\\TournamentXPSettings.cs";
            string pathoutput = "e:\\settings.cs";

            var xmlstrings = File.ReadAllLines(pathxml);
            var csfile = File.ReadAllLines(pathcs);

            using (var sw = new StreamWriter(pathoutput, false))
            {
                foreach (var line in csfile)
                {
                    if (line.Trim().StartsWith("[SettingProperty"))
                    {
                        var parts = line.Split('"');
                        var newparts = "";
                        foreach(var p in parts)
                        {
                            var pt = LocalizedTextManager.FindStringId("English", p);
                            if (pt.StartsWith("{"))
                            {
                                //pt = pt + LocalizedTextManager.GetTranslatedText("English", LocalizedTextManager.GetStringId(pt));
                                pt = LocalizedTextManager.GetTranslatedText("English", LocalizedTextManager.GetStringId(pt));
                            }
                            newparts += pt + "\"";
                        }
                        newparts = newparts.Substring(0,newparts.Length - 1);
                        sw.WriteLine(newparts);
                    }
                    else
                    {
                        sw.WriteLine(line);
                    }
                }
            }
#endif


            Application.Run(frm);
        }
    }
}
