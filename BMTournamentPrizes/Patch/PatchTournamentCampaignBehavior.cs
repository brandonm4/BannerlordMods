using BMTournamentPrizes.Models;
using HarmonyLib;

using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TournamentLib.Extensions;
using TournamentLib.Models;

namespace BMTournamentXP
{



    [HarmonyPatch(typeof(TournamentCampaignBehavior), "AddGameMenus")]
    public class TournamentCampaignBehaviorPatch1
    {
        private static bool Prefix(ref TournamentCampaignBehavior __instance)
        {

            return true;
        }

        private static void Postfix(TournamentCampaignBehavior __instance, CampaignGameStarter campaignGameSystemStarter)
        {
            var text = new TextObject("Re-roll Prize");
            campaignGameSystemStarter.AddGameMenuOption("town_arena", "bm_reroll_price", text.ToString(), new GameMenuOption.OnConditionDelegate(TournamentCampaignBehaviorPatch1.game_menu_reroll_tournament_reward_option), (MenuCallbackArgs args) => TournamentCampaignBehaviorPatch1.game_menu_reroll_tournament_reward(__instance), false, 2, false);
        }

        private static bool Prepare()
        {
            return TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled;
        }

        public static bool game_menu_reroll_tournament_reward_option(MenuCallbackArgs args)
        {
            if (!TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled)
            {
                return false;
            }

            bool flag;
            TextObject textObject;
            TournamentPrizeSettings settings = TournamentPrizeExpansion.Instance.GetPrizesSettingsForSettlement(Settlement.CurrentSettlement.StringId);
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);

            if (settings.RemainingRerolls <= 0)
            {
                flag = true;
                flag1 = false;
                textObject = new TextObject("Re-roll Attempts Exceeded");
            }
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
            //return false;
        }

        public static void game_menu_reroll_tournament_reward(TournamentCampaignBehavior campaignBehavior)
        {
            TournamentPrizeSettings settings = TournamentPrizeExpansion.Instance.GetPrizesSettingsForSettlement(Settlement.CurrentSettlement.StringId);
            TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
            ItemObject prize = (ItemObject)Traverse.Create(tournamentGame).Method("GetTournamentPrize").GetValue();
            TournamentPrizeExpansion.SetTournamentSelectedPrize(tournamentGame, prize);

            settings.RemainingRerolls--;
            TournamentPrizeExpansion.Instance.UpdatePrizeSettings(Settlement.CurrentSettlement.StringId, settings);

            try
            {
                GameMenu.SwitchToMenu("town_arena");
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                FileLog.Log(ex.ToStringFull());
            }
        }
    }


    [HarmonyPatch(typeof(TournamentCampaignBehavior), "AddGameMenus")]
    public class TournamentCampaignBehaviorPatchPrizeSelection
    {
        private static void Postfix(TournamentCampaignBehavior __instance, CampaignGameStarter campaignGameSystemStarter)
        {
            campaignGameSystemStarter.AddGameMenuOption("town_arena", "bm_select_prize", "Select Prize", new GameMenuOption.OnConditionDelegate(TournamentCampaignBehaviorPatchPrizeSelection.game_menu_select_tournament_reward_option), (MenuCallbackArgs args) => TournamentCampaignBehaviorPatchPrizeSelection.game_menu_select_tournament_reward(__instance), false, 3, false);
        }

        public static bool game_menu_select_tournament_reward_option(MenuCallbackArgs args)
        {
            if (!TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection)
            {
                return false;
            }
            bool flag;
            TextObject textObject;
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
        }

        public static void game_menu_select_tournament_reward(TournamentCampaignBehavior campaignBehavior)
        {
            List<InquiryElement> prizeElements = new List<InquiryElement>();
            TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
            TournamentPrizeSettings prizeSettings = TournamentPrizeExpansion.Instance.GetPrizesSettingsForSettlement(Settlement.CurrentSettlement.StringId);

            if (prizeSettings.Items.Count < TournamentConfiguration.Instance.PrizeConfiguration.NumberOfPrizeOptions)
            {
                ItemObject prize = TournamentPrizeExpansion.GenerateTournamentPrize(tournamentGame, prizeSettings.Items);
            }
            prizeSettings = TournamentPrizeExpansion.Instance.GetPrizesSettingsForSettlement(Settlement.CurrentSettlement.StringId);
            foreach (var p in prizeSettings.Items)
            {
                var ii = new ImageIdentifier(p.StringId, ImageIdentifierType.Item, p.Name.ToString());


                prizeElements.Add(new InquiryElement(p.StringId, p.Name.ToString(), ii, true, ""));
            }
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                    "Tournament Prize Selection", "You can choose an item from the list below as your reward if you win the tournament!", prizeElements, false, true, "OK", "Cancel",
                    new Action<List<InquiryElement>>(OnSelectPrize), new Action<List<InquiryElement>>(OnDeSelectPrize)), true);
            try
            {
                GameMenu.SwitchToMenu("town_arena");
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                FileLog.Log(ex.ToStringFull());
            }
        }

        static void OnSelectPrize(List<InquiryElement> prizeSelections)
        {
            if (prizeSelections.Count > 0)
            {
                TournamentPrizeExpansion.Instance.SetTournamentSelectedPrize(Settlement.CurrentSettlement.StringId, prizeSelections.First().Identifier.ToString());

                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                    FileLog.Log(ex.ToStringFull());
                }
            }
        }
        static void OnDeSelectPrize(List<InquiryElement> prizeSelections)
        {

        }


        public static bool Prepare()
        {
            return TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection;
        }
    }


    [HarmonyPatch(typeof(TournamentCampaignBehavior), "AddGameMenus")]
    public class TournamentCampaignBehaviorPatch2
    {
        private static void Postfix(TournamentCampaignBehavior __instance, CampaignGameStarter campaignGameSystemStarter)
        {
            var text = new TextObject("Select Tournament Type");
            campaignGameSystemStarter.AddGameMenuOption("town_arena", "bm_reroll_price", text.ToString(), new GameMenuOption.OnConditionDelegate(TournamentCampaignBehaviorPatch2.game_menu_reroll_tournament_select_option), (MenuCallbackArgs args) => TournamentCampaignBehaviorPatch2.game_menu_select_tournament_type(__instance), false, 2, false);
        }

        public static bool game_menu_reroll_tournament_select_option(MenuCallbackArgs args)
        {
            if (!TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection)
            {
                return false;
            }
            bool flag;
            TextObject textObject;
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
            //return false;
        }

        public static void game_menu_select_tournament_type(TournamentCampaignBehavior campaignBehavior)
        {
            List<InquiryElement> tournamentTypeElements = new List<InquiryElement>();
            tournamentTypeElements.Add(new InquiryElement("melee", "Melee Tournament", new ImageIdentifier("battania_noble_sword_2_t5", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("archery", "Archery Tournament", new ImageIdentifier("training_longbow", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("joust", "Jousting Tournament", new ImageIdentifier("khuzait_lance_3_t5", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("race", "Horse Racing Tournament", new ImageIdentifier("desert_war_horse", ImageIdentifierType.Item)));

            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                    "Tournament Type Selection", "What kind of Tournament would you like to compete in today?", tournamentTypeElements, true, true, "OK", "Cancel",
                    new Action<List<InquiryElement>>(OnSelectPrize), new Action<List<InquiryElement>>(OnDeSelectPrize)), true);
            try
            {
                GameMenu.SwitchToMenu("town_arena");
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                FileLog.Log(ex.ToStringFull());
            }

        }

        static void OnSelectPrize(List<InquiryElement> selectedTypes)
        {
            if (selectedTypes.Count > 0)
            {
                var town = Settlement.CurrentSettlement.Town;
                TournamentManager tournamentManager = Campaign.Current.TournamentManager as TournamentManager;
                TournamentGame tournamentGame = tournamentManager.GetTournamentGame(town);
                //this._activeTournaments.Remove(tournamentGame);
                ((List<TournamentGame>)Traverse.Create(tournamentManager).Field("_activeTournaments").GetValue()).Remove(tournamentGame);

                switch (selectedTypes.First().Identifier.ToString())
                {
                    case "melee":
                        tournamentGame = new FightTournamentGame(town);
                        break;
                    case "archery":
                        tournamentGame = new ArcheryTournamentGame(town);
                        break;
                    case "joust":
                        tournamentGame = new JoustingTournamentGame(town);
                        break;
                    case "race":
                        tournamentGame = new HorseRaceTournamentGame(town);
                        break;
                }

                tournamentManager.AddTournament(tournamentGame);
                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                    FileLog.Log(ex.ToStringFull());
                }
            }
        }
        static void OnDeSelectPrize(List<InquiryElement> prizeSelections)
        {

        }

        private static bool Prepare()
        {
#if DEBUG
    //        return true;
#endif
            // return TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled;
            return false;  //currently joining anything but melee crashes game.
        }

    }

}