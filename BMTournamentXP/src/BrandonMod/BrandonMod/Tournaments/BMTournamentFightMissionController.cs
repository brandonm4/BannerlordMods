using SandBox;
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
using TaleWorlds.MountAndBlade;

namespace BMTournamentXP
{
  


    public class BMExperienceOnHitLogic : MissionLogic, ITournamentGameBehavior
    {
    
        public override MissionBehaviourType BehaviourType { get { return MissionBehaviourType.Other; } }

        public BMExperienceOnHitLogic()
        {
            
        }

        public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, int affectorWeaponKind, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, int weaponCurrentUsageIndex)
        {
            base.OnScoreHit(affectedAgent, affectorAgent, affectorWeaponKind, isBlocked, damage, movementSpeedDamageModifier, hitDistance, attackType, shotDifficulty, weaponCurrentUsageIndex);
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
            this.EnemyHitReward(affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponCurrentUsageIndex, damage);

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
            Hero heroObject;
            bool flag;            
            if (MBNetwork.IsClient)
            {
                return;
            }
            CharacterObject character = (CharacterObject)affectedAgent.Character;
            CharacterObject characterObject = (CharacterObject)affectorAgent.Character;
            if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
            {
                PartyBase battleCombatant = (PartyBase)affectorAgent.Origin.BattleCombatant;
                Hero captain = BMExperienceOnHitLogic.GetCaptain(affectorAgent);
                if (affectorAgent.Team.Leader == null || !affectorAgent.Team.Leader.Character.IsHero)
                {
                    heroObject = null;
                }
                else
                {
                    heroObject = ((CharacterObject)affectorAgent.Team.Leader.Character).HeroObject;
                }
                Hero hero = heroObject;
                CharacterObject characterObject1 = characterObject;
                CharacterObject characterObject2 = character;
                Hero hero1 = captain;
                Hero hero2 = hero;
                float single = lastSpeedBonus;
                float single1 = lastShotDifficulty;
                int num = lastWeaponKind;
                float single2 = hitpointRatio;
                bool mountAgent = affectorAgent.MountAgent != null;
                bool team = affectorAgent.Team == affectedAgent.Team;
                if (hero == null || affectorAgent.Character == hero.CharacterObject)
                {
                    flag = false;
                }
                else if (hero != Hero.MainHero)
                {
                    flag = true;
                }
                else
                {
                    flag = (affectorAgent.Formation == null ? true : !affectorAgent.Formation.IsAIControlled);
                }
                SkillLevelingManager.OnCombatHit(characterObject1, characterObject2, hero1, hero2, single, single1, num, single2, false, mountAgent, team, flag, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);
               
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

    public class BMTournamentFightMissionController : TournamentFightMissionController
    {
        public BMTournamentFightMissionController(CultureObject culture) : base(culture)
        {
        }

        public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, int affectorWeaponKind, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, int weaponCurrentUsageIndex)
        {
            base.OnScoreHit(affectedAgent, affectorAgent, affectorWeaponKind, isBlocked, damage, movementSpeedDamageModifier, hitDistance, attackType, shotDifficulty, weaponCurrentUsageIndex);
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
            this.EnemyHitReward(affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponCurrentUsageIndex, damage);

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
            Hero heroObject;
            bool flag;
            if (MBNetwork.IsClient)
            {
                return;
            }
            CharacterObject character = (CharacterObject)affectedAgent.Character;
            CharacterObject characterObject = (CharacterObject)affectorAgent.Character;
            if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
            {
                PartyBase battleCombatant = (PartyBase)affectorAgent.Origin.BattleCombatant;
                Hero captain = BMTournamentFightMissionController.GetCaptain(affectorAgent);
                if (affectorAgent.Team.Leader == null || !affectorAgent.Team.Leader.Character.IsHero)
                {
                    heroObject = null;
                }
                else
                {
                    heroObject = ((CharacterObject)affectorAgent.Team.Leader.Character).HeroObject;
                }
                Hero hero = heroObject;
                CharacterObject characterObject1 = characterObject;
                CharacterObject characterObject2 = character;
                Hero hero1 = captain;
                Hero hero2 = hero;
                float single = lastSpeedBonus;
                float single1 = lastShotDifficulty;
                int num = lastWeaponKind;
                float single2 = hitpointRatio;
                bool mountAgent = affectorAgent.MountAgent != null;
                bool team = affectorAgent.Team == affectedAgent.Team;
                if (hero == null || affectorAgent.Character == hero.CharacterObject)
                {
                    flag = false;
                }
                else if (hero != Hero.MainHero)
                {
                    flag = true;
                }
                else
                {
                    flag = (affectorAgent.Formation == null ? true : !affectorAgent.Formation.IsAIControlled);
                }
                SkillLevelingManager.OnCombatHit(characterObject1, characterObject2, hero1, hero2, single, single1, num, single2, false, mountAgent, team, flag, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);

            }
        }

    }

    public class BMTournamentArcheryMissionController : TournamentArcheryMissionController
    {
        public BMTournamentArcheryMissionController(CultureObject culture) : base(culture)
        {
        }

        public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, int affectorWeaponKind, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, int weaponCurrentUsageIndex)
        {
            base.OnScoreHit(affectedAgent, affectorAgent, affectorWeaponKind, isBlocked, damage, movementSpeedDamageModifier, hitDistance, attackType, shotDifficulty, weaponCurrentUsageIndex);
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
            this.EnemyHitReward(affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponCurrentUsageIndex, damage);

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
            Hero heroObject;
            bool flag;
            if (MBNetwork.IsClient)
            {
                return;
            }
            CharacterObject character = (CharacterObject)affectedAgent.Character;
            CharacterObject characterObject = (CharacterObject)affectorAgent.Character;
            if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
            {
                PartyBase battleCombatant = (PartyBase)affectorAgent.Origin.BattleCombatant;
                Hero captain = BMTournamentArcheryMissionController.GetCaptain(affectorAgent);
                if (affectorAgent.Team.Leader == null || !affectorAgent.Team.Leader.Character.IsHero)
                {
                    heroObject = null;
                }
                else
                {
                    heroObject = ((CharacterObject)affectorAgent.Team.Leader.Character).HeroObject;
                }
                Hero hero = heroObject;
                CharacterObject characterObject1 = characterObject;
                CharacterObject characterObject2 = character;
                Hero hero1 = captain;
                Hero hero2 = hero;
                float single = lastSpeedBonus;
                float single1 = lastShotDifficulty;
                int num = lastWeaponKind;
                float single2 = hitpointRatio;
                bool mountAgent = affectorAgent.MountAgent != null;
                bool team = affectorAgent.Team == affectedAgent.Team;
                if (hero == null || affectorAgent.Character == hero.CharacterObject)
                {
                    flag = false;
                }
                else if (hero != Hero.MainHero)
                {
                    flag = true;
                }
                else
                {
                    flag = (affectorAgent.Formation == null ? true : !affectorAgent.Formation.IsAIControlled);
                }
                SkillLevelingManager.OnCombatHit(characterObject1, characterObject2, hero1, hero2, single, single1, num, single2, false, mountAgent, team, flag, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);

            }
        }
    }

    public class BMTournamentJoustingMissionController : TournamentJoustingMissionController
    {
        public BMTournamentJoustingMissionController(CultureObject culture) : base(culture)
        {
        }

        public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, int affectorWeaponKind, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, int weaponCurrentUsageIndex)
        {
            base.OnScoreHit(affectedAgent, affectorAgent, affectorWeaponKind, isBlocked, damage, movementSpeedDamageModifier, hitDistance, attackType, shotDifficulty, weaponCurrentUsageIndex);
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
            this.EnemyHitReward(affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponCurrentUsageIndex, damage);

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
            Hero heroObject;
            bool flag;
            if (MBNetwork.IsClient)
            {
                return;
            }
            CharacterObject character = (CharacterObject)affectedAgent.Character;
            CharacterObject characterObject = (CharacterObject)affectorAgent.Character;
            if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
            {
                PartyBase battleCombatant = (PartyBase)affectorAgent.Origin.BattleCombatant;
                Hero captain = BMTournamentJoustingMissionController.GetCaptain(affectorAgent);
                if (affectorAgent.Team.Leader == null || !affectorAgent.Team.Leader.Character.IsHero)
                {
                    heroObject = null;
                }
                else
                {
                    heroObject = ((CharacterObject)affectorAgent.Team.Leader.Character).HeroObject;
                }
                Hero hero = heroObject;
                CharacterObject characterObject1 = characterObject;
                CharacterObject characterObject2 = character;
                Hero hero1 = captain;
                Hero hero2 = hero;
                float single = lastSpeedBonus;
                float single1 = lastShotDifficulty;
                int num = lastWeaponKind;
                float single2 = hitpointRatio;
                bool mountAgent = affectorAgent.MountAgent != null;
                bool team = affectorAgent.Team == affectedAgent.Team;
                if (hero == null || affectorAgent.Character == hero.CharacterObject)
                {
                    flag = false;
                }
                else if (hero != Hero.MainHero)
                {
                    flag = true;
                }
                else
                {
                    flag = (affectorAgent.Formation == null ? true : !affectorAgent.Formation.IsAIControlled);
                }
                SkillLevelingManager.OnCombatHit(characterObject1, characterObject2, hero1, hero2, single, single1, num, single2, false, mountAgent, team, flag, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);

            }
        }
    }

    public class BMTownHorseRaceMissionController : TownHorseRaceMissionController
    {
        public BMTownHorseRaceMissionController(CultureObject culture) : base(culture)
        {
        }

        public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, int affectorWeaponKind, bool isBlocked, float damage, float movementSpeedDamageModifier, float hitDistance, AgentAttackType attackType, float shotDifficulty, int weaponCurrentUsageIndex)
        {
            base.OnScoreHit(affectedAgent, affectorAgent, affectorWeaponKind, isBlocked, damage, movementSpeedDamageModifier, hitDistance, attackType, shotDifficulty, weaponCurrentUsageIndex);
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
            this.EnemyHitReward(affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponCurrentUsageIndex, damage);

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
            Hero heroObject;
            bool flag;
            if (MBNetwork.IsClient)
            {
                return;
            }
            CharacterObject character = (CharacterObject)affectedAgent.Character;
            CharacterObject characterObject = (CharacterObject)affectorAgent.Character;
            if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
            {
                PartyBase battleCombatant = (PartyBase)affectorAgent.Origin.BattleCombatant;
                Hero captain = BMTownHorseRaceMissionController.GetCaptain(affectorAgent);
                if (affectorAgent.Team.Leader == null || !affectorAgent.Team.Leader.Character.IsHero)
                {
                    heroObject = null;
                }
                else
                {
                    heroObject = ((CharacterObject)affectorAgent.Team.Leader.Character).HeroObject;
                }
                Hero hero = heroObject;
                CharacterObject characterObject1 = characterObject;
                CharacterObject characterObject2 = character;
                Hero hero1 = captain;
                Hero hero2 = hero;
                float single = lastSpeedBonus;
                float single1 = lastShotDifficulty;
                int num = lastWeaponKind;
                float single2 = hitpointRatio;
                bool mountAgent = affectorAgent.MountAgent != null;
                bool team = affectorAgent.Team == affectedAgent.Team;
                if (hero == null || affectorAgent.Character == hero.CharacterObject)
                {
                    flag = false;
                }
                else if (hero != Hero.MainHero)
                {
                    flag = true;
                }
                else
                {
                    flag = (affectorAgent.Formation == null ? true : !affectorAgent.Formation.IsAIControlled);
                }
                SkillLevelingManager.OnCombatHit(characterObject1, characterObject2, hero1, hero2, single, single1, num, single2, false, mountAgent, team, flag, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);

            }
        }
    }
}
