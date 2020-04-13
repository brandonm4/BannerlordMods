using ModLib;
using ModLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.Core;

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
        #endregion

        #region Re-Roll        
        [XmlElement]
        [SettingProperty("Re-rolls per Tournament", "Tooltip")]
        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;
        #endregion

        #region Prize Selection
        [XmlElement]
        [SettingProperty("Prize Selection", "Tooltip")]
        public bool EnablePrizeSelection { get; set; } = true;
        [XmlElement]
        [SettingProperty("Prize List Mode", "Tooltip")]
        public PrizeListMode PrizeListMode { get; set; } = PrizeListMode.TownCustom;
        [XmlElement]
        [SettingProperty("Town Prize Min Value", "Tooltip")]
        public int TownPrizeMin { get; set; } = 1000;
        [XmlElement]
        [SettingProperty("Town Prize Max Value", "Tooltip")]
        public int TownPrizeMax { get; set; } = 5000;
        [XmlElement]
        [SettingProperty("Prize Value Filter", "Tooltip")]
        public bool TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell { get; set; } = false;
        [XmlElement]
        [SettingProperty("Prize Type Filter", "Tooltip")]
        public bool EnablePrizeTypeFilterToLists { get; set; } = false;
        [XmlElement]
        [SettingProperty("Valid Prize Types", "Tooltip")]
        public List<ItemObject.ItemTypeEnum> TownValidPrizeTypes { get; set; } = new List<ItemObject.ItemTypeEnum>();


        [XmlElement]
        [SettingProperty("Custom Prize Pool Filename", "Tooltip")]
        public string CustomPrizeFileName { get; set; } = "";
        [XmlElement]
        [SettingProperty("Prizes Per Pool", "Tooltip")]
        public int NumberOfPrizeOptions { get; set; } = 3;

        public List<string> CustomTourneyItems { get; set; }

        public bool EnableItemModifiersForPrizes { get; set; } = false;
        #endregion

        #region Match Odds       
        public bool OppenentDifficultyAffectsOdds { get; set; } = true;[XmlElement]
        [SettingProperty("Max Odds", "Tooltip")]
        public float MaximumBetOdds { get; set; } = 4;
        #endregion

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
        [SettingProperty("Bonus Influence for Tournament Win", "Tooltip")]
        public float BonusTournamentWinInfluence { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Bonus Renown for Opponent Difficulty", "Tooltip")]
        public bool EnableRenownPerTroopTier { get; set; } = false;
         
        public float RenownTroopTier1 { get; set; } = 1f;
        public float RenownTroopTier2 { get; set; } = 1f;
        public float RenownTroopTier3 { get; set; } = 1f;
        public float RenownTroopTier4 { get; set; } = 2f;
        public float RenownTroopTier5 { get; set; } = 2f;
        public float RenownTroopTier6 { get; set; } = 2f;
        public float RenownTroopTier7 { get; set; } = 3f;

        public float RenownPerHeroPropertyHeroBase { get; set; } = 1f;
        public float RenownPerHeroPropertyIsNoble { get; set; } = 3f;
        public float RenownPerHeroPropertyIsNotable { get; set; } = 1f;
        public float RenownPerHeroPropertyIsCommander { get; set; } = 1f;
        public float RenownPerHeroPropertyIsMinorFactionHero { get; set; } = 2f;
        public float RenownPerHeroPropertyIsMinorFactionLeader { get; set; } = 5f;
        public float RenownPerHeroPropertyIsMajorFactionLeader { get; set; } = 10f;

        public float[] RenownPerTroopTier
        {
            get
            {
                return new[] { 0f, RenownTroopTier1, RenownTroopTier2, RenownTroopTier3, RenownTroopTier4, RenownTroopTier5, RenownTroopTier6, RenownTroopTier7 };
            }
        }
        public float[] RenownPerHeroProperty
        {
            get
            {
                return new[] { RenownPerHeroPropertyHeroBase, RenownPerHeroPropertyIsNoble, RenownPerHeroPropertyIsNotable, RenownPerHeroPropertyIsCommander, RenownPerHeroPropertyIsMinorFactionHero, RenownPerHeroPropertyIsMinorFactionLeader, RenownPerHeroPropertyIsMajorFactionLeader };
            }
        }



        #endregion

        #region Other
        public bool TournamentEquipmentFilter { get; set; }
        #endregion

        #region UnImplemented

        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;
        public bool CompanionsWinPrizes { get; set; } = false;
        #endregion

        #region XP Adjustments
        public bool IsTournamentXPEnabled { get; set; } = true;
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        public bool IsArenaXPEnabled { get; set; } = true;
        public float ArenaXPAdjustment { get; set; } = 1.0f;
        #endregion


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
