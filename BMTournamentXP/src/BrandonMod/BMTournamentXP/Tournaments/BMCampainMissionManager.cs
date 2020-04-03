using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BMTournamentXP
{
    public class BMCampaignMissionManager : ICampaignMissionManager
    {
        public BMCampaignMissionManager()
        {
        }

        private static Type GetSiegeWeaponType(SiegeEngineType siegeWeaponType)
        {
            if (siegeWeaponType == DefaultSiegeEngineTypes.Ladder)
            {
                return typeof(SiegeLadder);
            }
            if (siegeWeaponType == DefaultSiegeEngineTypes.Ballista)
            {
                return typeof(Ballista);
            }
            if (siegeWeaponType == DefaultSiegeEngineTypes.FireBallista)
            {
                return typeof(FireBallista);
            }
            if (siegeWeaponType == DefaultSiegeEngineTypes.Ram)
            {
                return typeof(BatteringRam);
            }
            if (siegeWeaponType == DefaultSiegeEngineTypes.SiegeTower)
            {
                return typeof(SiegeTower);
            }
            if (siegeWeaponType == DefaultSiegeEngineTypes.Onager || siegeWeaponType == DefaultSiegeEngineTypes.Catapult)
            {
                return typeof(Mangonel);
            }
            if (siegeWeaponType == DefaultSiegeEngineTypes.FireOnager || siegeWeaponType == DefaultSiegeEngineTypes.FireCatapult)
            {
                return typeof(FireMangonel);
            }
            if (siegeWeaponType != DefaultSiegeEngineTypes.Trebuchet)
            {
                return null;
            }
            return typeof(Trebuchet);
        }

        private static Dictionary<Type, int> GetSiegeWeaponTypes(Dictionary<SiegeEngineType, int> values)
        {
            Dictionary<Type, int> types = new Dictionary<Type, int>();
            foreach (KeyValuePair<SiegeEngineType, int> value in values)
            {
                types.Add(BMCampaignMissionManager.GetSiegeWeaponType(value.Key), value.Value);
            }
            return types;
        }

        public IMission OpenArenaDuelMission(string scene, Location location, CharacterObject duelCharacter, bool requireCivilianEquipment, bool spawnBOthSidesWithHorse, Action<CharacterObject> onDuelEndAction, float customAgentHealth)
        {
            return BMSandBoxMissions.OpenArenaDuelMission(scene, location, duelCharacter, requireCivilianEquipment, spawnBOthSidesWithHorse, onDuelEndAction, customAgentHealth, "");
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenAlleyFightMission(string scene, int upgradeLevel)
        {
            return SandBoxMissions.OpenAlleyFightMission(scene, upgradeLevel);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenAmbushBattleMission(string scene, bool isPlayerAmbusher)
        {
            return SandBoxMissions.OpenAmbushBattleMission(scene, isPlayerAmbusher);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenArenaStartMission(string scene, Location location, CharacterObject talkToChar)
        {
            return BMSandBoxMissions.OpenArenaStartMission(scene, location, talkToChar, "");
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenBattleMission(MissionInitializerRecord rec)
        {
            return SandBoxMissions.OpenBattleMission(rec);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenBattleMission(string scene)
        {
            return SandBoxMissions.OpenBattleMission(scene);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenBattleMissionWhileEnteringSettlement(string scene, int upgradeLevel, int numberOfMaxTroopToBeSpawnedForPlayer, int numberOfMaxTroopToBeSpawnedForOpponent)
        {
            return SandBoxMissions.OpenBattleMissionWhileEnteringSettlement(scene, upgradeLevel, numberOfMaxTroopToBeSpawnedForPlayer, numberOfMaxTroopToBeSpawnedForOpponent);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenCampMission(string scene)
        {
            return SandBoxMissions.OpenCampMission(scene);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenCaravanBattleMission(MissionInitializerRecord rec, bool isCaravan)
        {
            return SandBoxMissions.OpenCaravanBattleMission(rec, isCaravan);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenCastleCourtyardMission(string scene, int castleUpgradeLevel, Location location, CharacterObject talkToChar)
        {
            return SandBoxMissions.OpenCastleCourtyardMission(scene, castleUpgradeLevel, location, talkToChar);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenCombatMissionWithDialogue(string scene, CharacterObject characterToTalkTo, int upgradeLevel)
        {
            return SandBoxMissions.OpenCombatMissionWithDialogue(scene, characterToTalkTo, upgradeLevel);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenConversationMission(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData, string specialScene, string sceneLevels)
        {
            return SandBoxMissions.OpenConversationMission(playerCharacterData, conversationPartnerData, specialScene, sceneLevels);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenConversationMission(List<ConversationCharacterData> conversationPlayerSidePartners, List<ConversationCharacterData> conversationOtherSidePartners, ConversationCharacterData firstCharacterToTalk, string specialScene, string sceneLevels)
        {
            return SandBoxMissions.OpenConversationMission(conversationPlayerSidePartners, conversationOtherSidePartners, firstCharacterToTalk, specialScene, sceneLevels);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenConversatonTestMission(string scene)
        {
            return TestMissions.OpenFacialAnimTestMission(scene, "");
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenEquipmentTestMission(string scene)
        {
            return TestMissions.OpenEquipmentTestMission(scene);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenHideoutBattleMission(string scene)
        {
            return SandBoxMissions.OpenHideoutBattleMission(scene);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenIndoorMission(string scene, Location location, CharacterObject talkToChar)
        {
            return SandBoxMissions.OpenIndoorMission(scene, location, talkToChar, "");
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenMeetingMission(string scene, CharacterObject character)
        {
            return SandBoxMissions.OpenMeetingMission(scene, character);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenSiegeMissionNoDeployment(string scene, bool isSallyOut, bool isReliefForceAttack)
        {
            return SandBoxMissions.OpenSiegeMissionNoDeployment(scene, isSallyOut, isReliefForceAttack);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenSiegeMissionWithDeployment(string scene, float[] wallHitPointsPercentages, bool hasAnySiegeTower, Dictionary<SiegeEngineType, int> siegeWeaponsCountOfAttackers, Dictionary<SiegeEngineType, int> siegeWeaponsCountOfDefenders, bool isPlayerAttacker, int upgradeLevel, bool isSallyOut, bool isReliefForceAttack)
        {
            return SandBoxMissions.OpenSiegeMissionWithDeployment(scene, wallHitPointsPercentages, hasAnySiegeTower, BMCampaignMissionManager.GetSiegeWeaponTypes(siegeWeaponsCountOfAttackers), BMCampaignMissionManager.GetSiegeWeaponTypes(siegeWeaponsCountOfDefenders), isPlayerAttacker, upgradeLevel, isSallyOut, isReliefForceAttack);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenSneakIntoTownFightMission(string scene, Location location, int townUpgradeLevel)
        {
            return SandBoxMissions.OpenSneakIntoTownFightMission(scene, location, townUpgradeLevel);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenSneakMission(string scene)
        {
            return SandBoxMissions.OpenSneakTeam3Mission(scene);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenTownAmbushMission(string scene, Location location)
        {
            return SandBoxMissions.OpenTownAmbushMission(scene, location);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenTownCenterMission(string scene, int townUpgradeLevel, Location location, CharacterObject talkToChar, string playerSpawnTag)
        {
            return SandBoxMissions.OpenTownCenterMission(scene, townUpgradeLevel, location, talkToChar, playerSpawnTag);
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenTownNightStealthMission(string scene, Location location)
        {
            return SandBoxMissions.OpenTownNightStealthMission(scene, location, "");
        }

        IMission TaleWorlds.CampaignSystem.ICampaignMissionManager.OpenVillageMission(string scene, Location location, CharacterObject talkToChar)
        {
            return SandBoxMissions.OpenVillageMission(scene, location, talkToChar, null);
        }
    }
}
