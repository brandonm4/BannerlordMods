using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;

using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Patches.TournamentGameClass
{
    public class GetTournamentPrize : PatchBase<GetTournamentPrize>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentGame).GetMethod("GetTournamentPrize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        //private static readonly MethodInfo PatchMethodInfoTransPile = typeof(GetTournamentPrize).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfoPostFix = typeof(GetTournamentPrize).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return true;
        }

        public override void Reset()
        {
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
          //transpiler: new HarmonyMethod(PatchMethodInfoTransPile),
          postfix: new HarmonyMethod(PatchMethodInfoPostFix)
              );

            Applied = true;
        }

        //private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        //{
        //    var codes = new List<CodeInstruction>(instructions);
        //    codes.RemoveRange(0, codes.Count - 1);
        //    return codes.AsEnumerable();
        //}

        private static void Postfix(TournamentGame __instance, ref ItemObject __result)
        {
            //__result currently has stock item.
            try
            {
                __result = TournamentPrizePoolBehavior.GenerateTournamentPrize(__instance, null, false);
            }
            catch (Exception ex)
            {
                ErrorLog.Log("ERROR: Tournament Prize System: GetTournamentPrize");
                ErrorLog.Log(ex.ToStringFull());
            }
        }
    }
}