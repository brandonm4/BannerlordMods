//using SandBox;
//using SandBox.Source.Missions;
//using SandBox.TournamentMissions.Missions;
//using System.Collections.Generic;

//using TaleWorlds.CampaignSystem;
//using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
//using TaleWorlds.Core;
//using TaleWorlds.Localization;
//using TaleWorlds.MountAndBlade;
//using TaleWorlds.MountAndBlade.Source.Missions;
//using TaleWorlds.SaveSystem;
//using TournamentsXPanded.Behaviors;

//namespace TournamentsXPanded.Models
//{
//    [SaveableClass(TournamentsXPandedSubModule.OBJ_TOURNAMENT_TYPE_MELEE3)]
//    public class BattleRoyalTournamentGame : FightTournamentGame
//    {
//        private const string descriptionMixed = "The tournament will be 1v1 and free for all melee group fights.";
//        private const string description1v1 = "The tournament will be bracket of 1v1 rounds.";
//        private const string descriptionFFA = "The tournament will be a series of free for all matches.";

//        public string Description { get; set; } = descriptionMixed;

//        public BattleRoyalTournamentGame(Town town) : base(town)
//        {
//            base.Mode = TournamentGame.QualificationMode.IndividualScore;
//        }

//        public override TextObject GetMenuText()
//        {
//            return new TextObject(string.Concat("{=MWGATOoz}", Description));
//        }

//        public override int MaxTeamSize
//        {
//            get
//            {
//                return 1;
//            }
//        }

//        private int _maxTeamNumberPerMatch = 32;

//        public override int MaxTeamNumberPerMatch
//        {
//            get
//            {
//                return _maxTeamNumberPerMatch;
//            }
//        }

//        public void SetFightMode(FightMode mode)
//        {
//            switch (mode)
//            {
//                case FightMode.Mixed:
//                    _maxTeamNumberPerMatch = 4;
//                    Description = descriptionMixed;
//                    break;

//                case FightMode.One_One:
//                    _maxTeamNumberPerMatch = 2;
//                    Description = description1v1;
//                    break;
//                case FightMode.BattleRoyal:
//                    _maxTeamNumberPerMatch = 32;
//                    Description = descriptionFFA;
//                    break;
//            }
//        }

//        public enum FightMode
//        {
//            Mixed,
//            One_One,
//            BattleRoyal,
//        }

//        public override void OpenMission(Settlement settlement, bool isPlayerParticipating)
//        {
//            int num = (settlement.IsTown ? settlement.GetComponent<Town>().GetWallLevel() : 1);
//            OpenBattleRoyalMission(LocationComplex.Current.GetScene("arena", num), this, settlement, settlement.Culture, isPlayerParticipating);         
//        }

//        public static IMission OpenBattleRoyalMission(string scene, TournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
//        {
//            return MissionState.OpenNew("TournamentFight", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false), (Mission missionController) => {
//                TournamentFightMissionController tournamentFightMissionController = new TournamentFightMissionController(culture);
//                return new MissionBehaviour[] { new CampaignMissionComponent(), tournamentFightMissionController, new BattleRoyalTournamentBehavior(tournamentGame, settlement, tournamentFightMissionController, isPlayerParticipating), new AgentVictoryLogic(), new AgentBattleAILogic(), new ArenaAgentStateDeciderLogic(), new MissionHardBorderPlacer(), new MissionBoundaryPlacer(), new MissionOptionsComponent(), new HighlightsController(), new SandboxHighlightsController() };
//            }, true, true, true);
//        }
//    }
    

//    public class BattleRoyalTournamentGameSaveableTypeDefiner : SaveableTypeDefiner
//    {
//        public BattleRoyalTournamentGameSaveableTypeDefiner() : base(TournamentsXPandedSubModule.SAVEDEF_TOURNAMENT_TYPE_MELEE3)
//        {
//        }

//        protected override void DefineClassTypes()
//        {
//            base.AddClassDefinition(typeof(BattleRoyalTournamentGame), 1);
//        }

//        protected override void DefineContainerDefinitions()
//        {
//            base.ConstructContainerDefinition(typeof(List<BattleRoyalTournamentGame>));
//            base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, BattleRoyalTournamentGame>));
//            base.ConstructContainerDefinition(typeof(Dictionary<string, BattleRoyalTournamentGame>));
//        }

//        //protected override void DefineGenericClassDefinitions()
//        //{
//        //  base.ConstructGenericClassDefinition(typeof(Fight2TournamentGame));
//        //}
//    }
//}