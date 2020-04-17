using ModLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentsXPanded.Models;
using XPanded.Common.Diagnostics;

namespace TournamentsXPanded.Settings
{
    public class SettingsHelperModLib
    {
        public static bool GetModLibSettings()
        {
            try
            {
                FileDatabase.Initialise(SettingsHelper.ModuleFolderName);
                TournamentXPSettingsModLib settings = FileDatabase.Get<TournamentXPSettingsModLib>(TournamentXPSettingsModLib.InstanceID);
                if (settings == null) settings = new TournamentXPSettingsModLib();
                SettingsDatabase.RegisterSettings(settings);            
                TournamentXPSettings.SetSettings(settings.GetSettings());
            }
            catch (Exception ex)
            {
                ErrorLog.Log("TournamentsXPanded failed to initialize settings data.\n\n" + ex.ToStringFull());
                return false;
            }
            return true;
        }
    }
}
