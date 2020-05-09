using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

using TournamentsXPanded.Models;

namespace TournamentsXPanded.Settings
{
    public static class SettingsHelper
    {
        public static string ModuleFolderName { get; } = "TournamentsXPanded";

        public static bool LoadSettings(string configPath)
        {
            //Load Settings
            try
            {
                var overridefile = System.IO.Path.Combine(configPath, "override.txt");
                bool overrideSettings = false;
                bool forceDebug = false;


                if (File.Exists(overridefile))
                {
                    string overridetext = File.ReadAllText(overridefile);
                    if (overridetext.IndexOf("force local settings", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        overrideSettings = true;
                    }
                    if (overridetext.IndexOf("force debug on", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        forceDebug = true;
                    }
                    if (overridetext.IndexOf("force disabled on", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return false;
                    }
                }
                else
                {
                    File.WriteAllText(overridefile, "");
                }
              
                bool modLibLoaded = false;
                
                if (!modLibLoaded)
                {
                    string configFilePath = System.IO.Path.Combine(configPath, "tournamentxpsettings.json");
                    TournamentXPSettings settings = GetFromFile(configFilePath);

                    if (forceDebug)
                    {
                        settings.DebugMode = true;
                    }
#if DEBUG
                    settings.DebugMode = true;
#endif
                    TournamentXPSettings.SetSettings(settings);
                }
            }
            catch 
            {
                // ErrorLog.Log("Error Loading Settings\n" + ex.ToStringFull());
                return false;
            }
            return true;
        }

        public static TournamentXPSettings GetFromFile(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            TournamentXPSettings settings = new TournamentXPSettings();
            if (File.Exists(System.IO.Path.GetFullPath(path)))
            {
                var settingsjson = File.ReadAllText(path);
                settings = JsonConvert.DeserializeObject<TournamentXPSettings>(settingsjson);
                //write new file to get latest updates
                File.WriteAllText(path, settingsjson);
            }
            else
            {
                var settingsjson = JsonConvert.SerializeObject(settings, serializerSettings);
                File.WriteAllText(path, settingsjson);
            }
            return settings;
        }
        public static void WriteSettings(string path, TournamentXPSettings settings)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            var settingsjson = JsonConvert.SerializeObject(settings, serializerSettings);
            File.WriteAllText(path, settingsjson);
        }
    }
}