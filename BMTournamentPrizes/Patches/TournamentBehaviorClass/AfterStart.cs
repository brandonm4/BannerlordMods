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

namespace TournamentsXPanded.Patches.TournamentBehaviorClass
{
    public class AfterStart : PatchBase<AfterStart>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentBehavior).GetMethod("AfterStart");

        private static readonly MethodInfo PatchMethodInfo = typeof(AfterStart).GetMethod(nameof(Prefix));
        public override bool IsApplicable(Game game)
        {
            return (TournamentXPSettings.Instance.IsTournamentXPEnabled || TournamentXPSettings.Instance.IsArenaXPEnabled);
        }
        public override void Reset() { }

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
        public static bool Prefix(TournamentBehavior __instance)
        {
            TournamentPrizePoolBehavior.TournamentReward = new TournamentReward(__instance.TournamentGame);
            typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + TournamentXPSettings.Instance.BonusTournamentWinGold);

            return true;
        }
    }
}
