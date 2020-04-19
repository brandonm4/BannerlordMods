using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

using TournamentsXPanded.Common.Patches;

using XPanded.Common.Diagnostics;

namespace TournamentsXPanded.Common
{
    public partial class XPandedSubModuleBase : MBSubModuleBase
    {
        public static string ModuleFolderName { get; } = "TournamentsXPanded";
        public static IDictionary<Type, IPatch> ActivePatches = new Dictionary<Type, IPatch>();

        protected override void OnSubModuleLoad()
        {
            //Setup Logging
            if (File.Exists(System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs")))
            {
                File.Delete(System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs"));
            }
            string logpath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs", "logfile.txt");
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(logpath)))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logpath));
            }
            ErrorLog.LogPath = logpath;
        }

        public static void ShowMessage(string msg, Color? color = null)
        {
            if (color == null)
            {
                color = Color.White;
            }

            InformationManager.DisplayMessage(new InformationMessage(msg, (Color)color));
        }
    }
}