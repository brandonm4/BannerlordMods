using HarmonyLib;

using SandBox.TournamentMissions.Missions;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Extensions;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.TournamentBehaviorClass
{
    public class EndCurrentMatch : PatchBase<EndCurrentMatch>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentBehavior).GetMethod("EndCurrentMatch", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(EndCurrentMatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return (TournamentXPSettings.Instance.BonusTournamentMatchGold > 0 || TournamentXPSettings.Instance.EnableRenownPerTroopTier);
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
              prefix: new HarmonyMethod(PatchMethodInfo)
              {
                  priority = Priority.First,
              }
              );
            Applied = true;
        }

        private static bool Prefix(TournamentBehavior __instance)
        {
            var winners = (List<TournamentParticipant>)Traverse.Create(__instance.CurrentMatch).Method("GetWinners").GetValue();
            if (winners.Where(x => x.Character.HeroObject == Hero.MainHero).Count() > 0)
            {
                var tournamentInfo = TournamentsXPandedBehavior.GetTournamentInfo(__instance.TournamentGame.Town);

                if (__instance.CurrentRoundIndex > tournamentInfo.Rewards.LastPayoutIndex)
                {
                    tournamentInfo.Rewards.LastPayoutIndex = __instance.CurrentRoundIndex;
                    if (TournamentXPSettings.Instance.EnableRenownPerTroopTier)
                    {
                        var renownbonus = 0f;
                        foreach (var team in __instance.CurrentMatch.Teams)
                        {
                            var teambonus = 0f;
                            foreach (var p in team.Participants)
                            {
                                teambonus += TournamentXPSettings.Instance.GetRenownValue(p.Character);
                                if (p.Character.IsHero && p.Character.HeroObject == Hero.MainHero)
                                {
                                    teambonus = 0;
                                    break;
                                }
                            }
                            renownbonus += teambonus;
                        }
                        tournamentInfo.Rewards.BonusRenown += renownbonus;
                    }
                    if (TournamentXPSettings.Instance.BonusTournamentMatchGold > 0)
                    {
                        if (TournamentXPSettings.Instance.BonusTournamentMatchGoldImmediate)
                        {
                            GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, TournamentXPSettings.Instance.BonusTournamentMatchGold, false);
                        }
                        else
                        {
                            typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + TournamentXPSettings.Instance.BonusTournamentMatchGold);
                        }
                    }
                }
            }

            return true;
        }
    }
}