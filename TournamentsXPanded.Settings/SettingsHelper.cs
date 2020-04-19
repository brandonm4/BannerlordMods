using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

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
                var overridefile = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "override.txt");
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
                else
                {
                    File.WriteAllText(overridefile, "");                 
                }
                var modnames = Utilities.GetModulesNames().ToList();
                bool modLibLoaded = false;
                if (modnames.Contains("ModLib") && !overrideSettings)
                {
                    //    modLibLoaded =  SettingsHelperModLib.GetModLibSettings(forceDebug, forceMenu);

                    var modlibsettingsdll = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetBasePath(), "Modules", ModuleFolderName, "bin", "Win64_Shipping_Client", "TournamentsXPanded.Settings.ModLib.dll");
                    modlibsettingsdll = System.IO.Path.GetFullPath(modlibsettingsdll);
                    Assembly assembly = Assembly.LoadFile(modlibsettingsdll);
                    Type settingsHelperModLibType = assembly.GetType("TournamentsXPanded.Settings.SettingsHelperModLib");

                    TournamentXPSettings osettings = (TournamentXPSettings)settingsHelperModLibType.GetMethod("GetModLibSettings", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Invoke(null, new object[] { forceDebug, forceMenu });
                    if (osettings != null)
                    {
                        TournamentXPSettings.SetSettings(osettings);
                        modLibLoaded = true;
                    }
                }
                if (!modLibLoaded)
                {
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                    serializerSettings.Formatting = Formatting.Indented;
                    TournamentXPSettings settings = new TournamentXPSettings();
                    string configPath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "tournamentxpsettings.json");
                    if (File.Exists(configPath))
                    {
                        var settingsjson = File.ReadAllText(configPath);
                        settings = JsonConvert.DeserializeObject<TournamentXPSettings>(settingsjson);
                        //write new file to get latest updates
                        File.WriteAllText(configPath, settingsjson);
                    }
                    else
                    {                      
                        var settingsjson = JsonConvert.SerializeObject(settings, serializerSettings);
                        File.WriteAllText(configPath, settingsjson);
                    }

                    if (forceDebug)
                        settings.DebugMode = true;
                    TournamentXPSettings.SetSettings(settings);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log("Error Loading Settings\n" + ex.ToStringFull());
                return false;
            }
            return true;
        }
    }
}