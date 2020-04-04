using HarmonyLib;
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

    [HarmonyPatch(typeof(TournamentGame), "GetTournamentPrize")]
    public class GetTournamentPrizePatch1
    {
        public GetTournamentPrizePatch1() { }

        public static bool Prefix(TournamentGame __instance, ref ItemObject __result)
        {

            List<string> townitems = new List<string>();
            if (BMTournamentXPMain.Configuration.PrizeListMode.Trim().IndexOf("town", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                townitems = GetValidTownItems(__instance.Town.Owner.ItemRoster, BMTournamentXPMain.Configuration.TownPrizeMin, BMTournamentXPMain.Configuration.TownPrizeMax, BMTournamentXPMain.Configuration.TownValidPrizeTypes);

                if (BMTournamentXPMain.Configuration.PrizeListMode.Trim().IndexOf("only", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    __result = Game.Current.ObjectManager.GetObject<ItemObject>(townitems.GetRandomElement<string>());                    
                    return false;
                }
            }
            //if we are still here, append the town items if any, to the custom/stock list.
            var allitems = BMTournamentXPMain.Configuration.TourneyItems.Concat(townitems.Take(10)).ToArray();
            __result = Game.Current.ObjectManager.GetObject<ItemObject>(allitems.GetRandomElement<string>());

            return false;
            //List<string> strArray = new List<string>();
            //foreach (var x in BMTournamentXPMain.Configuration.TourneyItems)
            //{
            //    strArray.Add(x);
            //}

            //if (BMTournamentXPMain.Configuration.PrizeListMode.Trim().IndexOf("town", StringComparison.OrdinalIgnoreCase) >= 0)
            //{
            //    //if it's only, remove the stock/custom items
            //    if (BMTournamentXPMain.Configuration.PrizeListMode.Trim().IndexOf("only", StringComparison.OrdinalIgnoreCase) >= 0)
            //    {
            //        strArray = new List<string>();
            //    }
            //    //var itemsRos = Hero.MainHero.CurrentSettlement.ItemRoster
            //    //Settlement.CurrentSettlement.ItemRoster.RemoveZeroCounts();
            //    IMarketData md = (IMarketData)Traverse.Create(InventoryManager.Instance).Method("GetCurrentMarketData").GetValue();

            //    var roster = Settlement.CurrentSettlement.ItemRoster;
            //    roster.RemoveZeroCounts();
            //    var list = roster.Where(x =>
            //    x.Amount > 0
            //    && BMTournamentXPMain.Configuration.TownValidPrizeTypes.Contains(x.EquipmentElement.Item.ItemType)
            //   && x.EquipmentElement.Item.Value >= BMTournamentXPMain.Configuration.TownPrizeMin
            //   && x.EquipmentElement.Item.Value <= BMTournamentXPMain.Configuration.TownPrizeMax
            //      ).OrderByDescending(x => x.EquipmentElement.Item.Value).Select(x => x.EquipmentElement.Item.StringId).ToList();

            //    int count = 0;
            //    foreach (var s in list)
            //    {
            //        strArray.Add(s);
            //        count++;
            //        if (count >= 10)
            //        {
            //            break;
            //        }
            //    }
            //}
            //__result = Game.Current.ObjectManager.GetObject<ItemObject>(strArray.GetRandomElement<string>());

        }

        static bool Prepare()
        {
            if (string.Equals(BMTournamentXPMain.Configuration.PrizeListMode.Trim(), "stock", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        private static List<string> GetValidTownItems(ItemRoster roster, int minValue, int maxValue, List<ItemObject.ItemTypeEnum> validtypes)
        {
            roster.RemoveZeroCounts();
            var list = roster.Where(x =>
            x.Amount > 0
            && validtypes.Contains(x.EquipmentElement.Item.ItemType)
           && x.EquipmentElement.Item.Value >= minValue
           && x.EquipmentElement.Item.Value <= maxValue
              ).OrderByDescending(x => x.EquipmentElement.Item.Value).Select(x => x.EquipmentElement.Item.StringId).ToList();
            return list;
        }

       
    }

}


