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
using TaleWorlds.CampaignSystem.Character;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.SandBox;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace BrandonMod
{
    public class BMSandboxManager : SandBoxManager
    {
        public BMSandboxManager() : base()
        {           
        }
        private static void AddGameTests(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddGameMenu("leave", "{=!}You shouldn't see this.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.AutoSelectFirst, null);
            gameInitializer.AddGameMenuOption("leave", "none", "", null, new GameMenuOption.OnConsequenceDelegate(GameMenusCallbacks.LeaveEncounterOnConsequence), false, -1, false);
            GenericTestStatics.AddGameMenus(gameInitializer);
            CampaignSiegeTestStatic.AddGameMenu(gameInitializer);
        }
        new public static void OnGameStart(CampaignGameStarter gameStarter)
        {
            //gameStarter.LoadGameTexts(String.Concat(BasePath.Name, "Modules/Native/ModuleData/global_strings.xml"));
            //gameStarter.LoadGameTexts(String.Concat(BasePath.Name, "Modules/Native/ModuleData/module_strings.xml"));
            //gameStarter.LoadGameTexts(String.Concat(BasePath.Name, "Modules/Native/ModuleData/native_strings.xml"));
            //gameStarter.LoadGameTexts(String.Concat(BasePath.Name, "Modules/SandBox/ModuleData/module_strings.xml"));
            //gameStarter.LoadGameMenus(typeof(GameMenusCallbacks), String.Concat(BasePath.Name, "Modules/SandBox/ModuleData/game_menus.xml"));
            //BMSandboxManager.AddGameTests(gameStarter);
            //gameStarter.AddModel(new DefaultSkillList());
            //gameStarter.AddModel(new DefaultCharacterDevelopmentModel());
            //gameStarter.AddModel(new DefaultValuationModel());
            //gameStarter.AddModel(new DefaultItemUsabilityModel());
            //gameStarter.AddModel(new DefaultStartEncounterModel());
            //gameStarter.AddModel(new DefaultMapVisibilityModel());
            //gameStarter.AddModel(new DefaultMapDistanceModel());
            //gameStarter.AddModel(new DefaultMapVisibilityListener());
            //gameStarter.AddModel(new DefaultPartyHealingModel());
            //gameStarter.AddModel(new DefaultPartyTrainingModel());
            //gameStarter.AddModel(new DefaultPartyTradeModel());
            //gameStarter.AddModel(new DefaultCombatSimulationModel());
            //gameStarter.AddModel(new DefaultCombatXpModel());
            //gameStarter.AddModel(new DefaultGenericXpModel());
            //gameStarter.AddModel(new DefaultSmithingModel());
            //gameStarter.AddModel(new DefaultPartySpeedCalculatingModel());
            //gameStarter.AddModel(new DefaultPartyImpairmentModel());
            //gameStarter.AddModel(new DefaultCharacterStatsModel());
            //gameStarter.AddModel(new DefaultMobilePartyFoodConsumptionModel());
            //gameStarter.AddModel(new DefaultPartyFoodBuyingModel());
            //gameStarter.AddModel(new DefaultPartyMoraleModel());
            //gameStarter.AddModel(new DefaultDiplomacyModel());
            //gameStarter.AddModel(new DefaultVillageProductionCalculatorModel());
            //gameStarter.AddModel(new DefaultVolunteerProductionModel());
            //gameStarter.AddModel(new DefaultArmyManagementCalculationModel());
            //gameStarter.AddModel(new DefaultBanditDensityModel());
            //gameStarter.AddModel(new DefaultPlayerCaptivityModel());
            //gameStarter.AddModel(new DefaultEncounterGameMenuModel());
            //gameStarter.AddModel(new DefaultBattleRewardModel());
            //gameStarter.AddModel(new DefaultMapTrackModel());
            //gameStarter.AddModel(new DefaultMapWeatherModel());
            //gameStarter.AddModel(new DefaultRidingModel());
            //gameStarter.AddModel(new DefaultStrikeMagnitudeModel());
            //gameStarter.AddModel(new DefaultTargetScoreCalculatingModel());
            //gameStarter.AddModel(new DefaultCrimeModel());
            //gameStarter.AddModel(new DefaultDisguiseDetectionModel());
            //gameStarter.AddModel(new DefaultBribeCalculationModel());
            //gameStarter.AddModel(new DefaultTroopSacrificeModel());
            //gameStarter.AddModel(new DefaultSettlementAccessModel());
            //gameStarter.AddModel(new DefaultKingdomDecisionPermissionModel());
            //gameStarter.AddModel(new DefaultPartySizeLimitModel());
            //gameStarter.AddModel(new DefaultPartyWageModel());
            //gameStarter.AddModel(new DefaultInventoryCapacityModel());
            //gameStarter.AddModel(new DefaultFaceGenAttributeChangerModel());
            //gameStarter.AddModel(new DefaultItemCategorySelector());
            //gameStarter.AddModel(new DefaultItemValueModel());
            //gameStarter.AddModel(new DefaultTradeItemPriceFactorModel());
            //gameStarter.AddModel(new DefaultSettlementValueModel());
            //gameStarter.AddModel(new DefaultSettlementMilitiaModel());
            //gameStarter.AddModel(new DefaultSettlementEconomyModel());
            //gameStarter.AddModel(new DefaultSettlementFoodModel());
            //gameStarter.AddModel(new DefaultSettlementLoyaltyModel());
            //gameStarter.AddModel(new DefaultSettlementSecurityModel());
            //gameStarter.AddModel(new DefaultSettlementProsperityModel());
            //gameStarter.AddModel(new DefaultSettlementGarrisonModel());
            //gameStarter.AddModel(new DefaultSettlementTaxModel());
            //gameStarter.AddModel(new DefaultBarterModel());
            //gameStarter.AddModel(new DefaultPersuasionModel());
            //gameStarter.AddModel(new DefaultClanTierModel());
            //gameStarter.AddModel(new DefaultClanPoliticsModel());
            //gameStarter.AddModel(new DefaultClanFinanceModel());
            gameStarter.AddModel(new BMClanFinanceModel());
            //gameStarter.AddModel(new DefaultTroopCountLimitModel());
            //gameStarter.AddModel(new DefaultHeirSelectionCalculationModel());
            //gameStarter.AddModel(new DefaultHeroDeathProbabilityCalculationModel());
            //gameStarter.AddModel(new DefaultBuildingConstructionModel());
            //gameStarter.AddModel(new DefaultBuildingEffectModel());
            //gameStarter.AddModel(new DefaultWallHitPointCalculationModel());
            //gameStarter.AddModel(new DefaultMarriageModel());
            //gameStarter.AddModel(new DefaultAgeModel());
            //gameStarter.AddModel(new DefaultDailyTroopXpBonusModel());
            //gameStarter.AddModel(new DefaultPregnancyModel());
            //gameStarter.AddModel(new DefaultNotablePowerModel());
            //gameStarter.AddModel(new DefaultTournamentModel());
            //gameStarter.AddModel(new DefaultSiegeStrategyActionModel());
            //gameStarter.AddModel(new DefaultSiegeEventModel());
            //gameStarter.AddModel(new DefaultCompanionHiringPriceCalculationModel());
            //gameStarter.AddModel(new DefaultBuildingScoreCalculationModel());
            //gameStarter.AddModel(new DefaultIssueDifficultyMultiplierCalculationModel());
            //gameStarter.AddModel(new DefaultPrisonerRecruitmentCalculationModel());
            //gameStarter.AddModel(new DefaultWorkshopModel());
            //gameStarter.AddModel(new DefaultDifficultyModel());
        }

    }

    

 

    internal class BMAgentBehaviorManager : IAgentBehaviorManager
    {
        public BMAgentBehaviorManager()
        {
        }

        public void AddFirstCompanionBehavior(IAgent agent)
        {
            BehaviorSets.AddFirstCompanionBehavior(agent);
        }

        public void AddQuestCharacterBehaviors(IAgent agent)
        {
            BehaviorSets.AddQuestCharacterBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddAmbushPlayerBehaviors(IAgent agent)
        {
            BehaviorSets.AddAmbushPlayerBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddBodyguardBehaviors(IAgent agent)
        {
            BehaviorSets.AddBodyguardBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddCompanionBehaviors(IAgent agent)
        {
            BehaviorSets.AddCompanionBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddFixedCharacterBehaviors(IAgent agent)
        {
            BehaviorSets.AddFixedCharacterBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddIndoorWandererBehaviors(IAgent agent)
        {
            BehaviorSets.AddIndoorWandererBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddOutdoorWandererBehaviors(IAgent agent)
        {
            BehaviorSets.AddOutdoorWandererBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddPatrollingGuardBehaviors(IAgent agent)
        {
            BehaviorSets.AddPatrollingGuardBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddStandGuardBehaviors(IAgent agent)
        {
            BehaviorSets.AddStandGuardBehaviors(agent);
        }

        void TaleWorlds.CampaignSystem.SandBox.IAgentBehaviorManager.AddWandererBehaviors(IAgent agent)
        {
            BehaviorSets.AddWandererBehaviors(agent);
        }
    }
}
