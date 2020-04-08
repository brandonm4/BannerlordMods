using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TournamentLib.Models;

namespace BMTournamentPrizes.Models
{
    public class TournamentPrizePool
    {
        public List<ItemObject> Items { get; set; } = new List<ItemObject>();
        public string Prize_StringId { get; set; } = "";
        public int RemainingRerolls { get; set; } = TournamentConfiguration.Instance.PrizeConfiguration.MaxNumberOfRerollsPerTournament;
    }
}
