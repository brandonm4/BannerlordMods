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
using TaleWorlds.Localization;


using TournamentsXPanded.Extensions;
using TournamentsXPanded.Models;
using TaleWorlds.Library;
using ModLib;

namespace TournamentsXPanded.Behaviors
{
    public class TournamentPrizePoolBehavior : CampaignBehaviorBase
    {
        public static TournamentReward TournamentReward { get; set; }
        public static List<ItemObject> CustomTourneyItems { get; set; }

        public TournamentPrizePoolBehavior()
        {
        }

        public static TournamentPrizePool GetTournamentPrizePool(string settlementStringId)
        {
            return GetTournamentPrizePool(Campaign.Current.Settlements.Where(x => x.StringId == settlementStringId).Single());
        }

        public static TournamentPrizePool GetTournamentPrizePool(Settlement settlement, ItemObject prize = null)
        {
            Town component = settlement.Town;
            TournamentPrizePool obj = MBObjectManager.Instance.GetObject<TournamentPrizePool>((TournamentPrizePool x) => x.Town == component);
            if (obj == null)
            {
                obj = MBObjectManager.Instance.CreateObject<TournamentPrizePool>();
                obj.Town = component;
            }
            if (prize != null)
            {
                obj.Prizes = new ItemRoster();
                obj.Prizes.Add(new ItemRosterElement(prize, 1));
                obj.SelectedPrizeStringId = prize.StringId;
            }
            else
            {
                if (settlement.HasTournament)
                {
                    var townPrize = Campaign.Current.TournamentManager.GetTournamentGame(settlement.Town).Prize;
                    if (townPrize.StringId != obj.SelectedPrizeStringId)
                    {
                        obj.Prizes = new ItemRoster();
                        obj.Prizes.Add(new ItemRosterElement(townPrize, 1));
                        obj.SelectedPrizeStringId = townPrize.StringId;
                        obj.RemainingRerolls = TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament;
                    }
                }                
            }
            return obj;
        }

        public static void ClearTournamentPrizes(string settlement_string_id)
        {
            ClearTournamentPrizes(Campaign.Current.Settlements.Where(x => x.StringId == settlement_string_id).Single());
        }

        public static void ClearTournamentPrizes(Settlement settlement)
        {
            var currentPool = GetTournamentPrizePool(settlement);
            currentPool.Prizes = new ItemRoster();
            currentPool.SelectedPrizeStringId = "";
            currentPool.RemainingRerolls = TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament;
        }

      


        #region Events

        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
            CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener(this, new Action(this.OnCleanSave));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnAfterNewGameCreated(CampaignGameStarter starter)
        {
            //To get these to line up with the other tournament items, have to move them to somewhere else
            GameMenu menuArena = ((Dictionary<string, GameMenu>)Traverse.Create(Campaign.Current.GameMenuManager).Field("_gameMenus").GetValue())["town_arena"];

            if (TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament > 0)
            {
                var text = new TextObject("Re-roll Prize"); //Was going to put the remaining count, but not updating correctly.
                starter.AddGameMenuOption("town_arena", "bm_reroll_tournamentprize", text.ToString(),
                    new GameMenuOption.OnConditionDelegate(RerollCondition),
                    new GameMenuOption.OnConsequenceDelegate(RerollConsequence),
                    false, -1, true);
            }
            if (TournamentXPSettings.Instance.EnablePrizeSelection)
            {
                starter.AddGameMenuOption("town_arena", "bm_select_tournamentprize", "Select Prize",
                 new GameMenuOption.OnConditionDelegate(PrizeSelectCondition),
                 new GameMenuOption.OnConsequenceDelegate(PrizeSelectConsequence), false, 1, true);
            }

            starter.AddGameMenuOption("town_arena", "bm_select_tournamenttype", "Select Tournament Style",
             new GameMenuOption.OnConditionDelegate(TournamentTypeSelectCondition),
             new GameMenuOption.OnConsequenceDelegate(TournamentTypeSelectConsequence), false, 2, true);
        }

        private void OnCleanSave()
        {
            //if (TournamentXPSettings.Instance.EnableCleanSaveProcess)
            //{
            //    List<TournamentPrizePool> prizePools = new List<TournamentPrizePool>();
            //    MBObjectManager.Instance.GetAllInstancesOfObjectType<TournamentPrizePool>(ref prizePools);
            //    foreach(var pp in prizePools)
            //    {
            //        MBObjectManager.Instance.UnregisterObject(pp);
            //    }
            //}
        }

        #endregion Events

        #region Prizes

        public static ItemObject GenerateTournamentPrize(TournamentGame tournamentGame, TournamentPrizePool currentPool = null, bool keepTownPrize = true)
        {
            ItemObject prize;
            var numItemsToGet = TournamentXPSettings.Instance.NumberOfPrizeOptions;
            bool bRegenAllPrizes = false;
            if (currentPool == null)
            {
                bRegenAllPrizes = true;
                currentPool = GetTournamentPrizePool(tournamentGame.Town.Settlement);
            }            
            var allitems = GetItemStringsRevised(tournamentGame, TournamentPrizePoolBehavior.GetMinPrizeValue(), TournamentPrizePoolBehavior.GetMaxPrizeValue(), TournamentPrizePoolBehavior.GetActivePrizeTypes());

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
                            pickeditems.Add(existingPrize.EquipmentElement.Item.StringId);
                        if (allitems.Contains(existingPrize.EquipmentElement.Item.StringId))
                            allitems.Remove(existingPrize.EquipmentElement.Item.StringId);
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
                var pickedPrize = Game.Current.ObjectManager.GetObject<ItemObject>(id);

                if (TournamentXPSettings.Instance.EnableItemModifiersForPrizes)
                {
                    if (pickedPrize.HasArmorComponent)
                    {
                        ItemModifierGroup itemModifierGroup = pickedPrize.ArmorComponent.ItemModifierGroup;
                        if (itemModifierGroup != null)
                        {
                            itemModifier = itemModifierGroup.GetRandomItemModifier(1f);
                        }
                        else
                        {
                            itemModifier = null;
                        }
                    }
                }
                currentPool.Prizes.Add(new ItemRosterElement(pickedPrize, 1, itemModifier));
                // currentPool.Prizes.Add(new ItemRosterElement(pickedPrize, 1, null)); //Turn off random item mods for now;
            }

            if (!keepTownPrize)
            {
                var selected = currentPool.Prizes.GetRandomElement<ItemRosterElement>();
                currentPool.SelectedPrizeStringId = selected.EquipmentElement.Item.StringId;
                SetTournamentSelectedPrize(tournamentGame, selected.EquipmentElement.Item);
            }
            return currentPool.SelectPrizeItemRosterElement.EquipmentElement.Item;
        }

        internal static List<string> GetValidTownItems(TournamentGame tournamentGame, int minValue, int maxValue, List<ItemObject.ItemTypeEnum> validtypes)
        {
            var roster = tournamentGame.Town.Owner.ItemRoster;
            roster.RemoveZeroCounts();
            var list = roster.Where(x =>
            x.Amount > 0
            && validtypes.Contains(x.EquipmentElement.Item.ItemType)
           && x.EquipmentElement.Item.Value >= minValue
           && x.EquipmentElement.Item.Value <= maxValue
           && !x.EquipmentElement.Item.NotMerchandise
              ).Select(x => x.EquipmentElement.Item.StringId).ToList();

         
            //if (list.Count == 0)
            //{
            //    list = TournamentPrizePoolBehavior.CustomTourneyItems;
            //    ErrorLog.Log("TournamentPrizeSystem in " + tournamentGame.Town.Name.ToString() + " : " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            //    ErrorLog.Log("No custom prizes found in value range");
            //}
            //if (list.Count == 0)
            //{
            //    MessageBox.Show("Tournament Prize System", "Warning: The current town has no prizes available with your current defined filter.  Defaulting to Vanilla items.");
            //    //list = PrizeConfiguration.StockTourneyItems;
            //    list = GetVanillaSetOfPrizes(tournamentGame.Town.Settlement, TournamentXPSettings.Instance.NumberOfPrizeOptions);
            //}
            return list;
        }

        public static List<string> GetItemStringsRevised(TournamentGame tournamentGame, int minValue, int maxMalue, List<ItemObject.ItemTypeEnum> validTypes)
        {
            string[] strArray = new String[] { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
            List<string> allitems = new List<string>();
            if (TournamentXPSettings.Instance.PrizeListIncludeLegacy)
            {               
                allitems = allitems.Concat(strArray.ToList()).ToList();
            }
            if (TournamentXPSettings.Instance.PrizeListIncludeCustom && TournamentPrizePoolBehavior.CustomTourneyItems != null && TournamentPrizePoolBehavior.CustomTourneyItems.Count > 0)
            {
                try
                {                   
                    var customItems = TournamentPrizePoolBehavior.CustomTourneyItems.Where(x => validTypes.Contains(x.ItemType));
                    if (TournamentXPSettings.Instance.TownPrizeMinMaxAffectsCustom)
                    {
                        customItems = customItems.Where(x => x.Value >= TournamentPrizePoolBehavior.GetMinPrizeValue() && x.Value <= TournamentPrizePoolBehavior.GetMaxPrizeValue());
                    }                  
                    allitems = allitems.Concat(customItems.Select(x => x.StringId).ToList()).ToList();
                }
                catch(Exception ex)
                {
                    ErrorLog.Log("Error adding custom items to prize pool.");
                    ErrorLog.Log(ex.ToStringFull());
                }
            }
            if (TournamentXPSettings.Instance.PrizeListIncludeTown)
            {
                var townItems = GetValidTownItems(tournamentGame, minValue, maxMalue, validTypes);
                allitems = allitems.Concat(townItems).ToList();
            }
            if (TournamentXPSettings.Instance.PrizeListIncludeVanilla)
            {
                float _minValue = 1600f;
                float _maxValue = 5000f;
                List<string> vanillaItems;
                if (TournamentXPSettings.Instance.TownPrizeMinMaxAffectsVanilla)
                {
                    _minValue = minValue;
                    _maxValue = maxMalue;
                }
                vanillaItems = ItemObject.All.Where(x => x.Culture == tournamentGame.Town.Settlement.Culture && validTypes.Contains(x.ItemType) && x.Value >= _minValue && x.Value <=_maxValue).Select(x => x.StringId).ToList();
                if (vanillaItems.Count == 0)
                {
                    vanillaItems = ItemObject.All.Where(x => validTypes.Contains(x.ItemType) && x.Value >= _minValue && x.Value <= _maxValue).Select(x => x.StringId).ToList();
                }

                allitems = allitems.Concat(vanillaItems).ToList();
            }
            if (allitems.Count == 0)
            {
                return strArray.ToList();
            }

            return allitems;
        }

        public static ItemObject GetTournamentPrizeVanilla(Settlement settlement)
        {
            float minValue = 1000f;
            float maxValue = 5000f;

            if (TournamentXPSettings.Instance.TownPrizeMinMaxAffectsVanilla)
            {
                minValue = TournamentPrizePoolBehavior.GetMinPrizeValue();
                maxValue = TournamentPrizePoolBehavior.GetMaxPrizeValue();
            }

            string[] strArray = new String[] { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };

            ItemObject obj = Game.Current.ObjectManager.GetObject<ItemObject>(strArray.GetRandomElement<string>());
            ItemObject itemObject = MBRandom.ChooseWeighted<ItemObject>(ItemObject.All, (ItemObject item) =>
            {
                if ((float)item.Value > minValue * (item.IsMountable ? 0.5f : 1f))
                {
                    if (TournamentXPSettings.Instance.EnablePrizeTypeFilterToLists)
                    {
                        var validPizeTypes = TournamentPrizePoolBehavior.GetActivePrizeTypes();

                        if ((float)item.Value < maxValue * (item.IsMountable ? 0.5f : 1f) && item.Culture == settlement.Town.Culture &&
                            validPizeTypes.Contains(item.ItemType)
                        )
                        {
                            return 1f;
                        }
                    }
                    else if ((float)item.Value < maxValue * (item.IsMountable ? 0.5f : 1f) && item.Culture == settlement.Town.Culture && (item.IsCraftedWeapon || item.IsMountable || item.ArmorComponent != null))
                    {
                        return 1f;
                    }
                }
                return 0f;
            }) ?? MBRandom.ChooseWeighted<ItemObject>(ItemObject.All, (ItemObject item) =>
            {
                if ((float)item.Value > minValue * (item.IsMountable ? 0.5f : 1f))
                {
                    if ((float)item.Value < maxValue * (item.IsMountable ? 0.5f : 1f) && (item.IsCraftedWeapon || item.IsMountable || item.ArmorComponent != null))
                    {
                        return 1f;
                    }
                }
                return 0f;
            });
            if (itemObject == null)
            {
                return obj;
            }
            return itemObject;
        }

        public static List<string> GetVanillaSetOfPrizes(Settlement settlement, int count)
        {
            List<string> prizes = new List<string>();
            int retryMax = 50;
            int currentTry = 0;
            while (prizes.Count < count && currentTry < retryMax)
            {
                var stringid = GetTournamentPrizeVanilla(settlement).StringId;
                if (!prizes.Contains(stringid))
                {
                    prizes.Add(stringid);
                }
                currentTry++;
            }
            return prizes;
        }

        public static void SetTournamentSelectedPrize(TournamentGame tournamentGame, ItemObject prize)
        {
            typeof(TournamentGame).GetProperty("Prize").SetValue(tournamentGame, prize);
        }

        #endregion Prizes

        #region Menu Entries

        private static bool RerollCondition(MenuCallbackArgs args)
        {
            if (TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament == 0)
            {
                return false;
            }
            bool flag;
            TextObject textObject;
            TournamentPrizePool settings = GetTournamentPrizePool(Settlement.CurrentSettlement);
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
                TournamentPrizePool settings = GetTournamentPrizePool(Settlement.CurrentSettlement);
                TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);

                //Clear old prizes
                settings.SelectedPrizeStringId = null;
                settings.Prizes = new ItemRoster();
                settings.RemainingRerolls--;

                //Generate New Prize
                var prize = GenerateTournamentPrize(tournamentGame, settings, false);
                SetTournamentSelectedPrize(tournamentGame, prize);

                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("ERROR: BMTournamentXP: Re-roll: Refreshing Arena Menu:");
                    ErrorLog.Log(ex.ToStringFull());
                }
            }
            catch (Exception ex)
            {                
                ErrorLog.Log("Re-roll Prize Pool");
                ErrorLog.Log(ex.ToStringFull());
            }
        }

        public static bool PrizeSelectCondition(MenuCallbackArgs args)
        {
            if (!TournamentXPSettings.Instance.EnablePrizeSelection)
            {
                return false;
            }
            bool flag;
            TextObject textObject;
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
        }

        public static void PrizeSelectConsequence(MenuCallbackArgs args)
        {
            try
            {
                List<InquiryElement> prizeElements = new List<InquiryElement>();
                TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
                TournamentPrizePool currentPool = GetTournamentPrizePool(Settlement.CurrentSettlement);

                if (currentPool.Prizes.Count < TournamentXPSettings.Instance.NumberOfPrizeOptions)
                {
                    ItemObject prize = GenerateTournamentPrize(tournamentGame, currentPool, true);
                }

                //  InformationManager.Clear();
                foreach (ItemRosterElement ire in currentPool.Prizes)
                {
                    var p = ire.EquipmentElement;
                    try
                    {
                        var ii = new ImageIdentifier(p.Item.StringId, ImageIdentifierType.Item, p.GetModifiedItemName().ToString());
                        // prizeElements.Add(new InquiryElement(p.Item.StringId, ii, true, p.Item.ToToolTipTextObject().ToString()));
                        prizeElements.Add(new InquiryElement(
                            p.Item.StringId,
                            p.GetModifiedItemName().ToString(),
                            ii,
                            true,
                            p.ToToolTipTextObject().ToString()
                            ));
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("ERROR: Tournament Prize System\nFailed to add prize element to display" + p.Item.StringId);
                        ErrorLog.Log(ex.ToStringFull());
                    }
                }
                if (prizeElements.Count > 0)
                {
                    string info = "You can choose an item from the list below as your reward if you win the tournament!";
                    TextObject descr = new TextObject(info);

                    if (TournamentXPSettings.Instance.DebugMode)
                    {
                        info = "Town:{TOWN}\nMin:{MIN}\nMax:{MAX}\nProsperity:{PROSPERITY}\nTypes:{TYPES}";
                        descr = new TextObject(info);
                        descr.SetTextVariable("TOWN", currentPool.Town.Name);
                        descr.SetTextVariable("MIN", TournamentPrizePoolBehavior.GetMinPrizeValue().ToString());
                        descr.SetTextVariable("MAX", TournamentPrizePoolBehavior.GetMaxPrizeValue().ToString());
                        descr.SetTextVariable("PROSPERITY", currentPool.Town.GetProsperityLevel().ToString());
                        var types = "";
                        foreach(var t in TournamentPrizePoolBehavior.GetActivePrizeTypes())
                        {
                            types += t.ToString() + ", ";
                        }
                        types = types.Substring(0, types.Length - 2);
                        descr.SetTextVariable("TYPES", types);
                    }
                    

                    InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                            "Tournament Prize Selection", descr.ToString(), prizeElements, true, true, "OK", "Cancel",
                            new Action<List<InquiryElement>>(OnSelectPrize), new Action<List<InquiryElement>>(OnDeSelectPrize)), true);
                    try
                    {
                        GameMenu.SwitchToMenu("town_arena");
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("ERROR: BMTournamentXP: Select Prize: Refresh Menu");
                        ErrorLog.Log(ex.ToStringFull());
                    }
                }
                else
                {
                    InformationManager.ShowInquiry(new InquiryData("Tournament Prize Selection", "You should not be seeing this.  Something went wrong generating the prize list. Your item restrictions may be set too narrow.", true, false, "OK", "", null, null));
                }
            }
            catch (Exception ex)
            {               
                ErrorLog.Log("ERROR: BMTournamentXP: Tournament Prize Selection");
                ErrorLog.Log(ex.ToStringFull());
            }
        }

        private static void OnSelectPrize(List<InquiryElement> prizeSelections)
        {
            if (prizeSelections.Count > 0)
            {
                try
                {
                    TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
                    TournamentPrizePool currentPool = GetTournamentPrizePool(Settlement.CurrentSettlement);
                    currentPool.SelectedPrizeStringId = prizeSelections.First().Identifier.ToString();
                    var prize = Game.Current.ObjectManager.GetObject<ItemObject>(prizeSelections.First().Identifier.ToString());
                    SetTournamentSelectedPrize(tournamentGame, prize);
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("ERROR: BMTournamentXP: OnSelectPrize: Error setting Town Prize");
                    ErrorLog.Log(ex.ToStringFull());
                }
                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("ERROR: BMTournamentXP: OnSelectPrize: Refresh Arena Menu");
                    ErrorLog.Log(ex.ToStringFull());
                }
            }
        }

        private static void OnDeSelectPrize(List<InquiryElement> prizeSelections)
        {
        }

        public static bool TournamentTypeSelectCondition(MenuCallbackArgs args)
        {
            //if (!TournamentConfiguration.Instance.EnableTournamentTypeSelection)
            //{
            //    return false;
            //}
            bool flag;
            TextObject textObject;
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
        }

        public static void TournamentTypeSelectConsequence(MenuCallbackArgs args)
        {
            List<InquiryElement> tournamentTypeElements = new List<InquiryElement>();
            tournamentTypeElements.Add(new InquiryElement("melee", "Standard Melee Tournament", new ImageIdentifier("battania_noble_sword_2_t5", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("melee2", "Individual Only Melee Tournament", new ImageIdentifier("battania_noble_sword_2_t5", ImageIdentifierType.Item)));
#if DEBUG
            //tournamentTypeElements.Add(new InquiryElement("archery", "Archery Tournament", new ImageIdentifier("training_longbow", ImageIdentifierType.Item)));
            //tournamentTypeElements.Add(new InquiryElement("joust", "Jousting Tournament", new ImageIdentifier("khuzait_lance_3_t5", ImageIdentifierType.Item)));
            //tournamentTypeElements.Add(new InquiryElement("race", "Horse Racing Tournament", new ImageIdentifier("desert_war_horse", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("race", "External Application Tournament", new ImageIdentifier("desert_war_horse", ImageIdentifierType.Item)));
#endif
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                    "Tournament Type Selection", "What kind of Tournament would you like to compete in today?", tournamentTypeElements, true, true, "OK", "Cancel",
                    new Action<List<InquiryElement>>(OnSelectTournamentStyle), new Action<List<InquiryElement>>(OnSelectDoNothing)), true);

            try
            {
                GameMenu.SwitchToMenu("town_arena");
            }
            catch (Exception ex)
            {
                ErrorLog.Log("ERROR: BMTournamentXP: Select TournyType: Refreshing Arena Menu:");
                ErrorLog.Log(ex.ToStringFull());
            }
        }

        private static void OnSelectTournamentStyle(List<InquiryElement> selectedTypes)
        {
            if (selectedTypes.Count > 0)
            {
                var town = Settlement.CurrentSettlement.Town;
                TournamentManager tournamentManager = Campaign.Current.TournamentManager as TournamentManager;
                TournamentGame tournamentGame;
                TournamentGame currentGame = tournamentManager.GetTournamentGame(town);

                switch (selectedTypes.First().Identifier.ToString())
                {
                    case "melee":
                        tournamentGame = new FightTournamentGame(town);
                        TournamentPrizePoolBehavior.TournamentReward = new TournamentReward(tournamentGame);
                        break;

                    case "melee2":
                        tournamentGame = new Fight2TournamentGame(town);
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

                    default:
                        tournamentGame = new FightTournamentGame(town);
                        break;
                }

                if (tournamentGame.GetType() != currentGame.GetType())
                {
                    ((List<TournamentGame>)Traverse.Create(tournamentManager).Field("_activeTournaments").GetValue()).Remove(currentGame);
                    tournamentManager.AddTournament(tournamentGame);
                }

                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                    ErrorLog.Log(ex.ToStringFull());
                }
            }
        }

        private static void OnSelectDoNothing(List<InquiryElement> prizeSelections)
        {
        }

        #endregion Menu Entries

        #region Rewards and Calculations

        public static float GetRenownValue(CharacterObject character)
        {
            var worth = 0f;
            if (character.IsHero)
            {
                worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.HeroBase];
                var hero = character.HeroObject;
                if (hero != null)
                {
                    if (hero.IsNoble)
                    {
                        worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.IsNoble];
                    }
                    if (hero.IsNotable)
                    {
                        worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.IsNotable];
                    }
                    if (hero.IsCommander)
                    {
                        worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.IsCommander];
                    }
                    if (hero.IsMinorFactionHero)
                    {
                        worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                    }
                    if (hero.IsFactionLeader)
                    {
                        if (hero.MapFaction.IsKingdomFaction)
                            worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.IsMajorFactionLeader];
                        if (hero.MapFaction.IsMinorFaction)
                            worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                    }
                }
            }
            else
            {
                worth += TournamentPrizePoolBehavior.RenownPerTroopTier[character.Tier];
            }
            return worth;
        }

        public static float GetTournamentThreatLevel(CharacterObject character)
        {
            float threat = 0f;
            //TournamentXP addon for Odd Calculations
            //Get armor bonus
            threat += character.GetArmArmorSum() * 2 + character.GetBodyArmorSum() * 4 + character.GetLegArmorSum() + character.GetHeadArmorSum() * 2;
            ////Get skills based
            threat += (float)character.GetSkillValue(DefaultSkills.Bow)
                + (float)character.GetSkillValue(DefaultSkills.OneHanded)
                + (float)character.GetSkillValue(DefaultSkills.TwoHanded)
                + (float)character.GetSkillValue(DefaultSkills.Throwing)
                + (float)character.GetSkillValue(DefaultSkills.Polearm)
                + (float)character.GetSkillValue(DefaultSkills.Riding);
            threat += (float)character.HitPoints;

            return threat;
        }
        public static List<ItemObject.ItemTypeEnum> GetActivePrizeTypes()
        {
            if (!TournamentXPSettings.Instance.EnablePrizeTypeFilterToLists)
                return _allValidTypes;

            List<ItemObject.ItemTypeEnum> validTypes = new List<ItemObject.ItemTypeEnum>();
            if (TournamentXPSettings.Instance.EnableItemType_BodyArmor)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.BodyArmor);
            }
            if (TournamentXPSettings.Instance.EnableItemType_Bow)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Bow);
            }
            if (TournamentXPSettings.Instance.EnableItemType_Cape)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Cape);
            }
            if (TournamentXPSettings.Instance.EnableItemType_Crossbow)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Crossbow);
            }
            if (TournamentXPSettings.Instance.EnableItemType_HandArmor)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.HandArmor);
            }
            if (TournamentXPSettings.Instance.EnableItemType_HeadArmor)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.HeadArmor);
            }
            if (TournamentXPSettings.Instance.EnableItemType_Horse)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Horse);
            }
            if (TournamentXPSettings.Instance.EnableItemType_HorseHarness)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.HorseHarness);
            }
            if (TournamentXPSettings.Instance.EnableItemType_LegArmor)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.LegArmor);
            }
            if (TournamentXPSettings.Instance.EnableItemType_OneHandedWeapon)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.OneHandedWeapon);
            }
            if (TournamentXPSettings.Instance.EnableItemType_Polearm)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Polearm);
            }
            if (TournamentXPSettings.Instance.EnableItemType_Shield)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Shield);
            }
            if (TournamentXPSettings.Instance.EnableItemType_Thrown)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Thrown);
            }
            if (TournamentXPSettings.Instance.EnableItemType_TwoHandedWeapon)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.TwoHandedWeapon);
            }

            return validTypes;
        }
        public static int GetMinPrizeValue(Settlement settlement = null)
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

            return MathF.Ceiling(((float)TournamentXPSettings.Instance.TownPrizeMin + MathF.Ceiling(((float)Hero.MainHero.Level * (float)TournamentXPSettings.Instance.PrizeValueMinIncreasePerLevel) )) * prosperityMod);
        }

        public static int GetMaxPrizeValue(Settlement settlement = null)
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

            return MathF.Ceiling(((float)TournamentXPSettings.Instance.TownPrizeMax + MathF.Ceiling(((float)Hero.MainHero.Level * (float)TournamentXPSettings.Instance.PrizeValueMaxIncreasePerLevel))) * prosperityMod);
        }
        public static float[] RenownPerTroopTier
        {
            get
            {
                return new[] { 0f, TournamentXPSettings.Instance.RenownTroopTier1, TournamentXPSettings.Instance.RenownTroopTier2, TournamentXPSettings.Instance.RenownTroopTier3, TournamentXPSettings.Instance.RenownTroopTier4, TournamentXPSettings.Instance.RenownTroopTier5, TournamentXPSettings.Instance.RenownTroopTier6, TournamentXPSettings.Instance.RenownTroopTier7 };
            }
        }

        public static float[] RenownPerHeroProperty
        {
            get
            {
                return new[] { TournamentXPSettings.Instance.RenownPerHeroPropertyHeroBase, TournamentXPSettings.Instance.RenownPerHeroPropertyIsNoble, TournamentXPSettings.Instance.RenownPerHeroPropertyIsNotable, TournamentXPSettings.Instance.RenownPerHeroPropertyIsCommander, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMinorFactionHero, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMinorFactionLeader, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMajorFactionLeader };
            }
        }

        public static List<ItemObject.ItemTypeEnum> _allValidTypes = new List<ItemObject.ItemTypeEnum>() { ItemObject.ItemTypeEnum.BodyArmor, ItemObject.ItemTypeEnum.Bow, ItemObject.ItemTypeEnum.Cape, ItemObject.ItemTypeEnum.Crossbow, ItemObject.ItemTypeEnum.HandArmor, ItemObject.ItemTypeEnum.HeadArmor, ItemObject.ItemTypeEnum.Horse, ItemObject.ItemTypeEnum.HorseHarness, ItemObject.ItemTypeEnum.LegArmor, ItemObject.ItemTypeEnum.OneHandedWeapon, ItemObject.ItemTypeEnum.Polearm, ItemObject.ItemTypeEnum.Shield, ItemObject.ItemTypeEnum.Thrown, ItemObject.ItemTypeEnum.TwoHandedWeapon };
        #endregion Rewards and Calculations
    }
}