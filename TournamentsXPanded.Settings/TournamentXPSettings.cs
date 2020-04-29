
using Newtonsoft.Json;
using TournamentsXPanded.Settings.Attributes;

namespace TournamentsXPanded.Models
{
    public class TournamentXPSettings
    {
        private static TournamentXPSettings _instance;

        public static TournamentXPSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TournamentXPSettings();
                }
                return _instance;
            }
        }

        public static void SetSettings(TournamentXPSettings settings)
        {
            _instance = settings;
        }

        #region XP Adjustments
        [SettingPropertyGroup("{=txpg0001}")]
        [SettingProperty("{=txpset01}")]
        public bool IsTournamentXPEnabled { get; set; } = true;
        [SettingPropertyGroup("{=txpg0001}")]
        [SettingProperty("{=txpd0015}", 0, 10f)]
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        [SettingPropertyGroup("{=txpg0001}")]
        [SettingProperty("{=txpd0016}")]
        public bool IsArenaXPEnabled { get; set; } = true;
        [SettingPropertyGroup("{=txpg0001}")]
        [SettingProperty("{=txpd0017}", 0, 10f)]
        public float ArenaXPAdjustment { get; set; } = 1.0f;

        #endregion

        #region Other


        [SettingPropertyGroup("{=txpg0001}")]
        [SettingProperty("{=txpd0018}", "{=txph0096}")]
        public bool TournamentEquipmentFilter { get; set; }

        #endregion Other

        #region Re-Roll


        [SettingProperty("{=txpd0019}", 0, 99, "{=txph0097}")]
        [SettingPropertyGroup("{=txpg0002}")]
        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;

        #endregion Re-Roll

        #region Prize Selection


        [SettingProperty("{=txpd0020}", "{=txph0098}")]
        [SettingPropertyGroup("{=txpg0003}")]
        public bool EnablePrizeSelection { get; set; } = true;


        [SettingPropertyGroup("{=txpg0003}")]
        [SettingProperty("{=txpd0021}", 0, 10)]
        public int NumberOfPrizeOptions { get; set; } = 3;

        #region {=txpg0004}

        [SettingPropertyGroup("{=txpg0004}")]
        [SettingProperty("{=txpd0022}", "{=txph0099}")]
        public bool PrizeListIncludeTown { get; set; } = true;


        [SettingPropertyGroup("{=txpg0004}")]
        [SettingProperty("{=txpd0023}", "{=txph0100}")]
        public bool PrizeListIncludeVanilla { get; set; } = true;


        [SettingPropertyGroup("{=txpg0004}")]
        [SettingProperty("{=txpd0024}", "{=txph0101}")]
        public bool PrizeListIncludeCustom { get; set; } = false;


        [SettingPropertyGroup("{=txpg0004}")]
        [SettingProperty("{=txpd0025}", "{=txph0102}")]
        public bool PrizeListIncludeLegacy { get; set; } = false;
        #endregion
        #region {=txpg0005}

        [SettingProperty("{=txpd0026}", 0, 10000, "{=txph0103}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public int PrizeValueMin { get; set; } = 1600;


        [SettingProperty("{=txpd0027}", 0, 300000, "{=txph0104}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public int PrizeValueMax { get; set; } = 8500;


        [SettingProperty("{=txpd0028}", 0, 10000, "{=txph0105}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public int PrizeValueMaxIncreasePerLevel { get; set; } = 500;


        [SettingProperty("{=txpd0029}", 0, 10000, "{=txph0106}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public int PrizeValueMinIncreasePerLevel { get; set; } = 10;


        [SettingProperty("{=txpd0030}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public bool PrizeFilterValueTownItems { get; set; } = true;


        [SettingProperty("{=txpd0031}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public bool PrizeFilterValueCustomItems { get; set; } = false;


        [SettingProperty("{=txpd0032}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public bool PrizeFilterValueStandardItems { get; set; } = true;


        [SettingProperty("{=txpd0033}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public bool PrizeFilterValueLegacyItems { get; set; } = true;


        [SettingProperty("{=txpd0034}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public bool TownProsperityAffectsPrizeValues { get; set; } = true;

        [SettingProperty("{=txpd0035}", 0, 1f, "{=txph0107}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public float TownProsperityLow { get; set; } = .65f;


        [SettingProperty("{=txpd0036}", 0, 2f, "{=txph0108}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public float TownProsperityMid { get; set; } = 1.0f;


        [SettingProperty("{=txpd0037}", 0, 3f, "{=txph0109}")]
        [SettingPropertyGroup("{=txpg0005}")]
        public float TownProsperityHigh { get; set; } = 1.3f;
        #endregion



        #region 3.4 Prize Types Filter

        [SettingProperty("{=txpd0038}")]
        [SettingPropertyGroup("{=txpg0006}")]
        public bool PrizeFilterItemTypesTownItems { get; set; } = true;

        [SettingProperty("{=txpd0039}")]
        [SettingPropertyGroup("{=txpg0006}")]
        public bool PrizeFilterItemTypesCustomItems { get; set; } = false;

        [SettingProperty("{=txpd0040}")]
        [SettingPropertyGroup("{=txpg0006}")]
        public bool PrizeFilterItemTypesStandardItems { get; set; } = true;

        [SettingProperty("{=txpd0041}")]
        [SettingPropertyGroup("{=txpg0006}")]
        public bool PrizeFilterItemTypesLegacyItems { get; set; } = false;



        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0042}")]
        public bool EnableItemType_Bow { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0043}")]
        public bool EnableItemType_Crossbow { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0044}")]
        public bool EnableItemType_OneHandedWeapon { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0045}")]
        public bool EnableItemType_Polearm { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0046}")]
        public bool EnableItemType_Shield { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0047}")]
        public bool EnableItemType_Thrown { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0048}")]
        public bool EnableItemType_TwoHandedWeapon { get; set; } = true;

        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0049}")]
        public bool EnableItemType_Arrow { get; set; } = true;

        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0050}")]
        public bool EnableItemType_Bolt { get; set; } = true;

        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0051}")]
        public bool EnableItemType_HeadArmor { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0052}")]
        public bool EnableItemType_HandArmor { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0053}")]
        public bool EnableItemType_BodyArmor { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0054}")]
        public bool EnableItemType_LegArmor { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0055}")]
        public bool EnableItemType_Cape { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0056}")]
        public bool EnableItemType_Horse { get; set; } = true;


        [SettingPropertyGroup("{=txpg0006}")]
        [SettingProperty("{=txpd0057}")]
        public bool EnableItemType_HorseHarness { get; set; } = true;
        #endregion
        #region 3.5 Prize Filter on Culture

        [SettingProperty("{=txpd0058}", "{=txph0113}")]
        [SettingPropertyGroup("{=txpg0007}")]
        public bool PrizeFilterCultureTownItems { get; set; } = false;

        [SettingProperty("{=txpd0059}", "{=txph0113}")]
        [SettingPropertyGroup("{=txpg0007}")]
        public bool PrizeFilterCultureCustomItems { get; set; } = false;

        [SettingProperty("{=txpd0060}", "{=txph0113}")]
        [SettingPropertyGroup("{=txpg0007}")]
        public bool PrizeFilterCultureStandardItems { get; set; } = true;

        [SettingProperty("{=txpd0061}", "{=txph0113}")]
        [SettingPropertyGroup("{=txpg0007}")]
        public bool PrizeFilterCultureLegacyItems { get; set; } = false;

        #endregion


        #region Bonus Winnings


        [SettingPropertyGroup("{=txpg0008}")]
        [SettingProperty("{=txpd0062}", 0, 20000)]
        public int BonusTournamentWinGold { get; set; } = 0;

        [SettingPropertyGroup("{=txpg0008}")]
        [SettingProperty("{=txpd0063}", 0, 100)]
        public int BonusTournamentWinRenown { get; set; } = 0;


        [SettingPropertyGroup("{=txpg0008}")]
        [SettingProperty("{=txpd0064}", 0f, 50f)]
        public float BonusTournamentWinInfluence { get; set; } = 0;




        [SettingPropertyGroup("{=txpg0009}")]
        [SettingProperty("{=txpd0065}", 0, 1000)]
        public int BonusTournamentMatchGold { get; set; } = 0;


        [SettingPropertyGroup("{=txpg0009}")]
        [SettingProperty("{=txpd0066}", 0, 20000, "{=txph0114}")]
        public bool BonusTournamentMatchGoldImmediate { get; set; } = false;


        [SettingPropertyGroup("{=txpg0009}")]
        [SettingProperty("{=txpd0067}", 0f, 5f)]
        public float BonusRenownMostKills { get; set; } = 0f;

        [SettingPropertyGroup("{=txpg0009}")]
        [SettingProperty("{=txpd0068}", 0f, 5f)]
        public float BonusRenownMostDamage { get; set; } = 0f;

        [SettingPropertyGroup("{=txpg0009}")]
        [SettingProperty("{=txpd0069}", 0f, 5f)]
        public float BonusRenownFirstKill { get; set; } = 0f;

        [SettingPropertyGroup("{=txpg0009}")]
        [SettingProperty("{=txpd0070}", 0f, 5f)]
        public float BonusRenownLeastDamage { get; set; } = 0f;




        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0071}")]
        public bool EnableRenownPerTroopTier { get; set; } = false;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0072}", 0f, 50f)]
        public float RenownTroopTier1 { get; set; } = .15f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0073}", 0f, 50f)]
        public float RenownTroopTier2 { get; set; } = .25f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0074}", 0f, 50f)]
        public float RenownTroopTier3 { get; set; } = .35f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0075}", 0f, 50f)]
        public float RenownTroopTier4 { get; set; } = .45f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0076}", 0f, 50f)]
        public float RenownTroopTier5 { get; set; } = .55f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0077}", 0f, 50f)]
        public float RenownTroopTier6 { get; set; } = .65f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0078}", 0f, 50f)]
        public float RenownTroopTier7 { get; set; } = .75f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0079}", 0f, 50f)]
        public float RenownPerHeroPropertyHeroBase { get; set; } = 1f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0080}", 0f, 50f)]
        public float RenownPerHeroPropertyIsNoble { get; set; } = 3f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0081}", 0f, 50f)]
        public float RenownPerHeroPropertyIsNotable { get; set; } = 1f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0082}", 0f, 50f)]
        public float RenownPerHeroPropertyIsCommander { get; set; } = 1f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0083}", 0f, 50f)]
        public float RenownPerHeroPropertyIsMinorFactionHero { get; set; } = 2f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0084}", 0f, 50f)]
        public float RenownPerHeroPropertyIsMinorFactionLeader { get; set; } = 5f;


        [SettingPropertyGroup("{=txpg0010}")]
        [SettingProperty("{=txpd0085}", 0f, 50f)]
        public float RenownPerHeroPropertyIsMajorFactionLeader { get; set; } = 10f;

        #endregion Bonus Winnings

        [SettingPropertyGroup("{=txpg0011}")]
        [SettingProperty("{=txpd0086}", "{=txph0115}")]
        public bool EnableItemModifiersForPrizes { get; set; } = false;
        [SettingPropertyGroup("{=txpg0011}")]
        [SettingProperty("{=txpd0087}", "{=txph0116}")]
        public bool TownProsperityAffectsItemModifiers { get; set; } = false;
        public bool EnableCleanSave { get; set; } = false;
        [SettingPropertyGroup("{=txpg0011}")]
        [SettingProperty("{=txpd0088}", "{=txph0117}")]
        public bool PrizeFilterPreventPlayerCraftedItems { get; set; } = false;

        #endregion Prize Selection

        #region Match Odds        
        [SettingPropertyGroup("{=txpg0011}")]
        [SettingProperty("{=txpd0089}", "{=txph0118}")]
        public bool OppenentDifficultyAffectsOdds { get; set; } = true;
        [SettingPropertyGroup("{=txpg0011}")]
        [SettingProperty("{=txpd0090}", 4f, 10f, "{=txph0119}")]
        public float MaximumBetOdds { get; set; } = 4;

        #endregion Match Odds

        #region UnImplemented


        [SettingPropertyGroup("{=txpg0012}")]
        [SettingProperty("{=txpd0091}", 0, 1)]
        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;
        [SettingPropertyGroup("{=txpg0012}")]
        [SettingProperty("{=txpd0092}")]
        public bool CompanionsWinPrizes { get; set; } = false;
        [SettingPropertyGroup("{=txpg0012}")]
        [SettingProperty("{=txpd0093}", "{=txph0120}")]
        public bool EnableTournamentRandomSelection { get; set; } = false;
        [SettingPropertyGroup("{=txpg0012}")]
        [SettingProperty("{=txpd0094}")]
        public bool EnableTournamentTypeSelection { get; set; } = true;

        #endregion
        [SettingPropertyGroup("{=txpg0013}")]
        [SettingProperty("{=txpd0095}")]
        public bool DebugMode { get; set; } = false;

        [JsonIgnore]
        public float[] RenownPerTroopTier
        {
            get
            {
                return new[] { 0f, TournamentXPSettings.Instance.RenownTroopTier1, TournamentXPSettings.Instance.RenownTroopTier2, TournamentXPSettings.Instance.RenownTroopTier3, TournamentXPSettings.Instance.RenownTroopTier4, TournamentXPSettings.Instance.RenownTroopTier5, TournamentXPSettings.Instance.RenownTroopTier6, TournamentXPSettings.Instance.RenownTroopTier7 };
            }
        }

        [JsonIgnore]
        public float[] RenownPerHeroProperty
        {
            get
            {
                return new[] { TournamentXPSettings.Instance.RenownPerHeroPropertyHeroBase, TournamentXPSettings.Instance.RenownPerHeroPropertyIsNoble, TournamentXPSettings.Instance.RenownPerHeroPropertyIsNotable, TournamentXPSettings.Instance.RenownPerHeroPropertyIsCommander, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMinorFactionHero, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMinorFactionLeader, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMajorFactionLeader };
            }
        }
    }
}
