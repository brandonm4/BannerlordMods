using Newtonsoft.Json;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TournamentsXPanded.Common;

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

        public int PrizeValueMin { get; set; } = 1600;

        public int PrizeValueMax { get; set; } = 8500;

        public int PrizeValueMaxIncreasePerLevel { get; set; } = 500;

        public int PrizeValueMinIncreasePerLevel { get; set; } = 10;
        public bool TownProsperityAffectsPrizeValues { get; set; } = true;


        public bool PrizeFilterValueTownItems { get; set; } = true;
        public bool PrizeFilterValueCustomItems { get; set; } = false;
        public bool PrizeFilterValueStandardItems { get; set; } = true;
        public bool PrizeFilterValueLegacyItems { get; set; } = false;

        public bool PrizeFilterCultureTownItems { get; set; } = false;
        public bool PrizeFilterCultureCustomItems { get; set; } = false;
        public bool PrizeFilterCultureStandardItems { get; set; } = true;
        public bool PrizeFilterCultureLegacyItems { get; set; } = false;


        public bool PrizeFilterItemTypesTownItems { get; set; } = true;
        public bool PrizeFilterItemTypesCustomItems { get; set; } = false;
        public bool PrizeFilterItemTypesStandardItems { get; set; } = true;
        public bool PrizeFilterItemTypesLegacyItems { get; set; } = false;



        public float TownProsperityLow { get; set; } = .65f;

        public float TownProsperityMid { get; set; } = 1.0f;

        public float TownProsperityHigh { get; set; } = 1.3f;

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
        public bool EnableItemType_Arrow { get; set; } = true;
        public bool EnableItemType_Bolt { get; set; } = true;
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

        public float TournamentChanceMelee { get; set; } = 50f;
        public float TournamentChanceMelee2 { get; set; } = 50f;
        public float TournamentChanceJoust { get; set; } = 75f;

        #region UnImplemented

        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;

        public bool CompanionsWinPrizes { get; set; } = false;

        public bool EnableCleanSave { get; set; } = false;
        #endregion UnImplemented


        public List<ItemObject.ItemTypeEnum> GetActivePrizeTypes()
        {
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
        public float GetRenownValue(CharacterObject character)
        {
            var worth = 0f;
            if (character.IsHero)
            {
                worth += RenownPerHeroProperty[(int)RenownHeroTier.HeroBase];
                var hero = character.HeroObject;
                if (hero != null)
                {
                    if (hero.IsNoble)
                    {
                        worth += RenownPerHeroProperty[(int)RenownHeroTier.IsNoble];
                    }
                    if (hero.IsNotable)
                    {
                        worth += RenownPerHeroProperty[(int)RenownHeroTier.IsNotable];
                    }
                    if (hero.IsCommander)
                    {
                        worth += RenownPerHeroProperty[(int)RenownHeroTier.IsCommander];
                    }
                    if (hero.IsMinorFactionHero)
                    {
                        worth += RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                    }
                    if (hero.IsFactionLeader)
                    {
                        if (hero.MapFaction.IsKingdomFaction)
                        {
                            worth += RenownPerHeroProperty[(int)RenownHeroTier.IsMajorFactionLeader];
                        }

                        if (hero.MapFaction.IsMinorFaction)
                        {
                            worth += RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                        }
                    }
                }
            }
            else
            {
                worth += RenownPerTroopTier[character.Tier];
            }
            return worth;
        }

        public float[] RenownPerTroopTier
        {
            get
            {
                return new[] { 0f, TournamentXPSettings.Instance.RenownTroopTier1, TournamentXPSettings.Instance.RenownTroopTier2, TournamentXPSettings.Instance.RenownTroopTier3, TournamentXPSettings.Instance.RenownTroopTier4, TournamentXPSettings.Instance.RenownTroopTier5, TournamentXPSettings.Instance.RenownTroopTier6, TournamentXPSettings.Instance.RenownTroopTier7 };
            }
        }

        public float[] RenownPerHeroProperty
        {
            get
            {
                return new[] { TournamentXPSettings.Instance.RenownPerHeroPropertyHeroBase, TournamentXPSettings.Instance.RenownPerHeroPropertyIsNoble, TournamentXPSettings.Instance.RenownPerHeroPropertyIsNotable, TournamentXPSettings.Instance.RenownPerHeroPropertyIsCommander, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMinorFactionHero, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMinorFactionLeader, TournamentXPSettings.Instance.RenownPerHeroPropertyIsMajorFactionLeader };
            }
        }
    }
}