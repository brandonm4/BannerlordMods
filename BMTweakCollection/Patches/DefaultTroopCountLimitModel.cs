using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(DefaultTroopCountLimitModel), "GetHideoutBattlePlayerMaxTroopCount")]
    public class DefaultTroopCountLimitModelPatch1
    {
        public DefaultTroopCountLimitModelPatch1()
        {

        }
        static bool Prefix(ref int __result)
        {
            if (BMTweakCollectionMain.Configuration.MaxHideoutTroopsEnabled)
            {
                __result = BMTweakCollectionMain.Configuration.MaxHideoutTroops;
                return false; // make sure you only skip if really necessary
            }
            return false;
        }
    }
}
