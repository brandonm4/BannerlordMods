using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Localization;

namespace BMTournamentPrizes.Models
{
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
}
