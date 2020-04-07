using BMTournamentPrize.Models;
using BMTournamentPrizes.Models;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
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

            if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.IndexOf("stock", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BMTournamentPrizeConfiguration.Instance.TourneyItems = new List<string>();
                BMTournamentPrizeConfiguration.Instance.TourneyItems = BMTournamentPrizeConfiguration.StockTourneyItems.ToList();
            }
            else if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.IndexOf("townonly", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BMTournamentPrizeConfiguration.Instance.TourneyItems = new List<string>();
                BMTournamentPrizeConfiguration.Instance.TourneyItems = BMTournamentPrizeConfiguration.StockTourneyItems.ToList();
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

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            TournamentPrizeExpansion.Instance.SettlementPrizes = new Dictionary<string, TournamentPrizeSettings>();

            if (BMTournamentPrizeConfiguration.Instance.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell)
            {
                List<string> tourneyItems = new List<string>();
                List<string> problemids = new List<string>();
                foreach (var id in BMTournamentPrizeConfiguration.Instance.TourneyItems)
                {
                    ItemObject item;

                    try
                    {
                        item = Game.Current.ObjectManager.GetObject<ItemObject>(id);
                    }
                    catch 
                    {
                        item = null;
                    }
                    if (item == null || item.ItemType == ItemObject.ItemTypeEnum.Invalid)
                    {
                        problemids.Add(id);
                        // MessageBox.Show("Tournament Prize System", String.Concat("Invalid Item Id detected in prize list.  Please remove from the list.  Ignoring problem item and continuing.\n\n", id));

                        FileLog.Log(String.Concat("WARNING: Tournament Prize System\n", "Invalid Item Id detected in prize list.  Please remove from the list.  Ignoring problem item and continuing.\n\n", id));
                    }

                    if (item.Value >= BMTournamentPrizeConfiguration.Instance.TownPrizeMin && item.Value <= BMTournamentPrizeConfiguration.Instance.TownPrizeMax)
                    {
                        tourneyItems.Add(id);
                    }
                }
                if (problemids.Count > 0)
                { 
                    string info = "Detected Errors in Custom Prize List.  Review list and correct or remove these entries:\n";
                    foreach (var p in problemids)
                    {
                        info = String.Concat(info, p, "\n");
                    }

                    InformationManager.ShowInquiry(new InquiryData("Tournament Prize Errors",
                        info,
                        true, false, "Ok", "No", null, null, ""), false);

                }

                if (tourneyItems.Count > 0)
                {
                    BMTournamentPrizeConfiguration.Instance.TourneyItems = tourneyItems;
                }
                else
                {
                    MessageBox.Show("Tournament Prize System", "Tournament Item Restrictions to narrow.  Reverting to unfiltered list.");
                }
            }
        }
    }
}
