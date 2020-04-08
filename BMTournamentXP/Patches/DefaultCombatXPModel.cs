using BMTournamentXP.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Library;
using TournamentLib.Models;

namespace BMTournamentXPMain.Patch
{
    public class DefaultCombatXPModelPatchGetXpFromHit
    {
        [HarmonyPatch(typeof(DefaultCombatXpModel), "GetXpFromHit")]
        public static bool Prefix(ref DefaultCombatXpModel __instance, 
            CharacterObject attackerTroop, 
            CharacterObject attackedTroop, 
            int damage, bool isFatal, 
            CombatXpModel.MissionTypeEnum missionType, 
            ref int xpAmount)
        {
            GetXpFromHit(attackerTroop, attackedTroop, damage, isFatal, missionType, out xpAmount);
            return false;
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

        static bool Prepare()
        {
            //    return (TournamentConfiguration.Instance.XPConfiguration.IsTournamentXPEnabled || TournamentConfiguration.Instance.XPConfiguration.IsArenaXPEnabled);
            return false;
        }
    }
}
