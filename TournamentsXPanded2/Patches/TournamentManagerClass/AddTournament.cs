using HarmonyLib;

using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;

namespace TournamentsXPanded.Patches.TournamentManagerClass
{
    internal class AddTournament : PatchBase<AddTournament>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentManager).GetMethod("AddTournament", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(AddTournament).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfoPost = typeof(AddTournament).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            //return (TournamentXPSettings.Instance.EnablePrizeSelection || TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament > 0);
            return false;
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
                  //       prefix: new HarmonyMethod(PatchMethodInfo),
                  postfix: new HarmonyMethod(PatchMethodInfoPost)
              );

            Applied = true;
        }

        private static void Prefix(TournamentGame game)
        {
        }

        private static void Postfix(TournamentGame game)
        {
            if (game.Town != null)
            {
                var info = TournamentsXPandedBehavior.GetTournamentInfo(game.Town);
                info.ReRollsUsed = 0;
            }
        }
    }
}