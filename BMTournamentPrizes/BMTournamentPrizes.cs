using BMTournamentPrize.Models;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BMTournamentPrizes
{
    public class BMTournamentPrizesMain : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            string appSettings = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/BMTournament.config.xml");

            if (File.Exists(appSettings))
            {
                //Configuration = new BMTournamentXPConfiguration(appSettings);                
                BMTournamentPrizeConfiguration.Instance.LoadXML(appSettings);
            }
            ////Load tournament items
            string tourneyitemsfile = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/", BMTournamentPrizeConfiguration.Instance.CustomPrizeFileName);
            if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim().IndexOf("custom", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (File.Exists(tourneyitemsfile))
                {
                    var configtxt = File.ReadAllText(tourneyitemsfile);
                    BMTournamentPrizeConfiguration.Instance.TourneyItems = JsonConvert.DeserializeObject<List<string>>(configtxt);
                }
            }
            try
            {
                var h = new Harmony("com.darkspyre.bannerlord.tournament");
                h.PatchAll();
            }
            catch (Exception exception1)
            {
                string message;
                Exception exception = exception1;
                string str = exception.Message;
                Exception innerException = exception.InnerException;
                if (innerException != null)
                {
                    message = innerException.Message;
                }
                else
                {
                    message = null;
                }
                MessageBox.Show(string.Concat("Error patching:\n", str, " \n\n", message));
            }

        }
    }
}
