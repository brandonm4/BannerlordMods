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
        public  static void ShowMessage(string msg, Color? color =null)
        {
            if (color == null)
                color = Color.White;

            InformationManager.DisplayMessage(new InformationMessage(msg,(Color)color));
        }

        protected override void OnSubModuleLoad()
        {
            string appSettings = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/BMTournament.config.xml");

            if (File.Exists(appSettings))
            {                        
                TournamentConfiguration.Instance.LoadXML(appSettings);
            }
        }
    }
}
