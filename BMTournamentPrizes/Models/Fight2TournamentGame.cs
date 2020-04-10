using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace BMTournamentPrizes.Models
{

    [SaveableClass(4106001)]
    public class Fight2TournamentGame : FightTournamentGame
    {
        public Fight2TournamentGame(Town town) : base(town)
        {
            base.Mode = TournamentGame.QualificationMode.IndividualScore;
        }
     
        public override TextObject GetMenuText()
        {
            return new TextObject("{=MWGATOoz}The tournament will be 1v1 and death match melee group fights.");
        }
        public override int MaxTeamSize
        {
            get
            {
                return 1;
            }
        }
    }

    public class Fight2TournamentGameSaveableTypeDefiner : SaveableTypeDefiner
    {
        public Fight2TournamentGameSaveableTypeDefiner() : base(4105001)
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
