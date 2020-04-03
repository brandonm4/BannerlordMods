﻿using HarmonyLib;
using Helpers;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BMTournamentXP
{


    [HarmonyPatch(typeof(TournamentCampaignBehavior), "AddGameMenus")]
    public class TournamentCampaignBehaviorPatch1
    {
        public static bool IsEnabled { get; set; } = true;
        public TournamentCampaignBehaviorPatch1()
        {

        }
        private static void Postfix(TournamentCampaignBehavior __instance, CampaignGameStarter campaignGameSystemStarter)
        {
            //        typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + bonusmoney);

            campaignGameSystemStarter.AddGameMenuOption("town_arena", "bm_reroll_price", "{=LN09ZLXZ}Reroll Prize", new GameMenuOption.OnConditionDelegate(TournamentCampaignBehaviorPatch1.game_menu_reroll_tournament_reward_option), (MenuCallbackArgs args) => TournamentCampaignBehaviorPatch1.game_menu_reroll_tournament_reward(__instance), false, 2, false);


        }

        public static bool game_menu_reroll_tournament_reward_option(MenuCallbackArgs args)
        {
            if (!TournamentCampaignBehaviorPatch1.IsEnabled)
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
            typeof(TournamentGame).GetProperty("Prize").SetValue(tournamentGame, prize);
        }

        //private static ItemObject GetTournamentPrize()
        //{
        //    string[] strArray = new String[] { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
        //    return Game.Current.ObjectManager.GetObject<ItemObject>(strArray.GetRandomElement<string>());
        //}
    }
}



//class BMTournamentBehavior : TournamentBehavior
//{
//    public BMTournamentBehavior(TournamentGame tournamentGame, Settlement settlement, ITournamentGameBehavior gameBehavior, bool isPlayerParticipating)
//        : base(tournamentGame, settlement, gameBehavior, isPlayerParticipating)
//    {

//    }

//    new public int MaximumBet = 300;


//    new public int MaximumBetInstance
//    {
//        get
//        {
//            return Math.Min(MaximumBet, this.PlayerDenars);
//        }
//    }
//}