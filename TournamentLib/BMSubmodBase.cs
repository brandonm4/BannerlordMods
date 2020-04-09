using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TournamentLib.Models;

namespace TournamentLib
{
    public class BMSubModuleBase : MBSubModuleBase
    {
        public static string Version { get { return TournamentConfiguration.TournamentXPVersion; } }

        public  static void ShowMessage(string msg)
        {
            InformationManager.DisplayMessage(new InformationMessage(msg));
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            string appSettings = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/BMTournament.config.xml");

            if (File.Exists(appSettings))
            {
                //Configuration = new BMTournamentXPConfiguration(appSettings);                
                TournamentConfiguration.Instance.LoadXML(appSettings);
            }
        }
    }
}
