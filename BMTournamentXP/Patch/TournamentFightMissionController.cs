//using HarmonyLib;
//using SandBox;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TaleWorlds.Core;
//using TaleWorlds.MountAndBlade;

//namespace BMTournamentXP.Patch
//{
//    [HarmonyPatch(typeof(TournamentFightMissionController), "OnScoreHit")]
//    public class TournamentFightMissionControllerPatch1 
//    {
//        public TournamentFightMissionControllerPatch1()
//        {

//        }
//        static bool Prefix(TournamentFightMissionController __instance, Agent affectedAgent, Agent affectorAgent, int affectorWeaponKind, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, int weaponCurrentUsageIndex)
//        {
//            if (BMTournamentXPMain.Configuration.IsTournamentXPEnabled)
//            {
//                if (affectorAgent.IsPlayerControlled && affectorAgent.IsHero)
//                {
//                    __instance.OnScoreHit(affectedAgent, affectorAgent, affectorWeaponKind, isBlocked, damage, movementSpeedDamageModifier, hitDistance, attackType, shotDifficulty, weaponCurrentUsageIndex);
//                    if (affectorAgent == null)
//                    {
//                        return false;
//                    }
//                    if (affectorAgent.Character == null || affectedAgent.Character == null)
//                    {
//                        return false;
//                    }
//                    if (damage > affectedAgent.HealthLimit)
//                    {
//                        damage = affectedAgent.HealthLimit;
//                    }
//                    float single = damage / affectedAgent.HealthLimit;
//                    single = single * BMTournamentXPMain.Configuration.TournamentXPAdjustment;
//                    typeof(TournamentFightMissionController).GetMethod("EnemyHitReward").Invoke(__instance, new object[] { affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponCurrentUsageIndex, damage });
//                    //Traverse.Create<TournamentFightMissionController>().Method("EnemyHitReward").GetValue();
//                    //__instance.EnemyHitReward(affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponCurrentUsageIndex, damage);
//                }
//                return false; 
//            }
//            return true;
//        }
//        /*
//        public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, int affectorWeaponKind, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, int weaponCurrentUsageIndex)
//        {
//            base.OnScoreHit(affectedAgent, affectorAgent, affectorWeaponKind, isBlocked, damage, movementSpeedDamageModifier, hitDistance, attackType, shotDifficulty, weaponCurrentUsageIndex);
//        }
//        */
//    }
//}
