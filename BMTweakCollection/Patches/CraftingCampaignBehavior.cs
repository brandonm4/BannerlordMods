using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TournamentsXPanded.Common.Patches;

namespace BMTweakCollection.Patches
{
    
    internal class HourlyTick : PatchBase<HourlyTick>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(CraftingCampaignBehavior).GetMethod("HourlyTick", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(HourlyTick).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return true;
        }
        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            BMTweakCollectionSubModule.Harmony.Patch(TargetMethodInfo,
              transpiler: new HarmonyMethod(PatchMethodInfo)
              {
                  priority = Priority.First,
                  //before = new[] { "that.other.harmony.user" }
              }
              );

            Applied = true;
        }

        public override void Reset()
        {
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            List<CodeInstruction> newSet = new List<CodeInstruction>();
            var bFound = false;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Bge_S && !bFound)
                {
                    i += 10;
                    bFound = true;
                }
                else
                {
                    newSet.Add(codes[i]);
                }
            }
            return codes.AsEnumerable();
        }

    }
}

