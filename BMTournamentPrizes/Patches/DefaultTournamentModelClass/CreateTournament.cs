using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.DefaultTournamentModelClass
{
    public class CreateTournament : PatchBase<CreateTournament>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultTournamentModel).GetMethod("CreateTournament", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfoTransPile = typeof(CreateTournament).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(CreateTournament).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return true;
        }

        public override void Apply(Game game)
        {
            if (Applied) return;
            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
                  prefix: new HarmonyMethod(PatchMethodInfo)
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

        private static bool Prefix(ref TournamentGame __result, Town town)
        {
            float gameBasicMeleeChance = 65f;
            float rdm = MBRandom.RandomFloatRanged(1f, 100f);
            if (rdm < gameBasicMeleeChance)
            {
                __result = new FightTournamentGame(town);
            }
            else
            {
                var newgame = new Fight2TournamentGame(town);
                rdm = MBRandom.RandomFloatRanged(1f, 100f);
                if (rdm < 50f)
                {
                    newgame.SetFightMode(Fight2TournamentGame.FightMode.One_One);
                }
                __result = newgame;
            }

            return false;
        }


    }
}


//public override TournamentGame CreateTournament(Town town)
//{
//    return new FightTournamentGame(town);
//}
