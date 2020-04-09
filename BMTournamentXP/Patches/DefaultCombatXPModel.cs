using HarmonyLib;

using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Library;

using TournamentLib.Models;

namespace BMTournamentXPMain.Patches
{
    [HarmonyPatch(typeof(DefaultCombatXpModel), "GetXpFromHit")]
    public class DefaultCombatXPModelPatchGetXpFromHit
    {
        [HarmonyPriority(Priority.Low)]
        public static void Postfix(ref DefaultCombatXpModel __instance,
            CharacterObject attackerTroop,
            CharacterObject attackedTroop,
            int damage, bool isFatal,
            CombatXpModel.MissionTypeEnum missionType,
            ref int xpAmount)
        {
            GetXpFromHit(attackerTroop, attackedTroop, damage, isFatal, missionType, out xpAmount);
            //DefaultCombatXPModelPatchGetXpFromHit.ShowMethod("XP CALC");
        }

        private static void ShowMethod(string msg)
        {
            InformationManager.DisplayMessage(new InformationMessage(msg));
        }

        private static void GetXpFromHit(CharacterObject attackerTroop, CharacterObject attackedTroop, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType, out int xpAmount)
        {
            int num = attackedTroop.MaxHitPoints();
            xpAmount = MBMath.Round(0.4f * ((attackedTroop.GetPower() + 0.5f) * (float)(Math.Min(damage, num) + (isFatal ? num : 0))));
            if (missionType == CombatXpModel.MissionTypeEnum.SimulationBattle)
            {
                xpAmount *= 8;
            }
            if (missionType == CombatXpModel.MissionTypeEnum.PracticeFight)
            {
                xpAmount = MathF.Round((float)xpAmount * TournamentConfiguration.Instance.XPConfiguration.ArenaXPAdjustment);
            }
            if (missionType == CombatXpModel.MissionTypeEnum.Tournament)
            {
                xpAmount = MathF.Round((float)xpAmount * TournamentConfiguration.Instance.XPConfiguration.TournamentXPAdjustment);
            }
        }

        private static bool Prepare()
        {
            // return false; //For some reason it's not firing the GetXPFromHit function - reverting to model replacement method (bad form in my opinion)
            return (TournamentConfiguration.Instance.XPConfiguration.IsTournamentXPEnabled || TournamentConfiguration.Instance.XPConfiguration.IsArenaXPEnabled);
        }
    }
}