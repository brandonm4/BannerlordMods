using HarmonyLib;

using Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using TournamentsXPanded.Common;
using TournamentsXPanded.Models;

using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Behaviors
{
    public partial class TournamentPrizePoolBehavior : CampaignBehaviorBase
    {
        public static ItemObject GenerateTournamentPrize(TournamentGame tournamentGame, TournamentPrizePool currentPool = null, bool keepTownPrize = true)
        {
            var numItemsToGet = TournamentXPSettings.Instance.NumberOfPrizeOptions;
            bool bRegenAllPrizes = false;
            if (currentPool == null)
            {
                bRegenAllPrizes = true;
                currentPool = GetTournamentPrizePool(tournamentGame.Town.Settlement);
            }
            var allitems = GetItemStringsRevised(tournamentGame, TournamentPrizePoolBehavior.GetActivePrizeTypes());

            if (allitems.Count == 0)
            {
                MessageBox.Show("TournamentsXPanded Error:\nAlert, your prize generation filters have resulted in no valid prizes.  Consider widening your prize value range and if you are using a custom list make sure it's loaded correctly.");
                return null;
            }

            //Add any existing items if we are filling in missing ones from an already generated pool
            var pickeditems = new List<string>();
            if (keepTownPrize && !string.IsNullOrWhiteSpace((tournamentGame.Prize.StringId)))
            {
                pickeditems.Add(tournamentGame.Prize.StringId);
                currentPool.SelectedPrizeStringId = tournamentGame.Prize.StringId;
            }
            try
            {
                if (!bRegenAllPrizes)
                {
                    foreach (ItemRosterElement existingPrize in currentPool.Prizes)
                    {
                        if (!pickeditems.Contains(existingPrize.EquipmentElement.Item.StringId))
                        {
                            pickeditems.Add(existingPrize.EquipmentElement.Item.StringId);
                        }

                        if (allitems.Contains(existingPrize.EquipmentElement.Item.StringId))
                        {
                            allitems.Remove(existingPrize.EquipmentElement.Item.StringId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log("ERROR: GetTournamentPrize existingprizes\n" + ex.ToStringFull());
            }

            //If the totoal pool of unique items is less than our desired number, reduce our pool size.
            if (allitems.Count() < numItemsToGet)
            {
                numItemsToGet = allitems.Count();
            }

            while (pickeditems.Count < numItemsToGet && allitems.Count() > 0)
            {
                var randomId = allitems.GetRandomElement<string>();

                if (!pickeditems.Contains(randomId))
                {
                    pickeditems.Add(randomId);
                    allitems.Remove(randomId);
                }
            }
            currentPool.Prizes = new ItemRoster();
            foreach (var id in pickeditems)
            {

                ItemModifier itemModifier = null;
                ItemObject pickedPrize = null;
                try
                {                    
                    pickedPrize = Game.Current.ObjectManager.GetObject<ItemObject>(id);
                }
                catch(Exception ex)
                {
                    ErrorLog.Log("Error getting object StringId: " + id + "\n" + ex.ToStringFull());
                }

                if (pickedPrize != null)
                {
#if VERSION120 || VERSION130
                    if (TournamentXPSettings.Instance.EnableItemModifiersForPrizes && pickedPrize.ItemType != ItemObject.ItemTypeEnum.Thrown && pickedPrize.ItemType != ItemObject.ItemTypeEnum.Arrows)
                    {
                        try
                        {
                            if (MBRandom.RandomFloatRanged(100f) < 50f)
                            {
                                var ee = GetEquipmentWithModifier(pickedPrize, TournamentPrizePoolBehavior.GetProsperityModifier(tournamentGame.Town.Settlement));
                                itemModifier = ee.ItemModifier;
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Log("Error in GetEquipmentWithModifier\nItem:" + pickedPrize.StringId + "\n" + ex.ToStringFull());
                        }

                    }
#endif
                    try
                    {
                        currentPool.Prizes.Add(new ItemRosterElement(pickedPrize, 1, itemModifier));
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("Error adding equipment to prizepool.\n" + ex.ToStringFull());
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Item detected.  Please remove: " + id + " from your list of custom items. Ignoring this item and continuing.");
                }
            }

            if (!keepTownPrize)
            {
                var selected = currentPool.Prizes.GetRandomElement<ItemRosterElement>();
                currentPool.SelectedPrizeStringId = selected.EquipmentElement.Item.StringId;
                SetTournamentSelectedPrize(tournamentGame, selected.EquipmentElement.Item);
            }
            return currentPool.SelectPrizeItemRosterElement.EquipmentElement.Item;

        }
        public static List<string> GetItemStringsRevised(TournamentGame tournamentGame, List<ItemObject.ItemTypeEnum> validTypes)
        {
            int prizeMin = MathF.Floor(TournamentPrizePoolBehavior.GetMinPrizeValue() * .7f);
            int prizeMax = MathF.Ceiling(TournamentPrizePoolBehavior.GetMaxPrizeValue() * 1.5f);

            List<string> allitems = new List<string>();

            if (TournamentXPSettings.Instance.PrizeListIncludeLegacy)
            {
                var legacyItems = _legacyItems.AsEnumerable();
                if (TournamentXPSettings.Instance.PrizeFilterCultureLegacyItems)
                {
                    legacyItems = legacyItems.Where(x => x.Culture == tournamentGame.Town.Culture);
                }
                if (TournamentXPSettings.Instance.PrizeFilterValueLegacyItems)
                {
                    legacyItems = legacyItems.Where(x => x.Value >= prizeMin && x.Value <= prizeMax);
                }
                if (TournamentXPSettings.Instance.PrizeFilterItemTypesLegacyItems)
                {
                    legacyItems = legacyItems.Where(x => validTypes.Contains(x.ItemType));
                }
                allitems = allitems.Concat(legacyItems.Select(x => x.StringId)).ToList();
            }
            if (TournamentXPSettings.Instance.PrizeListIncludeCustom && TournamentPrizePoolBehavior.CustomTournamentItems != null && TournamentPrizePoolBehavior.CustomTournamentItems.Count > 0)
            {
                try
                {
                    var customItems = _customItems.AsEnumerable();
                    if (TournamentXPSettings.Instance.PrizeFilterCultureCustomItems)
                    {
                        customItems = customItems.Where(x => x.Culture == tournamentGame.Town.Culture);
                    }
                    if (TournamentXPSettings.Instance.PrizeFilterValueCustomItems)
                    {
                        customItems = customItems.Where(x => x.Value >= prizeMin && x.Value <= prizeMax);
                    }
                    if (TournamentXPSettings.Instance.PrizeFilterItemTypesCustomItems)
                    {
                        customItems = customItems.Where(x => validTypes.Contains(x.ItemType));
                    }

                    allitems = allitems.Concat(customItems.Select(x => x.StringId).ToList()).ToList();
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("Error adding custom items to prize pool.");
                    ErrorLog.Log(ex.ToStringFull());
                }
            }
            if (TournamentXPSettings.Instance.PrizeListIncludeTown)
            {
                var roster = tournamentGame.Town.Owner.ItemRoster;
                roster.RemoveZeroCounts();
                var townItems = roster.Where(x => x.Amount > 0 && !x.EquipmentElement.Item.NotMerchandise).Select(x => x.EquipmentElement.Item);

                if (TournamentXPSettings.Instance.PrizeFilterCultureTownItems)
                {
                    townItems = townItems.Where(x => x.Culture == tournamentGame.Town.Culture);
                }
                if (TournamentXPSettings.Instance.PrizeFilterValueTownItems)
                {
                    townItems = townItems.Where(x => x.Value >= prizeMin && x.Value <= prizeMax);
                }
                if (TournamentXPSettings.Instance.PrizeFilterItemTypesTownItems)
                {
                    townItems = townItems.Where(x => validTypes.Contains(x.ItemType));
                }

                allitems = allitems.Concat(townItems.Select(x => x.StringId)).ToList();
            }
            if (TournamentXPSettings.Instance.PrizeListIncludeVanilla)
            {
                var vanillaItems = ItemObject.All.AsEnumerable();
                if (TournamentXPSettings.Instance.PrizeFilterCultureStandardItems)
                {
                    vanillaItems = vanillaItems.Where(x => x.Culture == tournamentGame.Town.Culture);
                }
                if (TournamentXPSettings.Instance.PrizeFilterValueStandardItems)
                {
                    vanillaItems = vanillaItems.Where(x => x.Value >= prizeMin && x.Value <= prizeMax);
                }
                if (TournamentXPSettings.Instance.PrizeFilterItemTypesStandardItems)
                {
                    vanillaItems = vanillaItems.Where(x => validTypes.Contains(x.ItemType));
                }									
                allitems = allitems.Concat(vanillaItems.Select( x=> x.StringId)).ToList();
            }
            
			if (allitems.Count == 0)
            {
                //Alert - fix it somehow
                MessageBox.Show("TournamentXPanded Error:\nYour filters are too strict, no items are found to populate the tournaments with. Check your settings to allow for a wider choice.  Generally, this can only occur if you've set the lists to only allow for custom items, and those items are not loaded correctly.\nYou can enable debug mode to view additional diagnostics, to help determine if you items are loading or not.");
                ErrorLog.Log("Error populating Tournament Prizes\n");
            }

            return allitems;
        }



    }
}
