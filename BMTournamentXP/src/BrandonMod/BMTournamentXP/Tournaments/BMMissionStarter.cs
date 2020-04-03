using SandBox;
using SandBox.Source.Missions;
using SandBox.Source.Missions.Handlers;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace BMTournamentXP
{
    [MissionManager]
    public static class BMMissionStarter
    {
        [MissionMethod]
        public static Mission OpenBattleChallengeMission(string scene, IList<Hero> priorityCharsAttacker, IList<Hero> priorityCharsDefender)
        {
            return null;
        }

        [MissionMethod]
        public static Mission OpenTournamentArcheryMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
        {
            return MissionState.OpenNew("TournamentArchery", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false), (Mission missionController) => {
                TournamentArcheryMissionController tournamentArcheryMissionController = new BMTournamentArcheryMissionController(culture);
                return new MissionBehaviour[] { new CampaignMissionComponent(), tournamentArcheryMissionController, new TournamentBehavior(tournamentGame, settlement, tournamentArcheryMissionController, isPlayerParticipating), new AgentVictoryLogic(), new AgentBattleAILogic(), new ArenaAgentStateDeciderLogic(), new BasicLeaveMissionLogic(true), new MissionHardBorderPlacer(), new MissionBoundaryPlacer(), new MissionOptionsComponent() };
            }, true, true, true);
        }

        [MissionMethod]
        public static Mission OpenTournamentFightMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
        {
            return MissionState.OpenNew("TournamentFight", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", true), (Mission missionController) => {
                TournamentFightMissionController tournamentFightMissionController = new BMTournamentFightMissionController(culture);
                return new MissionBehaviour[] { new CampaignMissionComponent(), tournamentFightMissionController, new TournamentBehavior(tournamentGame, settlement, tournamentFightMissionController, isPlayerParticipating), new AgentVictoryLogic(), new AgentBattleAILogic(), new ArenaAgentStateDeciderLogic(), new MissionHardBorderPlacer(), new MissionBoundaryPlacer(), new MissionOptionsComponent(), new HighlightsController(), new SandboxHighlightsController() };
            }, true, true, true);
        }

        [MissionMethod]
        public static Mission OpenTournamentHorseRaceMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
        {
            return MissionState.OpenNew("TournamentHorseRace", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false), (Mission missionController) => {
                TownHorseRaceMissionController townHorseRaceMissionController = new BMTownHorseRaceMissionController(culture);
                return new MissionBehaviour[] { new CampaignMissionComponent(), townHorseRaceMissionController, new TournamentBehavior(tournamentGame, settlement, townHorseRaceMissionController, isPlayerParticipating), new AgentVictoryLogic(), new AgentBattleAILogic(), new ArenaAgentStateDeciderLogic(), new MissionDebugHandler(), new MissionHardBorderPlacer(), new MissionBoundaryPlacer(), new MissionOptionsComponent() };
            }, true, true, true);
        }

        [MissionMethod]
        public static Mission OpenTournamentJoustingMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
        {
            return MissionState.OpenNew("TournamentJousting", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false), (Mission missionController) => {
                TournamentJoustingMissionController tournamentJoustingMissionController = new BMTournamentJoustingMissionController(culture);
                return new MissionBehaviour[] { new CampaignMissionComponent(), tournamentJoustingMissionController, new TournamentBehavior(tournamentGame, settlement, tournamentJoustingMissionController, isPlayerParticipating), new AgentVictoryLogic(), new AgentBattleAILogic(), new ArenaAgentStateDeciderLogic(), new MissionHardBorderPlacer(), new MissionBoundaryPlacer(), new MissionBoundaryCrossingHandler(), new MissionOptionsComponent() };
            }, true, true, true);
        }
    }
}
