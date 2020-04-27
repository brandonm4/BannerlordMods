using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;
using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Patches.TournamentFightMissionControllerClass
{
    public class StartMatch : PatchBase<StartMatch>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentFightMissionController).GetMethod("StartMatch", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(StartMatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

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
          prefix: new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }
        private static bool Prefix(TournamentFightMissionController __instance, TournamentMatch match, bool isLastRound)
        {
            TournamentXPandedMatchBehavior._match = match;
            TournamentXPandedMatchBehavior.firstKiller = null;
            TournamentXPandedMatchBehavior.achievements = new Dictionary<TournamentParticipant, ParticipantAchievements>();
            foreach (var p in match.Participants)
            {
                TournamentXPandedMatchBehavior.achievements.Add(p, new ParticipantAchievements());
            }
            return true;
        }
    }
}


