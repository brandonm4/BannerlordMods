using BMTweakCollection.Models;

using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TournamentsXPanded.Common.Patches;

namespace BMTweakCollection.Patches
{
    public class GetEnergyCostForRefining : PatchBase<GetEnergyCostForRefining>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSmithingModel).GetMethod("GetEnergyCostForRefining", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(GetEnergyCostForRefining).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
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
            __result = 1;
            return false; // make sure you only skip if really necessary
        }
    }
    public class GetEnergyCostForSmithing : PatchBase<GetEnergyCostForSmithing>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSmithingModel).GetMethod("GetEnergyCostForSmithing", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(GetEnergyCostForSmithing).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
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
            __result = 1;
            return false; // make sure you only skip if really necessary
        }
    }
    public class GetEnergyCostForSmelting : PatchBase<GetEnergyCostForSmelting>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSmithingModel).GetMethod("GetEnergyCostForSmelting", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(GetEnergyCostForSmelting).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
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
            __result = 1;
            return false; // make sure you only skip if really necessary
        }
    }

    public class ResearchPointsNeedForNewPart : PatchBase<ResearchPointsNeedForNewPart>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSmithingModel).GetMethod("ResearchPointsNeedForNewPart", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(ResearchPointsNeedForNewPart).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

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

        private static bool Prefix(ref int __result, int count)
        {
            __result = (count * count + 12) / BMRandomTweaksConfiguration.Instance.CustomSmithingXPDivisor;
            return false; // make sure you only skip if really necessary
        }
    }
}
