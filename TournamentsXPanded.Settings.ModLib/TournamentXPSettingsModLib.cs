using ModLib;
using ModLib.Attributes;
using System;
using System.Reflection;
using System.Xml.Serialization;

using TournamentsXPanded.Settings;
using XPanded.Common.Diagnostics;

namespace TournamentsXPanded.Models
{
    public class TournamentXPSettingsModLib : SettingsBase
    {
        public const string InstanceID = "TournamentXPSettings";
        public override string ModName => "Tournaments XPanded";
        public override string ModuleFolderName => SettingsHelper.ModuleFolderName;

        public static TournamentXPSettingsModLib Instance
        {
            get
            {
                return (TournamentXPSettingsModLib)SettingsDatabase.GetSettings(InstanceID);
            }
        }

        public TournamentXPSettings GetSettings()
        {
            //var config = new MapperConfiguration(cfg => cfg.CreateMap<TournamentXPSettingsModLib, TournamentXPSettings>());
            //var mapper = new Mapper(config);
            //TournamentXPSettings dto = mapper.Map<TournamentXPSettings>(Instance);
            TournamentXPSettings dto = new TournamentXPSettings();
            PropertyInfo[] propertiesML = typeof(TournamentXPSettingsModLib).GetProperties();
            foreach (PropertyInfo pTXP in typeof(TournamentXPSettings).GetProperties())
            {
                foreach (PropertyInfo pML in propertiesML)
                {
                    try
                    {
                        if (pTXP.Name == pML.Name && pML.Name != "Instance")
                        {
                            pTXP.SetValue(dto, pML.GetValue(TournamentXPSettingsModLib.Instance));
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("Error in assigning ModLib property to TXPSettings: " + pTXP.Name + "\n" + ex.ToStringFull());
                    }
                }
            }
            return dto;
        }

        [XmlElement]
        public override string ID { get; set; } = InstanceID;

        #region Debug Settings

        public bool DebugMode { get; set; } = false;

        #endregion Debug Settings

        #region XP Adjustments

        [XmlElement]
        [SettingPropertyGroup("1. Tournament Configuration")]
        [SettingProperty("Enable Tournament XP")]
        public bool IsTournamentXPEnabled { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("1. Tournament Configuration")]
        [SettingProperty("TournamentXPAdjustment", 0, 10f)]
        public float TournamentXPAdjustment { get; set; } = 1.0f;

        [XmlElement]
        [SettingPropertyGroup("1. Tournament Configuration")]
        [SettingProperty("Enable Arena XP")]
        public bool IsArenaXPEnabled { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("1. Tournament Configuration")]
        [SettingProperty("ArenaXPAdjustment", 0, 10f)]
        public float ArenaXPAdjustment { get; set; } = 1.0f;

        #endregion XP Adjustments

        #region Other

        [XmlElement]
        [SettingPropertyGroup("1. Tournament Configuration")]
        [SettingProperty("Equipment Filter (Remove Spears)", "Removes spears from tournament footmen")]
        public bool TournamentEquipmentFilter { get; set; }

        #endregion Other

        #region Re-Roll

        [XmlElement]
        [SettingProperty("Re-rolls per Tournament", 0, 99, "Maximum number of times you can re-roll the prize pool per tournament.  Set to 0 to disable.")]
        [SettingPropertyGroup("2. Re-Roll")]
        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;

        #endregion Re-Roll

        #region Prize Selection

        [XmlElement]
        [SettingProperty("Prize Selection", "Should the Select Prize from the Prize Pool be enable. ")]
        [SettingPropertyGroup("3. Prize Selection")]
        public bool EnablePrizeSelection { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("3. Prize Selection")]
        [SettingProperty("Prizes Per Pool", 0, 10)]
        public int NumberOfPrizeOptions { get; set; } = 3;

        [XmlElement]
        [SettingPropertyGroup("3. Prize Selection")]
        [SettingProperty("Include Town Inventory")]
        public bool PrizeListIncludeTown { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("3. Prize Selection")]
        [SettingProperty("Include Vanilla")]
        public bool PrizeListIncludeVanilla { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("3. Prize Selection")]
        [SettingProperty("Include Custom")]
        public bool PrizeListIncludeCustom { get; set; } = false;

        [XmlElement]
        [SettingPropertyGroup("3. Prize Selection")]
        [SettingProperty("Include Legacy")]
        public bool PrizeListIncludeLegacy { get; set; } = true;

        [XmlElement]
        [SettingProperty("Town Prize Min Value", 0, 10000, "Any item below this value will not be used.")]
        [SettingPropertyGroup("3. Prize Selection")]
        public int TownPrizeMin { get; set; } = 1600;

        [XmlElement]
        [SettingProperty("Town Prize Max Value", 0, 300000, "Any item above this value will not be used.")]
        [SettingPropertyGroup("3. Prize Selection")]
        public int TownPrizeMax { get; set; } = 8500;

        [XmlElement]
        [SettingProperty("Prize Value Per Level", 0, 10000)]
        [SettingPropertyGroup("3. Prize Selection")]
        public int PrizeValueMaxIncreasePerLevel { get; set; } = 500;

        [XmlElement]
        [SettingProperty("Prize Value Per Level", 0, 10000)]
        [SettingPropertyGroup("3. Prize Selection")]
        public int PrizeValueMinIncreasePerLevel { get; set; } = 10;

        [XmlElement]
        [SettingProperty("Prize Value Filters Vanilla Items")]
        [SettingPropertyGroup("3. Prize Selection")]
        public bool TownPrizeMinMaxAffectsVanilla { get; set; }

        [XmlElement]
        [SettingProperty("Prize Value Filters Custom Items")]
        [SettingPropertyGroup("3. Prize Selection")]
        public bool TownPrizeMinMaxAffectsCustom { get; set; }

        [XmlElement]
        [SettingProperty("Town Prosperity Affects Prize Value")]
        [SettingPropertyGroup("3. Prize Selection")]
        public bool TownProsperityAffectsPrizeValues { get; set; } = true;

        [XmlElement]
        [SettingProperty("Town Prosperity: Low", 0, 1f, "Setting is a decimal so .65 means 65% of max value")]
        [SettingPropertyGroup("3. Prize Selection")]
        public float TownProsperityLow { get; set; } = .65f;

        [XmlElement]
        [SettingProperty("Town Prosperity: Mid", 0, 2f, "Setting is a decimal so 1.0 means 100% of max value")]
        [SettingPropertyGroup("3. Prize Selection")]
        public float TownProsperityMid { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Town Prosperity: High", 0, 3f, "Setting is a decimal so 1.3 means 130% of max value")]
        [SettingPropertyGroup("3. Prize Selection")]
        public float TownProsperityHigh { get; set; } = 1.3f;

        [XmlElement]
        [SettingProperty("Prize Type Filter")]
        [SettingPropertyGroup("4. Prize Type Filter")]
        public bool EnablePrizeTypeFilterToLists { get; set; } = false;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Bow")]
        public bool EnableItemType_Bow { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Crossbow")]
        public bool EnableItemType_Crossbow { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType OneHanded Weapon")]
        public bool EnableItemType_OneHandedWeapon { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Polearm")]
        public bool EnableItemType_Polearm { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Shield")]
        public bool EnableItemType_Shield { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Thrown")]
        public bool EnableItemType_Thrown { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType TwoHanded Weapon")]
        public bool EnableItemType_TwoHandedWeapon { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType HeadArmor")]
        public bool EnableItemType_HeadArmor { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType HandArmor")]
        public bool EnableItemType_HandArmor { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType BodyArmor")]
        public bool EnableItemType_BodyArmor { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Leg Armor")]
        public bool EnableItemType_LegArmor { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Cape/Shoulders")]
        public bool EnableItemType_Cape { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Horse")]
        public bool EnableItemType_Horse { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("4. Prize Type Filter")]
        [SettingProperty("Allow ItemType Horse Harness/Saddle")]
        public bool EnableItemType_HorseHarness { get; set; } = true;

        #region Bonus Winnings

        [XmlElement]
        [SettingPropertyGroup("5. Bonus Winnings")]
        [SettingProperty("Bonus Gold Per Round", 0, 1000)]
        public int BonusTournamentMatchGold { get; set; } = 0;

        [XmlElement]
        [SettingPropertyGroup("5. Bonus Winnings")]
        [SettingProperty("Payout Bonus Gold On Match Win", 0, 20000)]
        public bool BonusTournamentMatchGoldImmediate { get; set; } = false;

        [XmlElement]
        [SettingPropertyGroup("5. Bonus Winnings")]
        [SettingProperty("Bonus Gold for Tournament Win", 0, 20000)]
        public int BonusTournamentWinGold { get; set; } = 0;

        [XmlElement]
        [SettingPropertyGroup("5. Bonus Winnings")]
        [SettingProperty("Bonus Renown for Tournament Win", 0, 100)]
        public int BonusTournamentWinRenown { get; set; } = 0;

        [XmlElement]
        [SettingPropertyGroup("5. Bonus Winnings")]
        [SettingProperty("Bonus Influence for Tournament Win", 0f, 50f)]
        public float BonusTournamentWinInfluence { get; set; } = 0;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown for Opponent Difficulty")]
        public bool EnableRenownPerTroopTier { get; set; } = false;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Tier 1", 0f, 50f)]
        public float RenownTroopTier1 { get; set; } = .15f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Tier 2", 0f, 50f)]
        public float RenownTroopTier2 { get; set; } = .25f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Tier 3", 0f, 50f)]
        public float RenownTroopTier3 { get; set; } = .35f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Tier 4", 0f, 50f)]
        public float RenownTroopTier4 { get; set; } = .45f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Tier 5", 0f, 50f)]
        public float RenownTroopTier5 { get; set; } = .55f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Tier 6", 0f, 50f)]
        public float RenownTroopTier6 { get; set; } = .65f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Tier 7", 0f, 50f)]
        public float RenownTroopTier7 { get; set; } = .75f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Hero Base", 0f, 50f)]
        public float RenownPerHeroPropertyHeroBase { get; set; } = 1f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown IsNoble", 0f, 50f)]
        public float RenownPerHeroPropertyIsNoble { get; set; } = 3f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown IsBotable", 0f, 50f)]
        public float RenownPerHeroPropertyIsNotable { get; set; } = 1f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown IsCommander", 0f, 50f)]
        public float RenownPerHeroPropertyIsCommander { get; set; } = 1f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Minor Faction Hero", 0f, 50f)]
        public float RenownPerHeroPropertyIsMinorFactionHero { get; set; } = 2f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Minor Faction Leader", 0f, 50f)]
        public float RenownPerHeroPropertyIsMinorFactionLeader { get; set; } = 5f;

        [XmlElement]
        [SettingPropertyGroup("6. Bonus Renown Configuration")]
        [SettingProperty("Bonus Renown Major Faction Leader", 0f, 50f)]
        public float RenownPerHeroPropertyIsMajorFactionLeader { get; set; } = 10f;

        #endregion Bonus Winnings

        [XmlElement]
        [SettingPropertyGroup("7. Other")]
        [SettingProperty("Enable Tournament Type Selection")]
        public bool EnableTournamentTypeSelection { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("7. Other")]
        [SettingProperty("Enable Random Tournament Type at Spawn", "If enabled this setting will randomize the tournament type between Melee Group, and solo free-for-all.")]
        public bool EnableTournamentRandomSelection { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("98. Experimental")]
        [SettingProperty("Enable ItemModifiers For Prizes", "Warning: May cause item loss, bug in core game. Seems OK in BL-1.2.0Beta")]
        public bool EnableItemModifiersForPrizes { get; set; } = false;

        [XmlElement]
        [SettingPropertyGroup("98. Experimental")]
        [SettingProperty("Town Prosperity Affects ItemModifiers", "If ItemModifiers are on, this setting will make them lean towards better if high prosperity or worse if it's low prosperity.")]
        public bool TownProsperityAffectsItemModifiers { get; set; } = false;

        #endregion Prize Selection

        #region Match Odds

        [XmlElement]
        [SettingPropertyGroup("98. Experimental")]
        [SettingProperty("OppenentDifficultyAffectsOdds", "Not working in 1.5-beta")]
        public bool OppenentDifficultyAffectsOdds { get; set; } = true;

        [XmlElement]
        [SettingPropertyGroup("98. Experimental")]
        [SettingProperty("Max Odds", 4f, 10f, "Maximum Odds for Tournament Bets")]
        public float MaximumBetOdds { get; set; } = 4;

        #endregion Match Odds

        #region UnImplemented

        [XmlElement]
        [SettingPropertyGroup("99. Not Used")]
        [SettingProperty("TournamentBonusMoneyBaseNamedCharLevel", 0, 1)]
        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;

        [XmlElement]
        [SettingPropertyGroup("99. Not Used")]
        [SettingProperty("CompanionsWinPrizes(Not Active)")]
        public bool CompanionsWinPrizes { get; set; } = false;
        [XmlElement]
        [SettingPropertyGroup("99. Not Used")]
        [SettingProperty("BonusRenownMostKills", 0f, 5f)]
        public float BonusRenownMostKills { get; set; } = 0f;
        [XmlElement]
        [SettingPropertyGroup("99. Not Used")]
        [SettingProperty("BonusRenownMostDamage", 0f, 5f)]
        public float BonusRenownMostDamage { get; set; } = 0f;
        [XmlElement]
        [SettingPropertyGroup("99. Not Used")]
        [SettingProperty("BonusRenownFirstKill", 0f, 5f)]
        public float BonusRenownFirstKill { get; set; } = 0f;
        [XmlElement]
        [SettingPropertyGroup("99. Not Used")]
        [SettingProperty("BonusRenownLeastDamage", 0f, 5f)]
        public float BonusRenownLeastDamage { get; set; } = 0f;

        #endregion UnImplemented
    }
}