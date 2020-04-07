using BMTournamentPrizes.Models;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TournamentLib.Models;

namespace BMTournamentPrizes
{
    public class BMTournamentPrizesMain : MBSubModuleBase
    {
        private static void ShowMessage(string msg)
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
            ////Load tournament items
            string tourneyitemsfile = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/", TournamentConfiguration.Instance.PrizeConfiguration.CustomPrizeFileName);
            if (TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode.Trim().IndexOf("custom", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (File.Exists(tourneyitemsfile))
                {
                    var configtxt = File.ReadAllText(tourneyitemsfile);
                    TournamentConfiguration.Instance.PrizeConfiguration.TourneyItems = JsonConvert.DeserializeObject<List<string>>(configtxt);
                }
            }

            if (TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode.IndexOf("stock", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                TournamentConfiguration.Instance.PrizeConfiguration.TourneyItems = new List<string>();
                TournamentConfiguration.Instance.PrizeConfiguration.TourneyItems = PrizeConfiguration.StockTourneyItems.ToList();
            }
            else if (TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode.IndexOf("townonly", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                TournamentConfiguration.Instance.PrizeConfiguration.TourneyItems = new List<string>();
                TournamentConfiguration.Instance.PrizeConfiguration.TourneyItems = PrizeConfiguration.StockTourneyItems.ToList();
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
            TournamentPrizeExpansion.Instance.ClearAllTournamentPrizes();

            if (TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell)
            {
                List<string> tourneyItems = new List<string>();
                List<string> problemids = new List<string>();
                foreach (var id in TournamentConfiguration.Instance.PrizeConfiguration.TourneyItems)
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

                    if (item.Value >= TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMin && item.Value <= TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMax)
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
                    TournamentConfiguration.Instance.PrizeConfiguration.TourneyItems = tourneyItems;
                }
                else
                {
                    MessageBox.Show("Tournament Prize System", "Tournament Item Restrictions to narrow.  Reverting to unfiltered list.");
                }
            }

            /* Do some error handling */
            foreach(var settlement in Campaign.Current.Settlements)
            {
                if (settlement.Town != null && settlement.HasTournament)
                {
                    var tournamenGame = Campaign.Current.TournamentManager.GetTournamentGame(settlement.Town);

                    if (tournamenGame.Prize == null)
                    {
                        var prize = TournamentPrizeExpansion.GenerateTournamentPrize(tournamenGame);
                        TournamentPrizeExpansion.SetTournamentSelectedPrize(tournamenGame, prize);
                    }
                }
                
            }
        }
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            ShowMessage("Tournament XPerience Prize Module Loaded");

        }




    }
}
