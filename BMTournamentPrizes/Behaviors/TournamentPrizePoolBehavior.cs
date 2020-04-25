using HarmonyLib;

using Helpers;

using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
#if VERSION130
using TaleWorlds.ObjectSystem;
#endif
using TournamentsXPanded.Common;
using TournamentsXPanded.Models;

using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Behaviors
{
    public partial class TournamentPrizePoolBehavior : CampaignBehaviorBase
    {
        internal static List<ItemObject> _customItems;
        internal static List<ItemObject> _legacyItems;

        public static TournamentReward TournamentReward { get; set; }
        public static List<ItemObject> CustomTournamentItems { get { return _customItems; }  }
        public static List<ItemObject> LegacyTournamentItems { get { return _legacyItems;  } }
        public TournamentPrizePoolBehavior()
        {
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
                obj.Prizes = new ItemRoster
                {
                    new ItemRosterElement(prize, 1)
                };
                obj.SelectedPrizeStringId = prize.StringId;
            }
            else
            {
                if (settlement.HasTournament)
                {
                    var townPrize = Campaign.Current.TournamentManager.GetTournamentGame(settlement.Town).Prize;
                    if (townPrize.StringId != obj.SelectedPrizeStringId)
                    {
                        obj.Prizes = new ItemRoster
                        {
                            new ItemRosterElement(townPrize, 1)
                        };
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
            //if (dataStore.IsSaving && TournamentXPSettings.Instance.EnableCleanSave)
            //{
            //    List<TournamentPrizePool> prizePools = new List<TournamentPrizePool>();
            //    MBObjectManager.Instance.GetAllInstancesOfObjectType<TournamentPrizePool>(ref prizePools);
            //    foreach (var pp in prizePools)
            //    {
            //        MBObjectManager.Instance.UnregisterObject(pp);             
            //    }

            //    TournamentManager tournamentManager = Campaign.Current.TournamentManager as TournamentManager;
            //    foreach (var s in Campaign.Current.Settlements)
            //    {
            //        if (s.HasTournament)
            //        {
            //            TournamentGame tg = tournamentManager.GetTournamentGame(s.Town);
            //            if (tg is Fight2TournamentGame)
            //            {
            //                ((List<TournamentGame>)Traverse.Create(tournamentManager).Field("_activeTournaments").GetValue()).Remove(tg);                            
            //            }
            //        }
            //    }
            //    InformationManager.DisplayMessage(new InformationMessage("TournamentXPanded saved in clean state.", Colors.Red));

            //}
        }

        private void OnAfterNewGameCreated(CampaignGameStarter starter)
        {
            //To get these to line up with the other tournament items, have to move them to somewhere else
            GameMenu menuArena = ((Dictionary<string, GameMenu>)Traverse.Create(Campaign.Current.GameMenuManager).Field("_gameMenus").GetValue())["town_arena"];

            if (TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament > 0)
            {
                var text = new TextObject("{=tourn001}Re-roll Prize"); //Was going to put the remaining count, but not updating correctly.
                starter.AddGameMenuOption("town_arena", "bm_reroll_tournamentprize", text.ToString(),
                    new GameMenuOption.OnConditionDelegate(RerollCondition),
                    new GameMenuOption.OnConsequenceDelegate(RerollConsequence),
                    false, -1, true);
            }
            if (TournamentXPSettings.Instance.EnablePrizeSelection)
            {
                starter.AddGameMenuOption("town_arena", "bm_select_tournamentprize", new TextObject("{=tourn002}Select Prize").ToString(),
                 new GameMenuOption.OnConditionDelegate(PrizeSelectCondition),
                 new GameMenuOption.OnConsequenceDelegate(PrizeSelectConsequence), false, 1, true);
            }

            if (TournamentXPSettings.Instance.EnableTournamentTypeSelection)
            {
                starter.AddGameMenuOption("town_arena", "bm_select_tournamenttype", new TextObject("{=tourn003}Select Tournament Style").ToString(),
                 new GameMenuOption.OnConditionDelegate(TournamentTypeSelectCondition),
                 new GameMenuOption.OnConsequenceDelegate(TournamentTypeSelectConsequence), false, 2, true);
            }
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
                textObject = new TextObject("{=tourn004}Re-roll Attempts Exceeded");
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
                    string info = "{=tourn012}You can choose an item from the list below as your reward if you win the tournament!";
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
                        foreach (var t in TournamentPrizePoolBehavior.GetActivePrizeTypes())
                        {
                            types += t.ToString() + ", ";
                        }
                        types = types.Substring(0, types.Length - 2);
                        descr.SetTextVariable("TYPES", types);
                    }

                    InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                           new TextObject("{=tourn005}Tournament Prize Selection").ToString(), descr.ToString(), prizeElements, true, true, new TextObject("{=tourn006}OK").ToString(), new TextObject("{=tourn007}Cancel").ToString(),
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
            tournamentTypeElements.Add(new InquiryElement("melee", new TextObject("{=tourn008}Standard Melee Tournament").ToString(), new ImageIdentifier("battania_noble_sword_2_t5", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("melee2", new TextObject("{=tourn009}Individual Only Melee Tournament").ToString(), new ImageIdentifier("battania_noble_sword_2_t5", ImageIdentifierType.Item)));


            //tournamentTypeElements.Add(new InquiryElement("melee3", new TextObject("{=tourn013}Battle Royal Melee Tournament").ToString(), new ImageIdentifier("battania_noble_sword_2_t5", ImageIdentifierType.Item)));
#if DEBUG
            //tournamentTypeElements.Add(new InquiryElement("archery", "Archery Tournament", new ImageIdentifier("training_longbow", ImageIdentifierType.Item)));
            //tournamentTypeElements.Add(new InquiryElement("joust", "Jousting Tournament", new ImageIdentifier("khuzait_lance_3_t5", ImageIdentifierType.Item)));
            //tournamentTypeElements.Add(new InquiryElement("race", "Horse Racing Tournament", new ImageIdentifier("desert_war_horse", ImageIdentifierType.Item)));
     //       tournamentTypeElements.Add(new InquiryElement("race", "External Application Tournament", new ImageIdentifier("desert_war_horse", ImageIdentifierType.Item)));
#endif
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                    new TextObject("{=tourn010}Tournament Type Selection").ToString(), new TextObject("{=tourn011}What kind of Tournament would you like to compete in today?").ToString(), tournamentTypeElements, true, true, new TextObject("{=tourn006}OK").ToString(), new TextObject("{=tourn007}Cancel").ToString(),
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
                        TournamentPrizePoolBehavior.TournamentReward = new TournamentReward(tournamentGame);
                        break;

                    //case "melee3":
                    //    tournamentGame = new BattleRoyalTournamentGame(town);                       
                    //    TournamentPrizePoolBehavior.TournamentReward = new TournamentReward(tournamentGame);
                    //    break;
                        
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
                        {
                            worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.IsMajorFactionLeader];
                        }

                        if (hero.MapFaction.IsMinorFaction)
                        {
                            worth += TournamentPrizePoolBehavior.RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                        }
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
            threat += character.GetSkillValue(DefaultSkills.Bow)
                + (float)character.GetSkillValue(DefaultSkills.OneHanded)
                + character.GetSkillValue(DefaultSkills.TwoHanded)
                + character.GetSkillValue(DefaultSkills.Throwing)
                + character.GetSkillValue(DefaultSkills.Polearm)
                + character.GetSkillValue(DefaultSkills.Riding);
            threat += character.HitPoints;

            return threat;
        }

        public static List<ItemObject.ItemTypeEnum> GetActivePrizeTypes()
        {
            //if (!TournamentXPSettings.Instance.EnablePrizeTypeFilterToLists)
            //{
            //    return _allValidTypes;
            //}

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
            if (TournamentXPSettings.Instance.EnableItemType_Arrow)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Arrows);
            }
            if (TournamentXPSettings.Instance.EnableItemType_Bolt)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Bolts);
            }
            return validTypes;
        }

#if VERSION120 || VERSION130
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
#endif

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

        public static List<ItemObject.ItemTypeEnum> _allValidTypes = new List<ItemObject.ItemTypeEnum>() { ItemObject.ItemTypeEnum.BodyArmor, ItemObject.ItemTypeEnum.Bow, ItemObject.ItemTypeEnum.Cape, ItemObject.ItemTypeEnum.Crossbow, ItemObject.ItemTypeEnum.HandArmor, ItemObject.ItemTypeEnum.HeadArmor, ItemObject.ItemTypeEnum.Horse, ItemObject.ItemTypeEnum.HorseHarness, ItemObject.ItemTypeEnum.LegArmor, ItemObject.ItemTypeEnum.OneHandedWeapon, ItemObject.ItemTypeEnum.Polearm, ItemObject.ItemTypeEnum.Shield, ItemObject.ItemTypeEnum.Thrown, ItemObject.ItemTypeEnum.TwoHandedWeapon, ItemObject.ItemTypeEnum.Arrows, ItemObject.ItemTypeEnum.Bolts };

#endregion Rewards and Calculations
    }
}