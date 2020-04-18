using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TournamentsXPanded.Models;
using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Settings
{
    public class SettingsHelper
    {
        public static string ModuleFolderName { get; } = "TournamentsXPanded";
        public static bool GetSettings()
        {
            //Load Settings
            try
            {
                var modnames = Utilities.GetModulesNames().ToList();
                bool modLibLoaded = false;
                if (modnames.Contains("ModLib"))
                {
                modLibLoaded =  SettingsHelperModLib.GetModLibSettings();
                }
                if (!modLibLoaded)
                {
                    TournamentXPSettings settings = new TournamentXPSettings();
                    string configPath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "tournamentxpsettings.json");
                    if (File.Exists(configPath))
                    {
                        var settingsjson = File.ReadAllText(configPath);
                        settings = JsonConvert.DeserializeObject<TournamentXPSettings>(settingsjson);
                    }
                    else
                    {
                        JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                        serializerSettings.Formatting = Formatting.Indented;

                        var settingsjson = JsonConvert.SerializeObject(settings, serializerSettings);
                        File.WriteAllText(configPath, settingsjson);
                    }
                    TournamentXPSettings.SetSettings(settings);
                }
            }
            catch(Exception ex)
            {
                ErrorLog.Log("Error Loading Settings\n" + ex.ToStringFull());
                return false;
            }
            return true;
        }
    }
}
