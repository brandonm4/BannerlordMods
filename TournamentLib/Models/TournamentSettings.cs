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
        public bool DebugMode { get; set; } = false;
        #endregion

        #region Re-Roll
        [XmlElement]
        [SettingProperty("Prize Re-roll", "Tooltip")]
        public bool TournamentPrizeRerollEnabled { get; set; } = false;
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


        public List<float> RenownPerTroopTier { get; set; } = new List<float>() { 0, 1, 1, 1, 2, 2, 2, 3 };
        /* Base, IsNoble, IsNotable, IsCommander, IsMinorFactionLeader, IsMajorFactionLeader */
        public List<float> RenownPerHeroProperty { get; set; } = new List<float>() { 1, 3, 1, 1, 2, 5, 10 };
        #endregion

        #region UnImplemented
      
        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;      
        public bool CompanionsWinPrizes { get; set; } = false;
        #endregion



        
    }
}
