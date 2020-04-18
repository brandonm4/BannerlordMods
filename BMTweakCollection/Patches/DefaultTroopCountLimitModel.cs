using BMTweakCollection.Models;

using HarmonyLib;

using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(DefaultTroopCountLimitModel), "GetHideoutBattlePlayerMaxTroopCount")]
    public class DefaultTroopCountLimitModelPatch1
    {
        public DefaultTroopCountLimitModelPatch1()
        {
        }

        private static bool Prefix(ref int __result)
        {
            if (BMRandomTweaksConfiguration.Instance.MaxHideoutTroopsEnabled)
            {
                __result = BMRandomTweaksConfiguration.Instance.MaxHideoutTroops;
                return false; // make sure you only skip if really necessary
            }
            return false;
        }
    }
}