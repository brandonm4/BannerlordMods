using ModLib;

using System.IO;
using System.Windows.Forms;

using TaleWorlds.Library;

using TournamentsXP.Addon.ModLib;

using TournamentsXPanded.Common;
using TournamentsXPanded.Models;
using TournamentsXPanded.Settings;

namespace TournamentsXP.Addon
{
    public class TournamentsXPAddonModLibSubModule : XPandedSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            var dirpath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName);
            try
            {
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
            }
            catch
            {
                MessageBox.Show("Failed to create config directory.  Please manually create this directory: " + dirpath);
            }
            base.OnSubModuleLoad();

            FileDatabase.Initialise(SettingsHelper.ModuleFolderName);
            TournamentsXPSettingsModLib settings = FileDatabase.Get<TournamentsXPSettingsModLib>(TournamentsXPSettingsModLib.InstanceID) ?? new TournamentsXPSettingsModLib();
            SettingsDatabase.RegisterSettings(settings);
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            string version = typeof(TournamentsXPAddonModLibSubModule).Assembly.GetName().Version.ToString();

            if (!TournamentXPSettings.Instance.DebugMode)
            {
                ShowMessage("Tournaments XPanded ModLib v" + version + " Loaded", Colors.Cyan);
            }
            else
            {
                ShowMessage("Tournaments XPanded ModLib v" + version + " Loaded {DEBUG MODE}", Colors.Red);
            }
        }
    }
}