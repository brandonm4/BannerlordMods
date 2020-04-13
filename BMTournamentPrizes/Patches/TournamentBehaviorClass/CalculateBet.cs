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
    public class CalculateBet : PatchBase<CalculateBet>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentBehavior).GetMethod("CalculateBet", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

       private static readonly MethodInfo PatchMethodInfo = typeof(CalculateBet).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        public override bool IsApplicable(Game game)
        {            
            return TournamentXPSettings.Instance.MaximumBetOdds > 0;
        }
        public override void Reset() { }

        public override void Apply(Game game)
        {
            if (Applied) return;
            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
          transpiler:new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /* Orig */
            /*
            IL_038c: ldc.r4 4      <--- We want to replace this one
            IL_0391: call float32[TaleWorlds.Library]TaleWorlds.Library.MathF::Clamp(float32, float32, float32)
            IL_0396: stloc.s V_9
            IL_0398: ldarg.0
            IL_0399: ldloc.s V_9
            IL_039b: ldc.r4 10
            IL_03a0: mul
            IL_03a1: conv.i4
            IL_03a2: conv.r4
            IL_03a3: ldc.r4 10
            IL_03a8: div
            IL_03a9: call instance void SandBox.TournamentMissions.Missions.TournamentBehavior::set_BetOdd(float32)

            IL_03ae: ret
            */
            var codes = new List<CodeInstruction>();
            var count = instructions.Count();

            /* Get all lines before what we want to change */
            var origCodes = new List<CodeInstruction>(instructions).Take(count - 13);

            // The line we want to change
            var changeCode = instructions.Skip(count - 13).Take(1).First();
            changeCode.operand = TournamentXPSettings.Instance.MaximumBetOdds;

            //All the lines after what we want to change
            var afterCodes = new List<CodeInstruction>(instructions).Skip(count - 12);
            
            //Rebuild and return
            origCodes.AddItem(changeCode);
            codes = origCodes.Concat(afterCodes).ToList();
           
           // codes.RemoveRange(0, codes.Count - 1);
            return codes.AsEnumerable();
        }
    }
}
