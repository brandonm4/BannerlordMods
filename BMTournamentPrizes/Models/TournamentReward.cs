using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using TournamentLib.Models;

namespace BMTournamentPrizes.Models
{
    public class TournamentReward 
    {
        public int BonusGold { get; set; } 
        public float BonusRenown { get; set; } 
        public float BonusInfluence { get; set; }
        public TournamentGame TournamentGame { get; set; }
        public int LastPayoutIndex { get; set; } = -1;
        public bool PrizeGiven { get; set; } = false;

        public TournamentReward(TournamentGame tournamentGame)
        {
            BonusRenown = TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinRenown;
                
            BonusInfluence = TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinInfluence +1f;
            BonusGold = 0;
        }
    }

   
}
