using Helpers;
using SandBox;
using SandBox.Source.Missions;
using SandBox.Source.Missions.Handlers;
using SandBox.Source.Missions.Handlers.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;
using static TaleWorlds.Library.CommandLineFunctionality;

namespace BMTournamentXP
{
    [MissionManager]
    public static class BMSandBoxMissions
    {
   
        static BMSandBoxMissions()
        {
           
        }

        public static MissionInitializerRecord CreateSandBoxMissionInitializerRecord(string sceneName, string sceneLevels = "", bool doNotUseLoadingScreen = false)
        {
            AtmosphereInfo atmosphereModel;
            MissionInitializerRecord missionInitializerRecord = new MissionInitializerRecord(sceneName)
            {
                DamageToPlayerMultiplier = Campaign.Current.Models.DifficultyModel.GetDamageToPlayerMultiplier(),
                DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetDamageToFriendsMultiplier(),
                PlayingInCampaignMode = Campaign.Current.GameMode == CampaignGameMode.Campaign
            };
            if (Campaign.Current.GameMode == CampaignGameMode.Campaign)
            {
                atmosphereModel = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, MobileParty.MainParty.GetLogicalPosition());
            }
            else
            {
                atmosphereModel = null;
            }
            missionInitializerRecord.AtmosphereOnCampaign = atmosphereModel;
            missionInitializerRecord.SceneLevels = sceneLevels;
            missionInitializerRecord.DoNotUseLoadingScreen = doNotUseLoadingScreen;
            return missionInitializerRecord;
        }


        [MissionMethod]
        public static Mission OpenArenaDuelMission(string scene, Location location, CharacterObject duelCharacter, bool requireCivilianEquipment, bool spawnBOthSidesWithHorse, Action<CharacterObject> onDuelEnd, float customAgentHealth, string sceneLevels = "")
        {
            return MissionState.OpenNew("ArenaDuelMission", BMSandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false), (Mission mission) => new MissionBehaviour[] { new MissionOptionsComponent(), new ArenaDuelMissionController(duelCharacter, requireCivilianEquipment, spawnBOthSidesWithHorse, onDuelEnd, customAgentHealth), new BasicLeaveMissionLogic(), new MissionFacialAnimationHandler(), new MissionDebugHandler(), new AgentBattleAILogic(), new ArenaAgentStateDeciderLogic(), new VisualTrackerMissionBehavior(), new CampaignMissionComponent(), new MissionAgentHandler(location, null), new BMExperienceOnHitLogic() }, true, true, true);
        }

        [MissionMethod]
        public static Mission OpenArenaDuelMission(string scene, Location location)
        {
            return MissionState.OpenNew("ArenaDuel", BMSandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false), (Mission mission) => new MissionBehaviour[] { new MissionOptionsComponent(), new CampaignMissionComponent(), new ArenaDuelMissionBehaviour(), new BasicLeaveMissionLogic(), new MissionAgentHandler(location, null), new HeroSkillHandler(), new MissionFacialAnimationHandler(), new MissionDebugHandler(), new AgentBattleAILogic(), new ArenaAgentStateDeciderLogic(), new BMExperienceOnHitLogic() }, true, true, true);
        }

        [MissionMethod]
        public static Mission OpenArenaStartMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = "")
        {
            return MissionState.OpenNew("ArenaPracticeFight", BMSandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false), (Mission mission) => new MissionBehaviour[] { new MissionOptionsComponent(), new ArenaPracticeFightMissionController(), new BasicLeaveMissionLogic(), new MissionConversationHandler(talkToChar), new HeroSkillHandler(), new MissionFacialAnimationHandler(), new MissionDebugHandler(), new AgentBattleAILogic(), new ArenaAgentStateDeciderLogic(), new VisualTrackerMissionBehavior(), new CampaignMissionComponent(), new MissionAgentHandler(location, null), new BMExperienceOnHitLogic() }, true, true, true);
        }
    }
}