using SandBox.TournamentMissions.Missions;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;


namespace TournamentsXPanded.Behaviors
{
    public class TournamentXPandedTournamentBehavior : MissionLogic, ITournamentGameBehavior
    {

        public override void AfterStart()
        {

        }
        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, int weaponKind, int currentWeaponUsageIndex)
        {

        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {

        }

        public void StartMatch(TournamentMatch match, bool isLastRound)
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


    }
}
