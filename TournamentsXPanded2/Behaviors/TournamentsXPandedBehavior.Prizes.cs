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
    public partial class TournamentsXPandedBehavior 
    {
        internal static List<ItemObject> _customItems;
        internal static List<ItemObject> _legacyItems;

        public static ItemObject GenerateTournamentPrize(TournamentGame tournamentGame, TournamentXPandedModel tournamentInfo, bool keepTownPrize = false, bool regenAllPrizes = true)
        {
            if (tournamentGame == null)
                return null;

            var numItemsToGet = TournamentXPSettings.Instance.NumberOfPrizeOptions;                      
            var allitems = GetItemStringsRevised(tournamentGame, TournamentXPSettings.Instance.GetActivePrizeTypes());


            if (allitems.Count == 0)
            {
                MessageBox.Show("TournamentsXPanded Error:\nAlert, your prize generation filters have resulted in no valid prizes.  Consider widening your prize value range and if you are using a custom list make sure it's loaded correctly.");
                return null;
            }

            //Add any existing items if we are filling in missing ones from an already generated pool
            if (regenAllPrizes)
            {
                tournamentInfo.SelectedPrizeStringId = "";
                tournamentInfo.PrizePool = new List<PrizeItem>();
            }
            var pickeditems = new List<PrizeItem>();
            if (keepTownPrize && tournamentGame != null && !string.IsNullOrWhiteSpace((tournamentGame.Prize.StringId)))
            {
                pickeditems.Add(new PrizeItem() { ItemStringId = tournamentGame.Prize.StringId, ItemModifierStringId = "" });
                tournamentInfo.SelectedPrizeStringId = tournamentGame.Prize.StringId;
            }
            try
            {
                if (!regenAllPrizes)
                {
                    foreach (PrizeItem existingPrize in tournamentInfo.PrizePool)
                    {
                        if (!pickeditems.Contains(existingPrize))
                        {
                            pickeditems.Add(existingPrize);
                        }
                        if (allitems.Contains(existingPrize.ItemStringId))
                        {
                            allitems.Remove(existingPrize.ItemStringId);
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
                if (pickeditems.Where( x=> x.ItemStringId == randomId).Count() == 0)
                {
                    pickeditems.Add(new PrizeItem() { ItemStringId = randomId, ItemModifierStringId = "" });
                }
                allitems.Remove(randomId);
            }
            tournamentInfo.PrizePool = new List<PrizeItem>();

            foreach (var pickedItem in pickeditems)
            {
                ItemModifier itemModifier = null;
                ItemObject pickedPrize = null;
                string itemModifierStringId = "";
                try
                {                    
                    pickedPrize = Game.Current.ObjectManager.GetObject<ItemObject>(pickedItem.ItemStringId);
                }
                catch(Exception ex)
                {
                    ErrorLog.Log("Error getting object StringId: " + pickedItem.ItemStringId + "\n" + ex.ToStringFull());
                }

                if (pickedPrize != null)
                {
                    if (TournamentXPSettings.Instance.EnableItemModifiersForPrizes && pickedPrize.ItemType != ItemObject.ItemTypeEnum.Thrown && pickedPrize.ItemType != ItemObject.ItemTypeEnum.Arrows)
                    {
                        try
                        {
                            if (MBRandom.RandomFloatRanged(100f) < 50f)
                            {
                                var ee = GetEquipmentWithModifier(pickedPrize, GetProsperityModifier(tournamentGame.Town.Settlement));
                                itemModifier = ee.ItemModifier;
                                if (itemModifier != null)
                                {
                                    itemModifierStringId = itemModifier.StringId;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Log("Error in GetEquipmentWithModifier\nItem:" + pickedPrize.StringId + "\n" + ex.ToStringFull());
                        }

                    }
                    try
                    {
                        tournamentInfo.PrizePool.Add(new PrizeItem() { ItemStringId = pickedPrize.StringId, ItemModifierStringId = itemModifierStringId });
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("Error adding equipment to prizepool.\n" + ex.ToStringFull());
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Item detected.  Please remove: " + pickedItem.ItemStringId + " from your list of custom items. Ignoring this item and continuing.");
                }
            }

            if (!keepTownPrize)
            {
                if (tournamentInfo.PrizePool.Count > 0)
                {
                    tournamentInfo.SelectedPrizeStringId = tournamentInfo.PrizePool.First().ItemStringId;
                //    SetTournamentSelectedPrize(tournamentGame, tournamentInfo.SelectPrizeItemRosterElement?.EquipmentElement.Item);
                }
            }
            try
            {
                return tournamentInfo.SelectedPrizeItem.ToItemRosterElement().EquipmentElement.Item;
            }
            catch {
                //something went wrong generating a valid prize somewhere up above. hopefully earlier try catches will provide info, this is just a final failsafe to let it revert to default behavior.
            }
            return null;
        }
        public static List<string> GetItemStringsRevised(TournamentGame tournamentGame, List<ItemObject.ItemTypeEnum> validTypes)
        {
            int prizeMin = MathF.Floor(GetMinPrizeValue() * .7f);
            int prizeMax = MathF.Ceiling(GetMaxPrizeValue() * 1.5f);

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
            if (TournamentXPSettings.Instance.PrizeListIncludeCustom && _customItems != null && _customItems.Count > 0)
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

        public static void SetTournamentSelectedPrize(TournamentGame tournamentGame, ItemObject prize)
        {
            typeof(TournamentGame).GetProperty("Prize").SetValue(tournamentGame, prize);
        }
        public static int GetMinPrizeValue(Settlement settlement = null)
        {
            return MathF.Floor(((float)TournamentXPSettings.Instance.PrizeValueMin + MathF.Ceiling((Hero.MainHero.Level * (float)TournamentXPSettings.Instance.PrizeValueMinIncreasePerLevel))) * GetProsperityModifier(settlement));
        }
        public static int GetMaxPrizeValue(Settlement settlement = null)
        {
            return MathF.Ceiling(((float)TournamentXPSettings.Instance.PrizeValueMax + MathF.Ceiling((Hero.MainHero.Level * (float)TournamentXPSettings.Instance.PrizeValueMaxIncreasePerLevel))) * GetProsperityModifier(settlement));
        }
        public static float GetProsperityModifier(Settlement settlement)
        {
            var prosperityMod = 1f;
            if (settlement != null && TournamentXPSettings.Instance.TownProsperityAffectsPrizeValues)
            {
                switch (settlement.Town.GetProsperityLevel())
                {
                    case SettlementComponent.ProsperityLevel.Low:
                        prosperityMod = TournamentXPSettings.Instance.TownProsperityLow;
                        break;

                    case SettlementComponent.ProsperityLevel.Mid:
                        prosperityMod = TournamentXPSettings.Instance.TownProsperityMid;
                        break;

                    case SettlementComponent.ProsperityLevel.High:
                        prosperityMod = TournamentXPSettings.Instance.TownProsperityHigh;
                        break;
                }
            }
            return prosperityMod;
        }
        public static EquipmentElement GetEquipmentWithModifier(ItemObject item, float prosperityFactor)
        {
            ItemModifierGroup itemModifierGroup;
            ArmorComponent armorComponent = item.ArmorComponent;
            if (armorComponent != null)
            {
                itemModifierGroup = armorComponent.ItemModifierGroup;
            }
            else
            {
                itemModifierGroup = null;
            }
            ItemModifierGroup itemModifierGroup1 = itemModifierGroup ?? Campaign.Current.ItemModifierGroupss.FirstOrDefault<ItemModifierGroup>((ItemModifierGroup x) => x.ItemTypeEnum == item.ItemType);
            ItemModifier itemModifierWithTarget = null;
            if (itemModifierGroup1 != null)
            {
                float prosperityVariance = 1f;
                if (prosperityFactor < 1f)
                {
                    prosperityVariance = MBRandom.RandomFloatRanged(prosperityFactor, 1f);
                }
                else
                {
                    prosperityVariance = MBRandom.RandomFloatRanged(1f, prosperityFactor);
                }

                itemModifierWithTarget = itemModifierGroup1.GetRandomModifierWithTarget(prosperityVariance, 1f);
            }
            //Toss out the bad ones - they suck as prizes
            if (itemModifierWithTarget.PriceMultiplier < 1)
            {
                itemModifierWithTarget = null;
            }
            return new EquipmentElement(item, itemModifierWithTarget);
        }
    }
}
