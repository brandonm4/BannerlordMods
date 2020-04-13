using TournamentsXPanded;
using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Models;
using HarmonyLib;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TournamentLib.Extensions;
using System.Reflection;

namespace TournamentsXPanded.Patches.TournamentGameClass
{
    public class get_TournamentWinRenown : PatchBase<get_TournamentWinRenown>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentGame).GetMethod("get_TournamentWinRenown", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(get_TournamentWinRenown).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return TournamentXPSettings.Instance.BonusTournamentWinRenown > 0 || TournamentXPSettings.Instance.EnableRenownPerTroopTier;
        }
        public override void Reset() { }

        public override void Apply(Game game)
        {
            if (Applied) return;
            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
                  postfix: new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }

        static void Postfix(ref float __result)
        {
            __result += TournamentPrizePoolBehavior.TournamentReward.BonusRenown;
        }
    }
}
