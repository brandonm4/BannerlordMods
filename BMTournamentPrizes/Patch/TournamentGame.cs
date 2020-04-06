using BMTournamentPrize.Models;
using BMTournamentPrizes.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using Helpers;

namespace BMTournamentPrizes.Patch
{
    [HarmonyPatch(typeof(TournamentGame), "GetTournamentPrize")]
    public class GetTournamentPrizePatch1
    {

        public static bool Prefix(TournamentGame __instance, ref ItemObject __result)
        {
            var prizeSettings = new TournamentPrizeSettings();
            

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
            while (pickeditems.Count < numItemsToGet && allitems.Count > 0)
            {
                var randomId = allitems.GetRandomElement<string>();
                if (!pickeditems.Contains(randomId))
                {
                    pickeditems.Add(randomId);
                    allitems.Remove(randomId);
                }
            }

            foreach (var id in pickeditems)
            {
                prizeSettings.Items.Add(Game.Current.ObjectManager.GetObject<ItemObject>(id));                
            }
            prizeSettings.itemid = prizeSettings.Items[MBRandom.RandomInt(prizeSettings.Items.Count - 1)].StringId;


            __result = prizeSettings.Items.Where(x => x.StringId == prizeSettings.itemid).Single();

            if (TournamentPrizeExpansion.Instance.SettlementPrizes.Keys.Contains(__instance.Town.Settlement.StringId))
            {
                TournamentPrizeExpansion.Instance.SettlementPrizes[__instance.Town.Settlement.StringId] = prizeSettings;
            }
            else
            {
                TournamentPrizeExpansion.Instance.SettlementPrizes.Add(__instance.Town.Settlement.StringId, prizeSettings);
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
              ).Select(x => x.EquipmentElement.Item.StringId).ToList();
            return list;
        }
    }
}
