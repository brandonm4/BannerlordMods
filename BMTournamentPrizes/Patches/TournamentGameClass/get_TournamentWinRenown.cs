using HarmonyLib;

using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.TournamentGameClass
{
    public class get_TournamentWinRenown : PatchBase<get_TournamentWinRenown>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentGame).GetMethod("get_TournamentWinRenown", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(get_TournamentWinRenown).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return (TournamentXPSettings.Instance.BonusTournamentWinRenown > 0
                || TournamentXPSettings.Instance.EnableRenownPerTroopTier
                || TournamentXPSettings.Instance.BonusRenownMostKills > 0
                || TournamentXPSettings.Instance.BonusRenownMostDamage > 0
                || TournamentXPSettings.Instance.BonusRenownLeastDamage > 0
                || TournamentXPSettings.Instance.BonusRenownFirstKill > 0
                );
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
                  postfix: new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }

        private static void Postfix(ref float __result)
        {
            __result += TournamentPrizePoolBehavior.TournamentReward.BonusRenown;
        }
    }
}