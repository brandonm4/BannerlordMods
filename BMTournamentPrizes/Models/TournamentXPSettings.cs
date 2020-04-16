

using System.Collections.Generic;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using ModLib;
using ModLib.Attributes;

namespace TournamentsXPanded.Models
{
    public class TournamentXPSettings : SettingsBase
    {
        private const string instanceID = "TournamentXPSettings";
        private static TournamentXPSettings _instance = null;
        public override string ModName => "Tournaments XPanded";
        public override string ModuleFolderName => TournamentsXPandedSubModule.ModuleFolderName;

        public static void SetSettings(TournamentXPSettings newSettings)
        {
            _instance = newSettings;
        }

        public static TournamentXPSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                   // _instance = FileDatabase.Get<TournamentXPSettings>(instanceID);
                    if (_instance == null)
                    {
                        _instance = new TournamentXPSettings();
                        _instance.ID = instanceID;

                      //  SettingsDatabase.SaveSettings(_instance);
                        //FileDatabase.TXPSaveToFile(_instance.ModuleFolderName, _instance);
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
        [SettingProperty("Re-rolls per Tournament",0, 99, "Maximum number of times you can re-roll the prize pool per tournament.  Set to 0 to disable.")]
        [SettingPropertyGroup("Re-Roll")]
        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;

        #endregion Re-Roll

        #region Prize Selection

        [XmlElement]
        [SettingProperty("Prize Selection", "Should the Select Prize from the Prize Pool be enable. ")]
        public bool EnablePrizeSelection { get; set; } = true;

        //[XmlElement]
        //[SettingProperty("Prize List Mode", "Where do tournaments get prizes from:\nVanilla is standard behavior\nCustom is from a custom list you provide.\nTownCustom,TownVanilla,TownOnly will also pull items from the Town's stock.")]
        //public int PrizeListMode { get; set; } = 0;

        public bool PrizeListIncludeTown { get; set; } = true;
        public bool PrizeListIncludeVanilla { get; set; } = true;
        public bool PrizeListIncludeCustom { get; set; } = false;
        public bool PrizeListIncludeLegacy { get; set; } = true;

        [XmlElement]
        [SettingProperty("Town Prize Min Value",0, 10000, "Any item below this value will not be used.")]
        public int TownPrizeMin { get; set; } = 1600;

        [XmlElement]
        [SettingProperty("Town Prize Max Value", 0, 300000,"Any item above this value will not be used.")]
        public int TownPrizeMax { get; set; } = 8500;

        [XmlElement]
        [SettingProperty("Prize Value Per Level", 0, 10000)]
        public int PrizeValueMaxIncreasePerLevel { get; set; } = 500;
        [XmlElement]
        [SettingProperty("Prize Value Per Level", 0, 10000)]
        public int PrizeValueMinIncreasePerLevel { get; set; } = 10;


        [XmlElement]
        [SettingProperty("Prize Value Filter", "Tooltip")]
        public bool TownPrizeMinMaxAffectsVanilla { get; set; }
        public bool TownPrizeMinMaxAffectsCustom { get; set; }
        [XmlElement]
        [SettingProperty("Prize Value Filter", "Tooltip")]
        public bool TownProsperityAffectsPrizeValues { get; set; } = true;
        [XmlElement]
        [SettingProperty("Town Prosperity: Low", 0, 10f)]
        public float TownProsperityLow { get; set; } = .65f;
        [XmlElement]
        [SettingProperty("Town Prosperity: Mid", 0, 10f)]
        public float TownProsperityMid { get; set; } = 1.0f;
        [XmlElement]
        [SettingProperty("Town Prosperity: High", 0, 10f)]
        public float TownProsperityHigh { get; set; } = 1.3f;


        [XmlElement]
        [SettingProperty("Prize Type Filter", "Tooltip")]
        public bool EnablePrizeTypeFilterToLists { get; set; } = false;


        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_BodyArmor { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_Bow { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_Cape { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_Crossbow { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_HandArmor { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_HeadArmor { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_Horse { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_HorseHarness { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_LegArmor { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_OneHandedWeapon { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_Polearm { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_Shield { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_Thrown { get; set; } = true;
        [XmlElement]
        [SettingProperty("Allow ItemType BodyArmor", "Tooltip")]
        public bool EnableItemType_TwoHandedWeapon { get; set; } = true;
              
        [XmlElement]
        [SettingProperty("Prizes Per Pool", 0, 10, "Tooltip")]
        public int NumberOfPrizeOptions { get; set; } = 3;
        [XmlElement]
        [SettingProperty("EnableItemModifiersForPrizes", "Tooltip")]
        public bool EnableItemModifiersForPrizes { get; set; } = false;

        #endregion Prize Selection

        #region Match Odds
        [XmlElement]
        [SettingProperty("OppenentDifficultyAffectsOdds", "Not working in 1.5-beta")]
        public bool OppenentDifficultyAffectsOdds { get; set; } = true;
        [XmlElement]
        [SettingProperty("Max Odds", 4f, 10f, "Maximum Odds for Tournament Bets - not working in 1.5-beta")]
        public float MaximumBetOdds { get; set; } = 4;

        #endregion Match Odds

        #region Bonus Winnings

        [XmlElement]
        [SettingProperty("Bonus Gold Per Round", 0, 1000, "Tooltip")]
        public int BonusTournamentMatchGold { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Payout Bonus Gold On Match Win", 0, 20000, "Tooltip")]
        public bool BonusTournamentMatchGoldImmediate { get; set; } = false;

        [XmlElement]
        [SettingProperty("Bonus Gold for Tournament Win", 0, 20000, "Tooltip")]
        public int BonusTournamentWinGold { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Bonus Renown for Tournament Win", 0, 100, "Tooltip")]
        public int BonusTournamentWinRenown { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Bonus Influence for Tournament Win",0f, 50f)]
        public float BonusTournamentWinInfluence { get; set; } = 0;

        [XmlElement]
        [SettingProperty("Bonus Renown for Opponent Difficulty", "Tooltip")]
        public bool EnableRenownPerTroopTier { get; set; } = false;

        [XmlElement]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier1 { get; set; } = .15f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 2", 0f, 50f)]
        public float RenownTroopTier2 { get; set; } = .25f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 3", 0f, 50f)]
        public float RenownTroopTier3 { get; set; } = .35f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 4", 0f, 50f)]
        public float RenownTroopTier4 { get; set; } = .45f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 5", 0f, 50f)]
        public float RenownTroopTier5 { get; set; } = .55f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 6", 0f, 50f)]
        public float RenownTroopTier6 { get; set; } = .65f;
        [XmlElement]
        [SettingProperty("Bonus Renown Tier 7", 0f, 50f)]
        public float RenownTroopTier7 { get; set; } = .75f;

        [XmlElement]
        [SettingProperty("Bonus Renown Hero Base", 0f, 50f)]
        public float RenownPerHeroPropertyHeroBase { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown IsNoble", 0f, 50f)]
        public float RenownPerHeroPropertyIsNoble { get; set; } = 3f;
        [XmlElement]
        [SettingProperty("Bonus Renown IsBotable", 0f, 50f)]
        public float RenownPerHeroPropertyIsNotable { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown IsCommander", 0f, 50f)]
        public float RenownPerHeroPropertyIsCommander { get; set; } = 1f;
        [XmlElement]
        [SettingProperty("Bonus Renown Minor Faction Hero", 0f, 50f)]
        public float RenownPerHeroPropertyIsMinorFactionHero { get; set; } = 2f;
        [XmlElement]
        [SettingProperty("Bonus Renown Minor Faction Leader", 0f, 50f)]
        public float RenownPerHeroPropertyIsMinorFactionLeader { get; set; } = 5f;
        [XmlElement]
        [SettingProperty("Bonus Renown Major Faction Leader", 0f, 50f)]
        public float RenownPerHeroPropertyIsMajorFactionLeader { get; set; } = 10f;
        
        #endregion Bonus Winnings

        #region Other
        [XmlElement]
        [SettingProperty("TournamentEquipmentFilter", "Removes spears from tournament footmen")]
        public bool TournamentEquipmentFilter { get; set; }

        #endregion Other

        #region UnImplemented

        [XmlElement]
        [SettingProperty("TournamentBonusMoneyBaseNamedCharLevel", 0 ,1)]
        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;
        [XmlElement]
        [SettingProperty("CompanionsWinPrizes(Not Active)")]
        public bool CompanionsWinPrizes { get; set; } = false;

        #endregion UnImplemented

        #region XP Adjustments
        [XmlElement]
        [SettingProperty("Enable Tournament XP")]
        public bool IsTournamentXPEnabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("TournamentXPAdjustment", 0, 10f)]
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        [XmlElement]
        [SettingProperty("Enable Arena XP")]
        public bool IsArenaXPEnabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("ArenaXPAdjustment", 0, 10f)]
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