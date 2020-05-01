
using SandBox;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TournamentsXPanded.Common;
using TournamentsXPanded.Models;
using TournamentsXPanded.Settings;
using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ModLib;
using TournamentsXP.Addon.ModLib;

namespace TournamentsXP.Addon
{
    public class TournamentsXPAddonModLibSubModule : XPandedSubModuleBase
    {
        private static bool disabled = false;
        private static ApplicationVersion versionNative;
        private System.ComponentModel.BackgroundWorker menuChecker1;
        private string _id;
        private bool inMenu;

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
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create config directory.  Please manually create this directory: " + dirpath);
            }
            base.OnSubModuleLoad();

           FileDatabase.Initialise(SettingsHelper.ModuleFolderName);
            TournamentsXPSettingsModLib settings = FileDatabase.Get<TournamentsXPSettingsModLib>(TournamentsXPSettingsModLib.InstanceID);
            if (settings == null)
            {
                settings = new TournamentsXPSettingsModLib();
            }
           SettingsDatabase.RegisterSettings(settings);           
        }
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            string version = typeof(TournamentsXPAddonModLibSubModule).Assembly.GetName().Version.ToString();
            if (!disabled)
            {
                if (!TournamentXPSettings.Instance.DebugMode)
                {
                    ShowMessage("Tournaments XPanded ModLib v" + version + " Loaded", Colors.Cyan);
                }
                else
                {
                    ShowMessage("Tournaments XPanded ModLib v" + version + " Loaded {DEBUG MODE}", Colors.Red);
                }           
            }
            else
            {
                ShowMessage("Tournaments XPanded ModLib v" + version + " Disabled", Colors.Red);
            }
        }


    }
}
