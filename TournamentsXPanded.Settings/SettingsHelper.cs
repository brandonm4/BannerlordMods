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
                //                var modnames = Utilities.GetModulesNames().ToList();
                bool modLibLoaded = false;
                //                if (
                //                    (modnames.Contains("ModLib")
                //                  //  || modnames.Contains("Bannerlord.MBOptionScreen")
                //                    ) && !overrideSettings)
                //                {

                Type settingsHelperModLibType = null;
                //                    Assembly modlibassembly = null;
                Assembly modlibSettingsAssembly = null;

                //                    try
                //                    {
                //                        modlibassembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name == "ModLib").FirstOrDefault();

                //                    }
                //                    catch { }

                //                    //try
                //                    //{
                //                    //    if (!AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName().Name).ToList().Contains("ModLib"))
                //                    //    {
                //                    //        var modlibdllpath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetBasePath(), "Modules", "Bannerlord.MBOptionScreen", "bin", "Win64_Shipping_Client", "ModLib.dll");
                //                    //        modlibdllpath = System.IO.Path.GetFullPath(modlibdllpath);
                //                    //        if (File.Exists(modlibdllpath))
                //                    //            modlibassembly = Assembly.LoadFile(modlibdllpath);
                //                    //    }
                //                    //}
                //                    //catch (Exception ex)
                //                    //{
                //                    //    ErrorLog.Log("Error Loading ModLib Assembly\n" + ex.ToStringFull());
                //                    //}

                /* My Own ModLib */
                /*
                try
                {
                    //     if (modlibassembly != null)
                    {
                        var modlibsettingsdll = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetBasePath(), "Modules", ModuleFolderName, "bin", "Win64_Shipping_Client", "TournamentsXPanded.Settings.ModLib.dll");
                        modlibsettingsdll = System.IO.Path.GetFullPath(modlibsettingsdll);
                        modlibSettingsAssembly = Assembly.LoadFile(modlibsettingsdll);
                        settingsHelperModLibType = modlibSettingsAssembly.GetType("TournamentsXPanded.Settings.SettingsHelperModLib");
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("Error Loading SettingsModLib Assembly\n" + ex.ToStringFull());
                }
                */

//                if (settingsHelperModLibType != null)
//                {
//                    try
//                    {
//                        TournamentXPSettings osettings = (TournamentXPSettings)settingsHelperModLibType.GetMethod("GetModLibSettings", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Invoke(null, new object[] { forceDebug });
//                        if (osettings != null)
//                        {
//#if DEBUG
//                            osettings.DebugMode = true;
//#endif
//                            TournamentXPSettings.SetSettings(osettings);
//                            modLibLoaded = true;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        ErrorLog.Log("Error getting ModLib settings\n" + ex.ToStringFull());
//                    }
//                }

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
            catch (Exception ex)
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