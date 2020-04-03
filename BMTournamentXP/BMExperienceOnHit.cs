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
            this.EnemyHitReward(affectedAgent, affectorAgent, movementSpeedDamageModifier, shotDifficulty, affectorWeaponKind, 0.5f * single, weaponCurrentUsageIndex, damage * _xpmod);

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

                BMExperienceOnHitLogic.OnCombatHit(characterObject1, characterObject2, hero1, hero2, single, single1, num, single2, false, mountAgent, team, flag, weaponUsageIndex, damageAmount, affectedAgent.Health < 1f);


            }
        }

        public static void OnCombatHit(CharacterObject affectorCharacter, CharacterObject affectedCharacter, Hero captainHero, Hero commander, float speedBonusFromMovement, float shotDifficulty, int affectorWeaponKind, float hitPointRatio, bool isSimulatedHit, bool isAffectorMounted, bool isTeamKill, bool isAffectorUnderCommand, int weaponUsageIndex, float damageAmount, bool isFatal)
        {
            int num;
            float single = 1f;
            if (!isTeamKill)
            {
                if (affectorCharacter.IsHero)
                {
                    Hero heroObject = affectorCharacter.HeroObject;
                    ItemObject itemFromWeaponKind = ItemObject.GetItemFromWeaponKind(affectorWeaponKind);
                    Campaign.Current.Models.CombatXpModel.GetXpFromHit(heroObject.CharacterObject, affectedCharacter, (int)damageAmount, isFatal, isSimulatedHit, out num);
                    single = (float)num;
                    if (itemFromWeaponKind == null)
                    {
                        heroObject.AddSkillXp(DefaultSkills.Athletics, MBRandom.RoundRandomized(single));
                    }
                    else
                    {
                        SkillObject skillForWeapon = Campaign.Current.Models.CombatXpModel.GetSkillForWeapon(itemFromWeaponKind, weaponUsageIndex);
                        float single1 = (skillForWeapon == DefaultSkills.Bow ? 0.5f : 1f);
                        if (shotDifficulty > 0f)
                        {
                            single += (float)MBMath.Floor(single * single1 * Campaign.Current.Models.CombatXpModel.GetXpMultiplierFromShotDifficulty(shotDifficulty));
                        }
                        if (heroObject == Hero.MainHero)
                            heroObject.AddSkillXp(skillForWeapon, MBRandom.RoundRandomized(single));
                    }
                    if (!isAffectorMounted)
                    {
                        float single2 = 0.2f;
                        if (speedBonusFromMovement > 0f)
                        {
                            single2 = single2 + 1.5f * speedBonusFromMovement;
                        }
                        if (single2 > 0f)
                        {
                            if (heroObject == Hero.MainHero)
                                heroObject.AddSkillXp(DefaultSkills.Athletics, MBRandom.RoundRandomized(single2 * single));
                        }
                    }
                    else
                    {
                        float single3 = 0.1f;
                        if (speedBonusFromMovement > 0f)
                        {
                            single3 = single3 * (1f + speedBonusFromMovement);
                        }
                        else if (shotDifficulty - 1f > 0f)
                        {
                            int num1 = MathF.Round(shotDifficulty - 1f);
                            if (num1 > 0)
                            {
                                single3 += (float)num1;
                            }
                        }
                        if (single3 > 0f)
                        {
                            if (heroObject == Hero.MainHero)
                                heroObject.AddSkillXp(DefaultSkills.Riding, MBRandom.RoundRandomized(single3 * single));
                        }
                    }
                }
                if (commander != null && commander != affectorCharacter.HeroObject && commander.PartyBelongedTo != null && commander == Hero.MainHero)
                {
                    BMExperienceOnHitLogic.OnTacticsUsed(commander.PartyBelongedTo, MathF.Ceiling(0.02f * single));
                }
            }
        }
        public static void OnTacticsUsed(MobileParty party, int xp)
        {
            if (xp > 0)
            {
                BMExperienceOnHitLogic.OnPartySkillExercised(party, DefaultSkills.Tactics, xp, SkillEffect.PerkRole.None);
            }
        }
        private static void OnPartySkillExercised(MobileParty party, SkillObject skill, int skillXp, SkillEffect.PerkRole perkRole = 0)
        {
            if (party.LeaderHero == Hero.MainHero)
            {
                party.LeaderHero.AddSkillXp(skill, skillXp);
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
