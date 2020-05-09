using BMTweakCollection.Models;

using HarmonyLib;

using System;
using System.Collections;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace BMTweakCollection.Patches
{
    //[HarmonyPatch(typeof(CraftingCampaignBehavior), "DoSmelting")]
    //public class DoSmeltingPatch
    //{
    //    private static MethodInfo openPartMethodInfo;

    //    static void Postfix(CraftingCampaignBehavior __instance, ItemObject item)
    //    {
    //        if (item == null) return;
    //        if (__instance == null) throw new ArgumentNullException(nameof(__instance), $"Tried to run postfix for {nameof(CraftingCampaignBehavior)}.DoSmelting but the instance was null.");
    //        if (openPartMethodInfo == null) GetMethodInfo();
    //        foreach (CraftingPiece piece in SmeltingHelper.GetNewPartsFromSmelting(item))
    //        {
    //            if (piece != null && piece.Name != null)
    //                openPartMethodInfo.Invoke(__instance, new object[] { piece });
    //        }
    //    }

    //    static bool Prepare()
    //    {
    //        if (BMRandomTweaksConfiguration.Instance.AutoLearnSmeltedParts)
    //            GetMethodInfo();
    //        return BMRandomTweaksConfiguration.Instance.AutoLearnSmeltedParts;
    //    }

    //    private static void GetMethodInfo()
    //    {
    //        openPartMethodInfo = typeof(CraftingCampaignBehavior).GetMethod("OpenPart", BindingFlags.NonPublic | BindingFlags.Instance);
    //    }
    //}

    [HarmonyPatch(typeof(CraftingCampaignBehavior), "GetMaxHeroCraftingStamina")]
    public class GetMaxHeroCraftingStaminaPatch
    {
        private static bool Prefix(CraftingCampaignBehavior __instance, ref int __result)
        {
            __result = BMRandomTweaksConfiguration.Instance.MaxCraftingStamina;
            return false;
        }

        private static bool Prepare()
        {
            return BMRandomTweaksConfiguration.Instance.CraftingStaminaTweakEnabled;
        }
    }

    [HarmonyPatch(typeof(CraftingCampaignBehavior), "HourlyTick")]
    public class HourlyTickPatch
    {
        private static FieldInfo recordsInfo;

        private static bool Prefix(CraftingCampaignBehavior __instance)
        {
            if (recordsInfo == null)
                GetRecordsInfo();
            //Get the list of hero records
            IDictionary records = (IDictionary)recordsInfo.GetValue(__instance);

            foreach (Hero hero in records.Keys)
            {
                int curCraftingStamina = __instance.GetHeroCraftingStamina(hero);

                if (curCraftingStamina < BMRandomTweaksConfiguration.Instance.MaxCraftingStamina)
                {
                    int staminaGainAmount = BMRandomTweaksConfiguration.Instance.CraftingStaminaGainAmount;

                    if (BMRandomTweaksConfiguration.Instance.CraftingStaminaGainOutsideSettlementMultiplier < 1 && hero.PartyBelongedTo?.CurrentSettlement == null)
                        staminaGainAmount = (int)Math.Ceiling(staminaGainAmount * BMRandomTweaksConfiguration.Instance.CraftingStaminaGainOutsideSettlementMultiplier);

                    int diff = BMRandomTweaksConfiguration.Instance.MaxCraftingStamina - curCraftingStamina;
                    if (diff < staminaGainAmount)
                        staminaGainAmount = diff;

                    __instance.SetHeroCraftingStamina(hero, Math.Min(BMRandomTweaksConfiguration.Instance.MaxCraftingStamina, curCraftingStamina + staminaGainAmount));
                }
            }
            return false;
        }

        private static bool Prepare()
        {
            if (BMRandomTweaksConfiguration.Instance.CraftingStaminaTweakEnabled)
                GetRecordsInfo();
            return BMRandomTweaksConfiguration.Instance.CraftingStaminaTweakEnabled;
        }

        private static void GetRecordsInfo()
        {
            recordsInfo = typeof(CraftingCampaignBehavior).GetField("_heroCraftingRecords", BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}