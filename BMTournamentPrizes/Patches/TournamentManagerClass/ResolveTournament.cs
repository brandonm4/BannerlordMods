using HarmonyLib;

using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.TournamentManagerClass
{
    internal class ResolveTournament : PatchBase<ResolveTournament>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentManager).GetMethod("ResolveTournament", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(ResolveTournament).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return (TournamentXPSettings.Instance.EnablePrizeSelection || TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament > 0);
        }

        public override void Reset()
        {
        }

        public override void Apply(Game game)
        {
            if (Applied) return;
            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
                  postfix: new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }

        private static void Prefix(TournamentGame tournament)
        {
            TournamentPrizePoolBehavior.ClearTournamentPrizes(tournament.Town.Settlement);
        }

        private static bool Prepare()
        {
            return (TournamentXPSettings.Instance.EnablePrizeSelection);
        }
    }
}