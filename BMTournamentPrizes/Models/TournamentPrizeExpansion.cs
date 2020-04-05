using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace BMTournamentPrizes.Models
{
    public class TournamentPrizeExpansion
    {
        public Dictionary<string, List<TournamentPrizeEntry>> SettlementPrizes = new Dictionary<string, List<TournamentPrizeEntry>>();
        private static TournamentPrizeExpansion _instance;

        public static TournamentPrizeExpansion Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TournamentPrizeExpansion();                  
                }

                return _instance;
            }
        }

    }

    public class TournamentPrizeEntry
    {
        public ItemObject Item { get; set; }
        public bool IsSelected { get; set; }
    }
}
