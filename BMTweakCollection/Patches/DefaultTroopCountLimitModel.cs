using BMTweakCollection.Models;

using HarmonyLib;

using System.Reflection;

using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

using TournamentsXPanded.Common.Patches;

namespace BMTweakCollection.Patches
{
    public class GetHideoutBattlePlayerMaxTroopCount : PatchBase<GetHideoutBattlePlayerMaxTroopCount>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultTroopCountLimitModel).GetMethod("GetHideoutBattlePlayerMaxTroopCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(GetHideoutBattlePlayerMaxTroopCount).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return true;
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            BMTweakCollectionSubModule.Harmony.Patch(TargetMethodInfo,
              prefix: new HarmonyMethod(PatchMethodInfo)
              {
                  priority = Priority.First,
                  //before = new[] { "that.other.harmony.user" }
              }
              );

            Applied = true;
        }

        public override void Reset()
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