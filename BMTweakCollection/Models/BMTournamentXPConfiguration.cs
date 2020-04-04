using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMTweakCollection.Models
{
    public class BMTournamentXPConfiguration
    {
        public static string Version { get; set; } = "e1.1.1";
        public bool IsTournamentXPEnabled { get; set; } = true;
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        public bool TournamentPrizeRerollEnabled { get; set; } = true;
        public bool IsArenaXPEnabled { get; set; } = true;
        public float ArenaXPAdjustment { get; set; } = 1.0f;
    }
}
