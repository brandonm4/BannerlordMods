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
    public class GetTournamentPrize : PatchBase<GetTournamentPrize>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentGame).GetMethod("GetTournamentPrize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfoTransPile = typeof(GetTournamentPrize).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfoPostFix = typeof(GetTournamentPrize).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            if (TournamentXPSettings.Instance.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell
                || TournamentXPSettings.Instance.EnablePrizeSelection
                || TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament > 0
                || TournamentXPSettings.Instance.PrizeListMode != PrizeListMode.Vanilla
                || TournamentXPSettings.Instance.EnablePrizeTypeFilterToLists)
            {
                return true;
            }
            return false;
        }
        public override void Reset() { }

        public override void Apply(Game game)
        {
            if (Applied) return;
            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
          transpiler: new HarmonyMethod(PatchMethodInfoTransPile),
          postfix: new HarmonyMethod(PatchMethodInfoPostFix)
              );

            Applied = true;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            codes.RemoveRange(0, codes.Count - 1);
            return codes.AsEnumerable();
        }

        static void Postfix(TournamentGame __instance, ref ItemObject __result)
        {
            //__result currently has stock item.          
            try
            {
                ItemObject prize;
                if (TournamentXPSettings.Instance.EnablePrizeSelection
                    || TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament > 0
                    || TournamentXPSettings.Instance.PrizeListMode != PrizeListMode.Vanilla)
                {
                    TournamentPrizePoolBehavior.GetTournamentPrizePool(__instance.Town.Settlement, __result);
                    prize = TournamentPrizePoolBehavior.GenerateTournamentPrize(__instance, null, false);
                }
                else
                {
                    prize = TournamentPrizePoolBehavior.GetTournamentPrizeVanilla(__instance.Town.Settlement);
                }
                if (prize == null)
                {
                    MessageBox.Show("Tournament Prize System", "Error generating Tournament Prize. Reverting to vanilla system.");
                    return;
                }
                __result = prize;
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: Tournament Prize System: GetTournamentPrize");
                FileLog.Log(ex.ToStringFull());
            }
        }
    }
}
