using HarmonyLib;

using SandBox.TournamentMissions.Missions;

using System.Reflection;

using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.TournamentBehaviorClass
{
    public class AfterStart : PatchBase<AfterStart>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentBehavior).GetMethod("AfterStart", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(AfterStart).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return true;
        }

        public override void Reset()
        {
        }

        public override void Apply(Game game)
        {
            if (Applied) return;
            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
              prefix: new HarmonyMethod(PatchMethodInfo)
              {
                  priority = Priority.Low,
              }
              );

            Applied = true;
        }

        private static bool Prefix(TournamentBehavior __instance)
        {
            TournamentPrizePoolBehavior.TournamentReward = new TournamentReward(__instance.TournamentGame);
            typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + TournamentXPSettings.Instance.BonusTournamentWinGold);

            return true;
        }
    }
}