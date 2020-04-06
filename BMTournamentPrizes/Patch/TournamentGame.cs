using BMTournamentPrizes.Extensions;
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
using System.Windows.Forms;

namespace BMTournamentPrizes.Patch
{
    [HarmonyPatch(typeof(TournamentGame), "GetTournamentPrize")]
    public class GetTournamentPrizePatch1
    {

        public static bool Prefix(TournamentGame __instance, ref ItemObject __result)
        {
            try
            {
                __result = GenerateTournamentPrize(__instance);
                if (__result == null)
                {
                    MessageBox.Show("Tournament Prize System", "Error generating Tournament Prize. Reverting to vanilla system.");
                    return true;
                }
            }
            catch(Exception ex)
            {                
                FileLog.Log("ERROR: Tournament Prize System");
                FileLog.Log(ex.ToStringFull());
            }

            return false;
        }

        public static ItemObject GenerateTournamentPrize(TournamentGame tournamentGame, List<ItemObject> existingPrizes = null)
        {
            ItemObject prize;
            TournamentPrizeSettings prizeSettings;// = new TournamentPrizeSettings();
            TournamentPrizeExpansion.Instance.SettlementPrizes.TryGetValue(tournamentGame.Town.Settlement.StringId, out prizeSettings);
            if (prizeSettings == null)
            {
                prizeSettings = new TournamentPrizeSettings();
            }

            List<string> townitems = new List<string>();
            if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim().IndexOf("town", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                townitems = GetValidTownItems(tournamentGame.Town.Owner.ItemRoster, BMTournamentPrizeConfiguration.Instance.TownPrizeMin, BMTournamentPrizeConfiguration.Instance.TownPrizeMax, BMTournamentPrizeConfiguration.Instance.TownValidPrizeTypes);

                if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim().IndexOf("only", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //If no prizes are found use the custom ones
                    if (townitems.Count == 0)
                    {
                        //if somehow the user deleted all the custom items use the vanilla items.
                        townitems = BMTournamentPrizeConfiguration.Instance.TourneyItems;
                        if (townitems.Count == 0)
                        {
                            townitems = BMTournamentPrizeConfiguration.StockTourneyItems;
                        }
                    }

                    prize = Game.Current.ObjectManager.GetObject<ItemObject>(townitems.GetRandomElement<string>());
                    return prize;
                }
            }

            //If user somehow manages to select custom but has no items, populate with vanilla
            if (townitems.Count == 0 && BMTournamentPrizeConfiguration.Instance.TourneyItems.Count == 0)
            {
                townitems = BMTournamentPrizeConfiguration.StockTourneyItems;
            }

            var allitems = townitems.Concat(BMTournamentPrizeConfiguration.Instance.TourneyItems).ToList();

            var numItemsToGet = BMTournamentPrizeConfiguration.Instance.NumberOfPrizeOptions;



            var pickeditems = new List<string>();
            if (existingPrizes != null)
            {
                foreach (var existingPrize in existingPrizes)
                {
                    pickeditems.Add(existingPrize.StringId);
                    if (allitems.Contains(existingPrize.StringId))
                    {
                        allitems.Remove(existingPrize.StringId);
                    }
                }
            }

            if (allitems.Count() < numItemsToGet)
            {
                numItemsToGet = allitems.Count();
            }

            while (pickeditems.Count < numItemsToGet && allitems.Count > 0)
            {
                var randomId = allitems.GetRandomElement<string>();

                if (!pickeditems.Contains(randomId))
                {
                    pickeditems.Add(randomId);
                    allitems.Remove(randomId);
                }
            }
            prizeSettings.Items = new List<ItemObject>();
            foreach (var id in pickeditems)
            {
                prizeSettings.Items.Add(Game.Current.ObjectManager.GetObject<ItemObject>(id));
            }
            prizeSettings.itemid = prizeSettings.Items[MBRandom.RandomInt(prizeSettings.Items.Count - 1)].StringId;
            prize = prizeSettings.Items.Where(x => x.StringId == prizeSettings.itemid).Single();
            TournamentPrizeExpansion.UpdatePrizeSettings(tournamentGame.Town.Settlement.StringId, prizeSettings);
            return prize;

        }

        private static bool Prepare()
        {
            if (BMTournamentPrizeConfiguration.Instance.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell
                || BMTournamentPrizeConfiguration.Instance.EnablePrizeSelection
                || BMTournamentPrizeConfiguration.Instance.TournamentPrizeRerollEnabled
                || BMTournamentPrizeConfiguration.Instance.PrizeListMode.IndexOf("custom", StringComparison.OrdinalIgnoreCase) >= 0
                || BMTournamentPrizeConfiguration.Instance.PrizeListMode.IndexOf("town", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            return false;
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

            if (list.Count == 0)
            {
                list = roster.Where(x =>
                    x.Amount > 0
                    && validtypes.Contains(x.EquipmentElement.Item.ItemType))
                   .Select(x => x.EquipmentElement.Item.StringId).ToList();
            }
            if (list.Count == 0)
            {
                list = BMTournamentPrizeConfiguration.Instance.TourneyItems;
            }

            return list;
        }
    }
}
