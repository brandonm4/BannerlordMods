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

        public bool IsTournamentXPEnabled { get; set; } = true;

        public float TournamentXPAdjustment { get; set; } = 1.0f;

        public bool IsArenaXPEnabled { get; set; } = true;

        public float ArenaXPAdjustment { get; set; } = 1.0f;

        #endregion XP Adjustments

        #region Other

        public bool TournamentEquipmentFilter { get; set; }

        #endregion Other

        #region Re-Roll

        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;

        #endregion Re-Roll

        #region Prize Selection

        public bool EnablePrizeSelection { get; set; } = true;

        public int NumberOfPrizeOptions { get; set; } = 3;

        public bool PrizeListIncludeTown { get; set; } = true;

        public bool PrizeListIncludeVanilla { get; set; } = true;

        public bool PrizeListIncludeCustom { get; set; } = false;

        public bool PrizeListIncludeLegacy { get; set; } = true;

        public int TownPrizeMin { get; set; } = 1600;

        public int TownPrizeMax { get; set; } = 8500;

        public int PrizeValueMaxIncreasePerLevel { get; set; } = 500;

        public int PrizeValueMinIncreasePerLevel { get; set; } = 10;

        public bool TownPrizeMinMaxAffectsVanilla { get; set; }

        public bool TownPrizeMinMaxAffectsCustom { get; set; }

        public bool TownProsperityAffectsPrizeValues { get; set; } = true;

        public float TownProsperityLow { get; set; } = .65f;

        public float TownProsperityMid { get; set; } = 1.0f;

        public float TownProsperityHigh { get; set; } = 1.3f;

        public bool EnablePrizeTypeFilterToLists { get; set; } = false;

        public bool EnableItemType_Bow { get; set; } = true;

        public bool EnableItemType_Crossbow { get; set; } = true;

        public bool EnableItemType_OneHandedWeapon { get; set; } = true;

        public bool EnableItemType_Polearm { get; set; } = true;

        public bool EnableItemType_Shield { get; set; } = true;

        public bool EnableItemType_Thrown { get; set; } = true;

        public bool EnableItemType_TwoHandedWeapon { get; set; } = true;

        public bool EnableItemType_HeadArmor { get; set; } = true;

        public bool EnableItemType_HandArmor { get; set; } = true;

        public bool EnableItemType_BodyArmor { get; set; } = true;

        public bool EnableItemType_LegArmor { get; set; } = true;

        public bool EnableItemType_Cape { get; set; } = true;

        public bool EnableItemType_Horse { get; set; } = true;

        public bool EnableItemType_HorseHarness { get; set; } = true;

        #region Bonus Winnings

        public int BonusTournamentMatchGold { get; set; } = 0;

        public bool BonusTournamentMatchGoldImmediate { get; set; } = false;

        public int BonusTournamentWinGold { get; set; } = 0;

        public int BonusTournamentWinRenown { get; set; } = 0;

        public float BonusTournamentWinInfluence { get; set; } = 0;

        public bool EnableRenownPerTroopTier { get; set; } = false;

        public float RenownTroopTier1 { get; set; } = .15f;

        public float RenownTroopTier2 { get; set; } = .25f;

        public float RenownTroopTier3 { get; set; } = .35f;

        public float RenownTroopTier4 { get; set; } = .45f;

        public float RenownTroopTier5 { get; set; } = .55f;

        public float RenownTroopTier6 { get; set; } = .65f;

        public float RenownTroopTier7 { get; set; } = .75f;

        public float RenownPerHeroPropertyHeroBase { get; set; } = 1f;

        public float RenownPerHeroPropertyIsNoble { get; set; } = 3f;

        public float RenownPerHeroPropertyIsNotable { get; set; } = 1f;

        public float RenownPerHeroPropertyIsCommander { get; set; } = 1f;

        public float RenownPerHeroPropertyIsMinorFactionHero { get; set; } = 2f;

        public float RenownPerHeroPropertyIsMinorFactionLeader { get; set; } = 5f;

        public float RenownPerHeroPropertyIsMajorFactionLeader { get; set; } = 10f;

        #endregion Bonus Winnings

        public bool EnableItemModifiersForPrizes { get; set; } = false;
        public bool TownProsperityAffectsItemModifiers { get; set; } = false;


        public float BonusRenownMostKills { get; set; } = 0f;
        public float BonusRenownMostDamage { get; set; } = 0f;
        public float BonusRenownFirstKill { get; set; } = 0f;
        public float BonusRenownLeastDamage { get; set; } = 0f;


        #endregion Prize Selection

        #region Match Odds

        public bool OppenentDifficultyAffectsOdds { get; set; } = true;

        public float MaximumBetOdds { get; set; } = 4;

        #endregion Match Odds

        public bool EnableTournamentTypeSelection { get; set; } = true;
        public bool EnableTournamentRandomSelection { get; set; } = true;
        public bool DebugMode { get; set; } = false;

        #region UnImplemented

        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;

        public bool CompanionsWinPrizes { get; set; } = false;

        #endregion UnImplemented
    }
}