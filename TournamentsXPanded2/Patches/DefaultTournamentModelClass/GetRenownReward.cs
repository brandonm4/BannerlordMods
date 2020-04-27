using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;

using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.DefaultTournamentModelClass
{
    public class GetRenownReward : PatchBase<GetRenownReward>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultTournamentModel).GetMethod("GetRenownReward", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);        
        private static readonly MethodInfo PatchMethodInfo = typeof(GetRenownReward).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            //return TournamentXPSettings.Instance.BonusTournamentWinRenown > 0 || TournamentXPSettings.Instance.EnableRenownPerTroopTier;
            return false;
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

        public override void Reset()
        {
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            codes.RemoveRange(0, codes.Count - 1);
            return codes.AsEnumerable();
        }

        private static void Postfix(ref int __result)
        {
         //   += TournamentPrizePoolBehavior.TournamentReward.BonusRenown;
        }
    }
}

//public override TournamentGame CreateTournament(Town town)
//{
//    return new FightTournamentGame(town);
//}