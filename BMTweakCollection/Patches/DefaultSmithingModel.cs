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
            if (BMTweakCollectionSubModule.Configuration.CustomSmithingModelEnabled)
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
            if (BMTweakCollectionSubModule.Configuration.CustomSmithingModelEnabled)
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
            if (BMTweakCollectionSubModule.Configuration.CustomSmithingModelEnabled)
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
            if (BMTweakCollectionSubModule.Configuration.CustomSmithingModelEnabled)
            {
                __result = (count * count + 12) / 36;
                return false; 
            }
            return true;
        }
    }

    //[HarmonyPatch(typeof(DefaultSmithingModel), "GetSkillXpForSmithing")]
    //public class DefaultSmithingModelPatch5
    //{    
    //    public static void Postfix(ref int __result)
    //    {
    //        __result = __result * 3;
    //    }
    //}
    //[HarmonyPatch(typeof(DefaultSmithingModel), "GetSkillXpForSmelting")]
    //public class DefaultSmithingModelPatch6
    //{
    //    public static void Postfix(ref int __result)
    //    {
    //        __result = __result * 5;
    //    }
    //}
    //[HarmonyPatch(typeof(DefaultSmithingModel), "GetSkillXpForRefining")]
    //public class DefaultSmithingModelPatch7
    //{
    //    public static void Postfix(ref int __result)
    //    {
    //        __result = __result * 5;
    //    }
    //}
    //[HarmonyPatch(typeof(DefaultSmithingModel), "GetPartResearchGainForSmeltingItem")]
    //public class DefaultSmithingModelPatch8
    //{
    //    public static void Postfix(ref int __result)
    //    {
    //        __result = __result * 5;
    //    }
    //}
    //[HarmonyPatch(typeof(DefaultSmithingModel), "GetPartResearchGainForSmithingItem")]
    //public class DefaultSmithingModelPatch9
    //{
    //    public static void Postfix(ref int __result)
    //    {
    //        __result = __result * 5;
    //    }
    //}
}



/*
 public override int GetSkillXpForRefining(ref Crafting.RefiningFormula refineFormula)
        {
            return MathF.Round(0.3f * (float)(this.GetCraftingMaterialItem(refineFormula.Output).Value * refineFormula.OutputCount));
        }

        public override int GetSkillXpForSmelting(ItemObject item)
        {
            return MathF.Round(0.02f * (float)item.Value);
        }

        public override int GetSkillXpForSmithing(ItemObject item)
        {
            return MathF.Round(0.1f * (float)item.Value);
        }

 */
