using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(DefaultSmithingModel), "GetEnergyCostForRefining")]
    public class DefaultSmithingModelPatch1
    {
        public DefaultSmithingModelPatch1()
        {

        }
        static bool Prefix(ref int __result)
        {
            if (BMTweakCollectionMain.Configuration.CustomSmithingModelEnabled)
            {
                __result = 1;
                return false; // make sure you only skip if really necessary
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(DefaultSmithingModel), "GetEnergyCostForSmithing")]
    public class DefaultSmithingModelPatch2
    {
        public DefaultSmithingModelPatch2()
        {

        }
        static bool Prefix(ref int __result)
        {
            if (BMTweakCollectionMain.Configuration.CustomSmithingModelEnabled)
            {
                __result = 1;
                return false; // make sure you only skip if really necessary
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(DefaultSmithingModel), "GetEnergyCostForSmelting")]
    public class DefaultSmithingModelPatch3
    {
        public DefaultSmithingModelPatch3()
        {

        }
        static bool Prefix(ref int __result)
        {
            if (BMTweakCollectionMain.Configuration.CustomSmithingModelEnabled)
            {
                __result = 1;
                return false; // make sure you only skip if really necessary
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DefaultSmithingModel), "ResearchPointsNeedForNewPart")]
    public class DefaultSmithingModelPatch4
    {
        public DefaultSmithingModelPatch4()
        {

        }
        static bool Prefix(ref int __result, int count)
        {
            if (BMTweakCollectionMain.Configuration.CustomSmithingModelEnabled)
            {
                __result = (count * count + 12) / BMTweakCollectionMain.Configuration.CustomSmithingXPDivisor;
                return false; // make sure you only skip if really necessary
            }
            return true;
        }
    }
}



