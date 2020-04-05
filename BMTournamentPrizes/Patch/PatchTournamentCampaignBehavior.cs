using BMTournamentPrize.Models;
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

        private static bool Prepare()
        {
            return BMTournamentPrizeConfiguration.Instance.TournamentPrizeRerollEnabled;
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
            SetTournamentSelectedPrize(tournamentGame, prize);
            try
            {
                GameMenu.SwitchToMenu("town_arena");
            }
            catch(Exception ex)
            {
                FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                FileLog.Log(ex.Message.ToString());
            }
        }

        public static void SetTournamentSelectedPrize(TournamentGame tournamentGame, ItemObject prize)
        {
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
        

        public GetTournamentPrizePatch1()
        {
        }

        public static bool Prefix(TournamentGame __instance, ref ItemObject __result)
        {
            List<TournamentPrizeEntry> prizes = new List<TournamentPrizeEntry>();

            List<string> townitems = new List<string>();
            if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim().IndexOf("town", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                townitems = GetValidTownItems(__instance.Town.Owner.ItemRoster, BMTournamentPrizeConfiguration.Instance.TownPrizeMin, BMTournamentPrizeConfiguration.Instance.TownPrizeMax, BMTournamentPrizeConfiguration.Instance.TownValidPrizeTypes);

                if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim().IndexOf("only", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //If no prizes are found use the custom ones
                    if (townitems.Count == 0)
                    {
                        //if somehow the user deleted all the custom items use the vanilla items.
                        townitems = BMTournamentPrizeConfiguration.Instance.TourneyItems;
                        if (townitems.Count == 0)
                        {
                            townitems = BMTournamentPrizeConfiguration.Instance.StockTourneyItems;
                        }
                    }

                    __result = Game.Current.ObjectManager.GetObject<ItemObject>(townitems.GetRandomElement<string>());
                    return false;
                }
            }
            
            //If user somehow manages to select custom but has no items, populate with vanilla
            if (townitems.Count == 0 && BMTournamentPrizeConfiguration.Instance.TourneyItems.Count == 0)
            {
                townitems = BMTournamentPrizeConfiguration.Instance.StockTourneyItems;
            }

            var allitems = townitems.Concat(BMTournamentPrizeConfiguration.Instance.TourneyItems).ToList();

            var numItemsToGet = BMTournamentPrizeConfiguration.Instance.NumberOfPrizeOptions;
            if (allitems.Count() < numItemsToGet)
            {
                numItemsToGet = allitems.Count();
            }

            var pickeditems = new List<string>();
            while(pickeditems.Count < numItemsToGet && allitems.Count > 0)
            {
                var randomId = allitems.GetRandomElement<string>();
                if (!pickeditems.Contains(randomId))
                {
                    pickeditems.Add(randomId);
                    allitems.Remove(randomId);
                }
            }
            
            foreach(var id in pickeditems)
            {
                prizes.Add(new TournamentPrizeEntry
                {
                    IsSelected = false,
                    Item = Game.Current.ObjectManager.GetObject<ItemObject>(id)
                }) ;
            }
            prizes.First().IsSelected = true;


            __result = prizes.Where(x => x.IsSelected == true).Single().Item;
            if (TournamentPrizeExpansion.Instance.SettlementPrizes.Keys.Contains(__instance.Town.Settlement.StringId))
            {
                TournamentPrizeExpansion.Instance.SettlementPrizes[__instance.Town.Settlement.StringId] = prizes;
            }
            else
            {
                TournamentPrizeExpansion.Instance.SettlementPrizes.Add(__instance.Town.Settlement.StringId, prizes);
            }
            return false;           
        }

        private static bool Prepare()
        {
            if (string.Equals(BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim(), "stock", StringComparison.OrdinalIgnoreCase))
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