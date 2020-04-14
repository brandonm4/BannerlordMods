﻿using ModLib;
using ModLib.Attributes;

using System.Collections.Generic;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TournamentsXPanded.Models
{
    public class TournamentXPSettings : SettingsBase
    {
        private const string instanceID = "TournamentsXPanded";
        private static TournamentXPSettings _instance = null;
        public override string ModName => "TournamentsXPanded";
        public override string ModuleFolderName => "TournamentsXPanded";

        public static TournamentXPSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FileDatabase.Get<TournamentXPSettings>(instanceID);
                    if (_instance == null)
                    {
                        _instance = new TournamentXPSettings();
                        _instance.ID = instanceID;

                        SettingsDatabase.SaveSettings(_instance);
                    }
                }
                return _instance;
            }
        }

        [XmlElement]
        public override string ID { get; set; } = instanceID;

        #region Debug Settings

        public bool DebugMode { get; set; } = false;

        #endregion Debug Settings

        #region Re-Roll

        [XmlElement]
        [SettingProperty("Re-rolls per Tournament", "Maximum number of times you can re-roll the prize pool per tournament.  Set to 0 to disable.")]
        [SettingPropertyGroup("Re-Roll")]
        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;

        #endregion Re-Roll

        #region Prize Selection

        [XmlElement]
        [SettingProperty("Prize Selection", "Should the Select Prize from the Prize Pool be enable. ")]
        public bool EnablePrizeSelection { get; set; } = true;

        [XmlElement]
        [SettingProperty("Prize List Mode", "Where do tournaments get prizes from:\nVanilla is standard behavior\nCustom is from a custom list you provide.\nTownCustom,TownVanilla,TownOnly will also pull items from the Town's stock.")]
        public int PrizeListMode { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Town Prize Min Value", "Any item below this value will not be used.")]
        public int TownPrizeMin { get; set; } = 1600;

        [XmlElement]
        [SettingProperty("Town Prize Max Value", "Any item above this value will not be used.")]
        public int TownPrizeMax { get; set; } = 8500;

        [XmlElement]
        [SettingProperty("Prize Value Filter", "Tooltip")]
        public bool TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell { get; set; } = false;


        [XmlElement]
        [SettingProperty("Prize Value Filter", "Tooltip")]
        public bool TownProsperityAffectsPrizeValues { get; set; } = true;


        [XmlElement]
        [SettingProperty("Prize Type Filter", "Tooltip")]
        public bool EnablePrizeTypeFilterToLists { get; set; } = false;

        [XmlElement]
        [SettingProperty("Prize Value Per Level")]
        public int PrizeValueIncreasePerLevel { get; set; } = 1000;

        public bool EnableItemType_BodyArmor { get; set; } = true;        
        public bool EnableItemType_Bow { get; set; } = true;
        public bool EnableItemType_Cape { get; set; } = true;
        public bool EnableItemType_Crossbow { get; set; } = true;
        public bool EnableItemType_HandArmor { get; set; } = true;
        public bool EnableItemType_HeadArmor { get; set; } = true;
        public bool EnableItemType_Horse { get; set; } = true;
        public bool EnableItemType_HorseHarness { get; set; } = true;
        public bool EnableItemType_LegArmor { get; set; } = true;
        public bool EnableItemType_OneHandedWeapon { get; set; } = true;
        public bool EnableItemType_Polearm { get; set; } = true;
        public bool EnableItemType_Shield { get; set; } = true;
        public bool EnableItemType_Thrown { get; set; } = true;
        public bool EnableItemType_TwoHandedWeapon { get; set; } = true;
              
        [XmlElement]
        [SettingProperty("Prizes Per Pool", "Tooltip")]
        public int NumberOfPrizeOptions { get; set; } = 3;
        
        public bool EnableItemModifiersForPrizes { get; set; } = false;

        #endregion Prize Selection

        #region Match Odds

        public bool OppenentDifficultyAffectsOdds { get; set; } = true;[XmlElement]
        [SettingProperty("Max Odds", 4f, 10f, "Maximum Odds for Tournament Bets")]
        public float MaximumBetOdds { get; set; } = 4;

        #endregion Match Odds

        #region Bonus Winnings

        [XmlElement]
        [SettingProperty("Bonus Gold Per Round", "Tooltip")]
        public int BonusTournamentMatchGold { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Payout Bonus Gold On Match Win", "Tooltip")]
        public bool BonusTournamentMatchGoldImmediate { get; set; } = false;

        [XmlElement]
        [SettingProperty("Bonus Gold for Tournament Win", "Tooltip")]
        public int BonusTournamentWinGold { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Bonus Renown for Tournament Win", "Tooltip")]
        public int BonusTournamentWinRenown { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Bonus Influence for Tournament Win",0f, 50f)]
        public float BonusTournamentWinInfluence { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Bonus Renown for Opponent Difficulty", "Tooltip")]
        public bool EnableRenownPerTroopTier { get; set; } = false;

        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier1 { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier2 { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier3 { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier4 { get; set; } = 2f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier5 { get; set; } = 2f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier6 { get; set; } = 2f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier7 { get; set; } = 3f;

        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownPerHeroPropertyHeroBase { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownPerHeroPropertyIsNoble { get; set; } = 3f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownPerHeroPropertyIsNotable { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownPerHeroPropertyIsCommander { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownPerHeroPropertyIsMinorFactionHero { get; set; } = 2f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownPerHeroPropertyIsMinorFactionLeader { get; set; } = 5f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownPerHeroPropertyIsMajorFactionLeader { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]

        
        #endregion Bonus Winnings

        #region Other

        public bool TournamentEquipmentFilter { get; set; }

        #endregion Other

        #region UnImplemented

        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;
        public bool CompanionsWinPrizes { get; set; } = false;

        #endregion UnImplemented

        #region XP Adjustments

        public bool IsTournamentXPEnabled { get; set; } = true;
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        public bool IsArenaXPEnabled { get; set; } = true;
        public float ArenaXPAdjustment { get; set; } = 1.0f;

        #endregion XP Adjustments


     
    }

    public enum PrizeListMode
    {
        Vanilla,
        Custom,
        TownVanilla,
        TownCustom,
        TownOnly,
    }

    public enum RenownHeroTier
    {
        HeroBase,
        IsNoble,
        IsNotable,
        IsCommander,
        IsMinorFactionHero,
        IsMinorFactionLeader,
        IsMajorFactionLeader,
    };
}