using HarmonyLib;

using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.TournamentCampaignBehaviorClass
{
    internal class ConsiderStartOrEndTournament : PatchBase<ConsiderStartOrEndTournament>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentCampaignBehavior).GetMethod("AddTournament", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(ConsiderStartOrEndTournament).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);       

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
                  postfix: new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }

     
        private static void Postfix(Town town)
        {
            //if (game.Town != null)
            //{
            //    var info = TournamentsXPandedBehavior.GetTournamentInfo(game.Town);
            //    info.ReRollsUsed = 0;
            //}
        }
    }
}