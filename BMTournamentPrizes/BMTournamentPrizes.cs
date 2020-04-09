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
using TournamentLib;
using TournamentLib.Models;

namespace BMTournamentPrizes
{
    public class BMTournamentPrizesMain : BMSubModuleBase
    {
      
        public static TournamentPrizeExpansion TournamentPrizeExpansionModel
        {
            get
            {
                return (TournamentPrizeExpansion)Campaign.Current.Models.GetGameModels().Where(x => x.GetType() == typeof(TournamentPrizeExpansion)).FirstOrDefault();                
            }
        }
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            try
            {
                var h = new Harmony("com.darkspyre.bannerlord.tournamentprizes");
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
                MessageBox.Show(string.Concat("Tournament XP Prizes Error patching:\n", str, " \n\n", message));
            }

        }
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            ShowMessage("Tournament XPerience Prize Module Loaded");

        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {

            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (game.GameType is Campaign)
            {
                gameStarterObject.AddModel(new TournamentPrizeExpansion());              
            }
        }
        public override void OnGameInitializationFinished(Game game)
        {
            Campaign gameType = game.GameType as Campaign;
            if (gameType != null)
            {
                TournamentPoolInitialization(gameType);
            }
        }


        private void TournamentPoolInitialization(Campaign campaign)
        {

            var tournamentPrizeExpansion = BMTournamentPrizesMain.TournamentPrizeExpansionModel;
            if (tournamentPrizeExpansion != null)
            {
                tournamentPrizeExpansion.ClearAllTournamentPrizes();

                if (TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell)
                {
                    List<string> tourneyItems = new List<string>();
                    List<string> problemids = new List<string>();
                    foreach (var id in TournamentConfiguration.Instance.PrizeConfiguration.CustomTourneyItems)
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
                        TournamentConfiguration.Instance.PrizeConfiguration.CustomTourneyItems = tourneyItems;
                    }
                    else
                    {
                        MessageBox.Show("Tournament Prize System", "Tournament Item Restrictions to narrow.  Reverting to unfiltered list.");
                    }
                }

                /* Do some error handling */
                foreach (var settlement in Campaign.Current.Settlements)
                {
                    if (settlement.Town != null && settlement.HasTournament)
                    {
                        var tournamenGame = Campaign.Current.TournamentManager.GetTournamentGame(settlement.Town);

                        if (tournamenGame.Prize == null)
                        {
                            var prize = tournamentPrizeExpansion.GenerateTournamentPrize(tournamenGame);
                            tournamentPrizeExpansion.SetTournamentSelectedPrize(tournamenGame, prize);
                        }
                    }

                }
            }
        }
    }
}
