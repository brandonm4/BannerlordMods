using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BMTournamentXP
{
    public class BMExperienceOnHitLogic : MissionLogic, ITournamentGameBehavior
    {
        private float _xpmod = 1;


        public override MissionBehaviourType BehaviourType { get { return MissionBehaviourType.Other; } }

        public BMExperienceOnHitLogic(float xpmod)
        {
            _xpmod = xpmod;
        }

        

        public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, int affectorWeaponKind, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, int weaponUsageIndex)
        {            
            if (affectorAgent == null)
            {
                return;
            }
            if (affectorAgent.Character == null || affectedAgent.Character == null)
            {
                return;
            }
            if (damage > affectedAgent.HealthLimit)
            {
                damage = affectedAgent.HealthLimit;
            }
            float single = damage / affectedAgent.HealthLimit;
            this.EnemyHitReward(affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponUsageIndex, damage * _xpmod);
        }


        private static Hero GetCaptain(Agent affectorAgent)
        {
            Hero heroObject = null;
            if (affectorAgent.Formation != null)
            {
                Agent captain = affectorAgent.Formation.Captain;
                if (captain != null)
                {
                    float captainRadius = Campaign.Current.Models.CombatXpModel.CaptainRadius;
                    if (captain.Position.Distance(affectorAgent.Position) < captainRadius)
                    {
                        heroObject = ((CharacterObject)captain.Character).HeroObject;
                    }
                }
            }
            return heroObject;
        }

        private void EnemyHitReward(Agent affectedAgent, Agent affectorAgent, float lastSpeedBonus, float lastShotDifficulty, int lastWeaponKind, float hitpointRatio, int weaponUsageIndex, float damageAmount)
        {
            if (MBNetwork.IsClient)
            {
                return;
            }
            CharacterObject character = (CharacterObject)affectedAgent.Character;
            CharacterObject characterObject = (CharacterObject)affectorAgent.Character;
            if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
            {
                SkillLevelingManager.OnCombatHit(characterObject, character, null, null, lastSpeedBonus, lastShotDifficulty, lastWeaponKind, hitpointRatio, CombatXpModel.MissionTypeEnum.Tournament, affectorAgent.MountAgent != null, affectorAgent.Team == affectedAgent.Team, false, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);
            }

        }
        public void StartMatch(TournamentMatch match, bool isLastRound)
        {
            //    throw new NotImplementedException();
        }

        public void SkipMatch(TournamentMatch match)
        {
            // throw new NotImplementedException();
        }

        public bool IsMatchEnded()
        {
            // throw new NotImplementedException();
            return false;
        }

        public void OnMatchEnded()
        {
            //   throw new NotImplementedException();
        }
    }
}
