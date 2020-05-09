using BMTweakCollection.Models;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;

using TournamentsXPanded.Common.Patches;

namespace BMTweakCollection.Patches.ChangeKingdomActionClass
{
    public class ApplyInternal : PatchBase<ApplyInternal>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(ChangeKingdomAction).GetMethod("ApplyInternal", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(ApplyInternal).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

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

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                var c = codes[i];
                try
                {
                 //   bool found = false;
                    if (c.opcode == OpCodes.Ldc_I4_S)
                    {
                        if ((sbyte)c.operand == -20)
                        {
                            c.operand = (sbyte)BMRandomTweaksConfiguration.Instance.RelationLossNoSettlements;
                        }
                        if ((sbyte)c.operand == -40)
                        {
                            c.operand = (sbyte)BMRandomTweaksConfiguration.Instance.RelationLossWithSettlements;
                            //found = true;
                            //if (BMRandomTweaksConfiguration.Instance.LeaveKingdomWithSettlementsRebellion == false)
                            //    codes[i + 18].opcode = OpCodes.Ldc_I4_0;
                        }
                    }

                }
                catch
                { }
            }

            return codes.AsEnumerable();
        }
    }


}