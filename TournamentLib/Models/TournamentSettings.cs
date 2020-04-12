using ModLib;
using ModLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.Core;

namespace TournamentLib.Models
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
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool EnableConfigReloadRealTime { get; set; } = false;
        #endregion

        #region Re-Roll
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool TournamentPrizeRerollEnabled { get; set; } = false;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;
        #endregion

        #region Prize Selection
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool EnablePrizeSelection { get; set; } = true;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public PrizeListMode PrizeListMode { get; set; } = PrizeListMode.TownCustom;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public int TownPrizeMin { get; set; } = 1000;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public int TownPrizeMax { get; set; } = 5000;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell { get; set; } = false;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool EnablePrizeTypeFilterToLists { get; set; } = false;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public List<ItemObject.ItemTypeEnum> TownValidPrizeTypes { get; set; } = new List<ItemObject.ItemTypeEnum>();
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public List<string> CustomTourneyItems { get; set; }
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public string CustomPrizeFileName { get; set; } = "";
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public int NumberOfPrizeOptions { get; set; } = 3;
        #endregion

        #region Match Odds
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool OppenentDifficultyAffectsOdds { get; set; } = true;[XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public float MaximumBetOdds { get; set; } = 3;
        #endregion

        #region Bonus Winnings
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public int BonusTournamentMatchGold { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public int BonusTournamentWinGold { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool BonusTournamentMatchGoldImmediate { get; set; } = false;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public int BonusTournamentWinRenown { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public float BonusTournamentWinInfluence { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool EnableRenownPerTroopTier { get; set; } = false;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public List<float> RenownPerTroopTier { get; set; } = new List<float>() { 0, 1, 1, 1, 2, 2, 2, 3 };
        /* Base, IsNoble, IsNotable, IsCommander, IsMinorFactionLeader, IsMajorFactionLeader */
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public List<float> RenownPerHeroProperty { get; set; } = new List<float>() { 1, 3, 1, 1, 2, 5, 10 };
        #endregion

        #region UnImplemented
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Label", "Tooltip")]
        public bool CompanionsWinPrizes { get; set; } = false;
        #endregion




    }
}
