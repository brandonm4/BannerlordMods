using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace BMTweakCollection.Patches
{

    //public static class LootCollectorInfo
    //{
        //public static Type ClassType = Type.GetType("TaleWorlds.CampaignSystem.LootCollector, TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
    //}


  // [HarmonyPatch(Type.GetType("TaleWorlds.CampaignSystem.LootCollector, TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"))]
  
   // [HarmonyPatch(LootCollectorInfo.ClassType, "GetXpFromHit")]
    public class LootCollectorPatch
    {
        // make sure DoPatching() is called at start either by
        // the mod loader or by your injector

        public static void DoPatching()
        {
            var harmony = new Harmony("com.example.patch");
            var TypeOfLootCollector = Type.GetType("TaleWorlds.CampaignSystem.LootCollector, TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");


            var mOriginal = AccessTools.Method(TypeOfLootCollector, "GetRandomEquipment");
            var mPrefix = SymbolExtensions.GetMethodInfo(() => MyPrefix());
            var mPostfix = SymbolExtensions.GetMethodInfo(() => MyPostfix());
            // in general, add null checks here (new HarmonyMethod() does it for you too)

            harmony.Patch(mOriginal, new HarmonyMethod(mPrefix), new HarmonyMethod(mPostfix));
        }

        public static void MyPrefix()
        {
            // ...
        }

        public static void MyPostfix()
        {
            // ...
        }
    }
}
