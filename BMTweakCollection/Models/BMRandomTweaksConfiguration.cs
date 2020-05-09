using ModLib;
using ModLib.Attributes;

using System.Xml.Serialization;

namespace BMTweakCollection.Models
{
    public class BMRandomTweaksConfiguration : SettingsBase
    {
        public const string InstanceID = "BMRandomTweaksConfiguration";
        public override string ModName => "BM Tweaks Collection";
        public override string ModuleFolderName => BMTweakCollectionSubModule.ModuleFolderName;

        public static BMRandomTweaksConfiguration Instance
        {
            get
            {
                return (BMRandomTweaksConfiguration)SettingsDatabase.GetSettings(InstanceID);
            }
        }

        [XmlElement]
        public override string ID { get; set; } = InstanceID;

        [XmlElement]
        [SettingProperty("MaxHideoutTroopsEnabled")]
        public bool MaxHideoutTroopsEnabled { get; set; } = true;

        [XmlElement]
        [SettingProperty("MaxHideoutTroopsEnabled", 8, 50)]
        public int MaxHideoutTroops { get; set; } = 20;

        [XmlElement]
        [SettingProperty("CustomSmithingModelEnabled")]
        public bool CustomSmithingModelEnabled { get; set; } = true;

        [XmlElement]
        [SettingProperty("CustomSmithingXPDivisor", 4, 100)]
        public int CustomSmithingXPDivisor { get; set; } = 36;

        [XmlElement]
        [SettingProperty("HorseDropEnabled")]
        public bool HorseDropEnabled { get; set; } = true;

        [XmlElement]
        [SettingProperty("BaseDropRate", 10, 100)]
        public float BaseDropRate { get; set; } = 30f;

        [XmlElement]
        [SettingProperty("BaseMaxValue", 250, 250000)]
        public float BaseMaxValue { get; set; } = 5000f;

        [XmlElement]
        [SettingProperty("LootRandomMaxRoll", 25, 150)]
        public float LootRandomMaxRoll { get; set; } = 110f;

        [XmlElement]
        [SettingProperty("LootSkillXpGain", 0, 100)]
        public float LootSkillXpGain { get; set; } = 10f;

        public bool AutoLearnSmeltedParts { get; set; } = true;

        [XmlElement]
        [SettingProperty("MaxCraftingStamina", 0, 1000)]
        public int MaxCraftingStamina { get; set; } = 400;
        [XmlElement]
        [SettingProperty("CraftingStaminaTweakEnabled")]
        public bool CraftingStaminaTweakEnabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("CraftingStaminaGainAmount", 0, 30)]
        public int CraftingStaminaGainAmount { get; set; } = 10;
        [XmlElement]
        [SettingProperty("CraftingStaminaGainOutsideSettlementMultiplier", 0, 1f)]
        public float CraftingStaminaGainOutsideSettlementMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Leave Kingdom: Relationship Penalty No Fiefs", -40, 0)]
        public int RelationLossNoSettlements { get; set; } = -20;
        [XmlElement]
        [SettingProperty("Leave Kingdom: Relationship Penalaty with Fiefs", -40, 0)]
        public int RelationLossWithSettlements { get; set; } = -40;
        [XmlElement]
        [SettingProperty("Leave Kingdom: Rebellion with Fiefs")]
        public bool LeaveKingdomWithSettlementsRebellion { get; set; } = true;
    }
}