using BMTournamentPrizes.Models;
using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TournamentLib.Extensions;
using TournamentLib.Models;

namespace BMTournamentPrizes.Behaviors
{
    public class TournamentPrizePoolBehavior : CampaignBehaviorBase
    {
        public TournamentPrizePoolBehavior() { }

        public static TournamentPrizePool GetSettlementPrizePool(string settlementStringId)
        {
            return GetTournamentPrizePool(Campaign.Current.Settlements.Where(x => x.StringId == settlementStringId).Single());
        }
        public static TournamentPrizePool GetTournamentPrizePool(Settlement settlement)
        {
            Town component = settlement.Town;
            TournamentPrizePool obj = MBObjectManager.Instance.GetObject<TournamentPrizePool>((TournamentPrizePool x) => x.Town == component);
            if (obj == null)
            {
                obj = MBObjectManager.Instance.CreateObject<TournamentPrizePool>();
                obj.Town = component;
            }
            return obj;
        }

        private void OnAfterNewGameCreated(CampaignGameStarter starter)
        {
            var text = new TextObject("Re-roll Prize"); //Was going to put the remaining count, but not updating correctly.
            starter.AddGameMenuOption("town_arena", "bm_reroll_price", text.ToString(), 
                new GameMenuOption.OnConditionDelegate(RerollCondition), 
                new GameMenuOption.OnConsequenceDelegate(RerollConsequence),                
                false, 2, false);          
        }


        private static bool RerollCondition(MenuCallbackArgs args)
        {
            if (!TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled)
            {
                return false;
            }

            var tournamentPrizeExpansion = BMTournamentPrizesMain.TournamentPrizeExpansionModel;
            bool flag;
            TextObject textObject;
            TournamentPrizePool settings = tournamentPrizeExpansion.GetPrizesSettingsForSettlement(Settlement.CurrentSettlement.StringId);
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);

            if (settings.RemainingRerolls <= 0)
            {
                flag = true;
                flag1 = false;
                textObject = new TextObject("Re-roll Attempts Exceeded");
            }
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
        }
        public static void RerollConsequence(MenuCallbackArgs args)
        {
            try
            {
                TournamentPrizePool settings = BMTournamentPrizesMain.TournamentPrizeExpansionModel.GetPrizesSettingsForSettlement(Settlement.CurrentSettlement.StringId);
                TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);

                //Clear old prizes
                settings.SelectedPrizeStringId = "";
                settings.Prizes = new ItemRoster();
                settings.RemainingRerolls--;
                BMTournamentPrizesMain.TournamentPrizeExpansionModel.UpdatePrizeSettings(Settlement.CurrentSettlement.StringId, settings);

                //Generate New Prize
                var prize = BMTournamentPrizesMain.TournamentPrizeExpansionModel.GenerateTournamentPrize(tournamentGame);
                BMTournamentPrizesMain.TournamentPrizeExpansionModel.SetTournamentSelectedPrize(tournamentGame, prize);


                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: Re-roll: Refreshing Arena Menu:");
                    FileLog.Log(ex.ToStringFull());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tournament XPerience", "An error was detected re-rolling your prize pool.\nPlease zip up your save game (Documents folder)\nError log (harmony.log.txt on desktop)\nYour config file in BMTournamentXP\\ModuleData\nPut on google drive and message me link on Nexus.");
                FileLog.Log("ERROR: BMTournamentXP: Re-roll Prize Pool");
                FileLog.Log(ex.ToStringFull());
            }
        }



        public override void RegisterEvents()
        {
            throw new NotImplementedException();
        }

        public override void SyncData(IDataStore dataStore)
        {
            throw new NotImplementedException();
        }
    }
}
