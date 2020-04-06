﻿using BMTournamentXP.Models;
using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BMTournamentXP.Patches
{

    [HarmonyPatch(typeof(TournamentFightMissionController), "EnemyHitReward")]
    public class TournamentFightMissionControllerXPGainPatch
    {
        private bool Prefix (Agent affectedAgent, Agent affectorAgent, float lastSpeedBonus, float lastShotDifficulty, int lastWeaponKind, float hitpointRatio, int weaponUsageIndex, float damageAmount)
        {
            if (MBNetwork.IsClient)
            {
                return false;
            }
            CharacterObject character = (CharacterObject)affectedAgent.Character;
            CharacterObject characterObject = (CharacterObject)affectorAgent.Character;
            if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
            {
                damageAmount = BMTournamentXPConfiguration.Instance.TournamentXPAdjustment;
                SkillLevelingManager.OnCombatHit(characterObject, character, null, null, lastSpeedBonus, lastShotDifficulty, lastWeaponKind, hitpointRatio, false, affectorAgent.MountAgent != null, affectorAgent.Team == affectedAgent.Team, false, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);
            }
            return false;
        }

        static bool Prepare()
        {
            return BMTournamentXPConfiguration.Instance.IsTournamentXPEnabled && BMTournamentXPConfiguration.Instance.TournamentXPAdjustment != 1f;
        }
    }

    [HarmonyPatch(typeof(ArenaPracticeFightMissionController), "EnemyHitReward")]
    public class ArenaPracticeFightMissionControllerXPGainPatch
    {
        private bool Prefix(Agent affectedAgent, Agent affectorAgent, float lastSpeedBonus, float lastShotDifficulty, int lastWeaponKind, float hitpointRatio, int weaponUsageIndex, float damageAmount)
        {
            if (MBNetwork.IsClient)
            {
                return false;
            }
            CharacterObject character = (CharacterObject)affectedAgent.Character;
            CharacterObject characterObject = (CharacterObject)affectorAgent.Character;
            if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
            {
                damageAmount = BMTournamentXPConfiguration.Instance.TournamentXPAdjustment;
                SkillLevelingManager.OnCombatHit(characterObject, character, null, null, lastSpeedBonus, lastShotDifficulty, lastWeaponKind, hitpointRatio, false, affectorAgent.MountAgent != null, affectorAgent.Team == affectedAgent.Team, false, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);
            }
            return false;
        }

        static bool Prepare()
        {
            return BMTournamentXPConfiguration.Instance.IsArenaXPEnabled && BMTournamentXPConfiguration.Instance.ArenaXPAdjustment != 1f;
        }
    }
}
