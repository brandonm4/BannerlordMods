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
    public static class SettingsHelper
    {
        public static string ModuleFolderName { get; } = "TournamentsXPanded";
        public static bool GetSettings()
        {

          

            //Load Settings
            try
            {
                var overridefile =  System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "override.txt");
                bool overrideSettings = false;
                bool forceDebug = false;
                bool forceMenu = false;
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
                    if (overridetext.IndexOf("force modlib on", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        forceMenu = true;
                    }

                }

                var modnames = Utilities.GetModulesNames().ToList();
                bool modLibLoaded = false;
                if (modnames.Contains("ModLib") && !overrideSettings)
                {                    
                modLibLoaded =  SettingsHelperModLib.GetModLibSettings(forceDebug, forceMenu);
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

                    if (forceDebug)
                        settings.DebugMode = true;
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
