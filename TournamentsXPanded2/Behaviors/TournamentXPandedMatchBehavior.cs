using SandBox.TournamentMissions.Missions;

using System.Collections.Generic;

using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

/*
 * public float BonusRenownMostKills { get; set; } = 0;
        public float BonusRenownMostDamage { get; set; } = 0;
        public float BonusRenownFirstKill { get; set; } = 0;
        public float BonusRenownLeastDamage { get; set; } = 0;*/

namespace TournamentsXPanded.Behaviors
{
    public class TournamentXPandedMatchListener : IMissionListener
    {
        private TournamentBehavior tournamentBehavior;

        public TournamentXPandedMatchListener(TournamentBehavior behavor)
        {
            tournamentBehavior = behavor;
        }

        public void OnConversationCharacterChanged()
        {
        }

        public void OnEndMission()
        {
        }

        public void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
        {
        }

        public void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
        {
            TournamentXPandedMatchBehavior._match = tournamentBehavior.CurrentMatch;
            TournamentXPandedMatchBehavior.firstKiller = null;
            TournamentXPandedMatchBehavior.achievements = new Dictionary<TournamentParticipant, ParticipantAchievements>();
            foreach (var p in TournamentXPandedMatchBehavior._match.Participants)
            {
                TournamentXPandedMatchBehavior.achievements.Add(p, new ParticipantAchievements());
            }
        }

        public void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
        {
        }

        public void OnResetMission()
        {
        }
    }

    public class TournamentXPandedMatchBehavior : MissionLogic, ITournamentGameBehavior
    {
        internal static TournamentParticipant firstKiller = null;
        internal static TournamentMatch _match;
        internal static Dictionary<TournamentParticipant, ParticipantAchievements> achievements;
        internal TournamentBehavior tournamentBehavior;

        //public override MissionBehaviourType BehaviourType { get; }

        public TournamentXPandedMatchBehavior(TournamentBehavior behavior)
        {
            tournamentBehavior = behavior;
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

            if (damage > 0 && affectedAgent.Team != affectorAgent.Team && _match != null)
            {
                try
                {
                    var pa = _match.GetParticipant(affectorAgent.Origin.UniqueSeed);
                    var pd = _match.GetParticipant(affectedAgent.Origin.UniqueSeed);
                    achievements[pa].DamageInfliced += damage;
                    achievements[pd].DamageTaken += damage;
                }
                catch { }
            }
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
        {
            if (!GameNetwork.IsClientOrReplay && !this.IsMatchEnded() && affectorAgent != null && affectedAgent != null && affectedAgent != affectorAgent && affectedAgent.IsHuman && affectorAgent.IsHuman && _match != null)
            {
                try
                {
                    var p = _match.GetParticipant(affectorAgent.Origin.UniqueSeed);
                    if (firstKiller == null)
                    {
                        firstKiller = p;
                    }
                    achievements[p].NumberOfKills++;
                }
                catch { }
            }
        }

        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, int weaponKind, int currentWeaponUsageIndex)
        {
        }

        public override void EarlyStart()
        {
        }

        public override void AfterStart()
        {
        }

        public void SkipMatch(TournamentMatch match)
        {
        }

        public bool IsMatchEnded()
        {
            return false;
        }

        public void OnMatchEnded()
        {
        }

        public static List<TournamentParticipant> MostKills()
        {
            int bestNumberOfKills = 0;
            List<TournamentParticipant> mostScore = new List<TournamentParticipant>();
            if (achievements != null)
            {
                foreach (var x in achievements.Keys)
                {
                    if (achievements[x].NumberOfKills > bestNumberOfKills)
                    {
                        mostScore = new List<TournamentParticipant>();
                        bestNumberOfKills = achievements[x].NumberOfKills;
                        mostScore.Add(x);
                    }
                    else if (achievements[x].NumberOfKills == bestNumberOfKills)
                    {
                        mostScore.Add(x);
                    }
                }
            }
            return mostScore;
        }

        public static List<TournamentParticipant> MostDamage()
        {
            float best = 0;
            List<TournamentParticipant> mostScore = new List<TournamentParticipant>();
            if (achievements != null)
            {
                foreach (var x in achievements.Keys)
                {
                    if (achievements[x].DamageInfliced > best)
                    {
                        mostScore = new List<TournamentParticipant>();
                        best = achievements[x].DamageInfliced;
                        mostScore.Add(x);
                    }
                    else if (achievements[x].NumberOfKills == best)
                    {
                        mostScore.Add(x);
                    }
                }
            }
            return mostScore;
        }

        public static List<TournamentParticipant> LeastDamage()
        {
            float best = 500000;
            List<TournamentParticipant> mostScore = new List<TournamentParticipant>();
            if (achievements != null)
            {
                foreach (var x in achievements.Keys)
                {
                    if (achievements[x].DamageTaken < best)
                    {
                        mostScore = new List<TournamentParticipant>();
                        best = achievements[x].DamageTaken;
                        mostScore.Add(x);
                    }
                    else if (achievements[x].DamageTaken == best)
                    {
                        mostScore.Add(x);
                    }
                }
            }
            return mostScore;
        }

        public void StartMatch(TournamentMatch match, bool isLastRound)
        {
        }
    }

    internal class ParticipantAchievements
    {
        public float DamageInfliced { get; set; } = 0;
        public float DamageTaken { get; set; } = 0;
        public int NumberOfKills { get; set; } = 0;
    }
}