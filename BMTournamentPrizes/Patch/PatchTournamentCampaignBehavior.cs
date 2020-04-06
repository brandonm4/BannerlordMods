using BMTournamentPrize.Models;
using BMTournamentPrizes.Models;
using HarmonyLib;

using Helpers;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BMTournamentXP
{



    [HarmonyPatch(typeof(TournamentCampaignBehavior), "AddGameMenus")]
    public class TournamentCampaignBehaviorPatch1
    {
        private static void Postfix(TournamentCampaignBehavior __instance, CampaignGameStarter campaignGameSystemStarter)
        {
            //        typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + bonusmoney);

            campaignGameSystemStarter.AddGameMenuOption("town_arena", "bm_reroll_price", "{=LN09ZLXZ}Reroll Prize", new GameMenuOption.OnConditionDelegate(TournamentCampaignBehaviorPatch1.game_menu_reroll_tournament_reward_option), (MenuCallbackArgs args) => TournamentCampaignBehaviorPatch1.game_menu_reroll_tournament_reward(__instance), false, 2, false);
        }

        private static bool Prepare()
        {
            return BMTournamentPrizeConfiguration.Instance.TournamentPrizeRerollEnabled;
        }

        public static bool game_menu_reroll_tournament_reward_option(MenuCallbackArgs args)
        {
            if (!BMTournamentPrizeConfiguration.Instance.TournamentPrizeRerollEnabled)
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

        public static void game_menu_reroll_tournament_reward(TournamentCampaignBehavior campaignBehavior)
        {
            TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
            // tournamentGame.Prize = TournamentCampaignBehaviorPatch1.GetTournamentPrize();
            ItemObject prize = (ItemObject)Traverse.Create(tournamentGame).Method("GetTournamentPrize").GetValue();
            TournamentPrizeExpansion.SetTournamentSelectedPrize(tournamentGame, prize);
            try
            {
                GameMenu.SwitchToMenu("town_arena");
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                FileLog.Log(ex.Message.ToString());
            }
        }


        //private static ItemObject GetTournamentPrize()
        //{
        //    string[] strArray = new String[] { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
        //    return Game.Current.ObjectManager.GetObject<ItemObject>(strArray.GetRandomElement<string>());
        //}
    }


    [HarmonyPatch(typeof(TournamentCampaignBehavior), "AddGameMenus")]
    public class TournamentCampaignBehaviorPatchPrizeSelection
    {
        private static void Postfix(TournamentCampaignBehavior __instance, CampaignGameStarter campaignGameSystemStarter)
        {
            //        typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + bonusmoney);

            campaignGameSystemStarter.AddGameMenuOption("town_arena", "bm_select_prize", "{=LN09ZLXZ}Select Prize", new GameMenuOption.OnConditionDelegate(TournamentCampaignBehaviorPatchPrizeSelection.game_menu_select_tournament_reward_option), (MenuCallbackArgs args) => TournamentCampaignBehaviorPatchPrizeSelection.game_menu_select_tournament_reward(__instance), false, 3, false);
        }

        public static bool game_menu_select_tournament_reward_option(MenuCallbackArgs args)
        {
            if (!BMTournamentPrizeConfiguration.Instance.TournamentPrizeRerollEnabled)
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

        public static void game_menu_select_tournament_reward(TournamentCampaignBehavior campaignBehavior)
        {
            TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
            // tournamentGame.Prize = TournamentCampaignBehaviorPatch1.GetTournamentPrize();
            // ItemObject prize = (ItemObject)Traverse.Create(tournamentGame).Method("GetTournamentPrize").GetValue();
            // TournamentPrizeExpansion.SetTournamentSelectedPrize(tournamentGame, prize);
            //InformationManager.ShowInquiry(new InquiryData("Mercenary Contract Expired", "Your mercenary contract has expired. You may choose to renew the contract for an additional 30 days or end the contract and surrender any fiefs.", true, true, "Renew", "Expire", new Action(this.RenewMercContract), new Action(this.EndMercContract), ""), true);
            List<InquiryElement> prizeElements = new List<InquiryElement>();
            TournamentPrizeSettings prizeSettings;

            if (TournamentPrizeExpansion.Instance.SettlementPrizes.TryGetValue(Settlement.CurrentSettlement.StringId, out prizeSettings))
            {
                foreach (var p in prizeSettings.Items)
                {


                    prizeElements.Add(new InquiryElement(p.StringId, p.Name.ToString(), new TaleWorlds.Core.ImageIdentifier(p), true, ""));
                }


                InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                        "Tournament Prize Selection", "You can choose from the following set of prizes to win from the tournament!", prizeElements, false, true, "OK", "Cancel",
                        new Action<List<InquiryElement>>(OnSelectPrize), new Action<List<InquiryElement>>(OnDeSelectPrize)), true);
                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                    FileLog.Log(ex.Message.ToString());
                }
            }
        }

        static void OnSelectPrize(List<InquiryElement> prizeSelections)
        {
            if (prizeSelections.Count > 0)
            {
                TournamentPrizeExpansion.SetTournamentSelectedPrize(Settlement.CurrentSettlement.StringId, prizeSelections.First().Identifier.ToString());

                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                    FileLog.Log(ex.Message.ToString());
                }
            }
        }
        static void OnDeSelectPrize(List<InquiryElement> prizeSelections)
        {

        }


        public static bool Prepare()
        {
            return BMTournamentPrizeConfiguration.Instance.EnablePrizeSelection;
        }
    }
}