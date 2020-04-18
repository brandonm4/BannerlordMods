using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TournamentsXPanded.Models
{
    [SaveableClass(TournamentsXPandedSubModule.OBJ_TOURNAMENT_TYPE_MELEE2)]
    public class Fight2TournamentGame : FightTournamentGame
    {
        private const string descriptionMixed = "The tournament will be 1v1 and free for all melee group fights.";
        private const string description1v1 = "The tournament will be bracket of 1v1 rounds.";
        private const string descriptionFFA = "The tournament will be a set of group free for all bouts.";

        public string Description { get; set; } = descriptionMixed;

        public Fight2TournamentGame(Town town) : base(town)
        {
            base.Mode = TournamentGame.QualificationMode.IndividualScore;
        }

        public override TextObject GetMenuText()
        {
            return new TextObject(string.Concat("{=MWGATOoz}", Description));
        }

        public override int MaxTeamSize
        {
            get
            {
                return 1;
            }
        }

        private int _maxTeamNumberPerMatch = 4;

        public override int MaxTeamNumberPerMatch
        {
            get
            {
                return _maxTeamNumberPerMatch;
            }
        }

        public void SetFightMode(FightMode mode)
        {
            switch (mode)
            {
                case FightMode.Mixed:
                    _maxTeamNumberPerMatch = 4;
                    Description = descriptionMixed;
                    break;

                case FightMode.One_One:
                    _maxTeamNumberPerMatch = 2;
                    Description = description1v1;
                    break;
            }
        }

        public enum FightMode
        {
            Mixed,
            One_One
        }
    }

    public class Fight2TournamentGameSaveableTypeDefiner : SaveableTypeDefiner
    {
        public Fight2TournamentGameSaveableTypeDefiner() : base(TournamentsXPandedSubModule.SAVEDEF_TOURNAMENT_TYPE_MELEE2)
        {
        }

        protected override void DefineClassTypes()
        {
            base.AddClassDefinition(typeof(Fight2TournamentGame), 1);
        }

        protected override void DefineContainerDefinitions()
        {
            base.ConstructContainerDefinition(typeof(List<Fight2TournamentGame>));
            base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, Fight2TournamentGame>));
            base.ConstructContainerDefinition(typeof(Dictionary<string, Fight2TournamentGame>));
        }

        //protected override void DefineGenericClassDefinitions()
        //{
        //  base.ConstructGenericClassDefinition(typeof(Fight2TournamentGame));
        //}
    }
}