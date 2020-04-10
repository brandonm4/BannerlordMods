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
   // [SaveableClass(9006000)]
    public class TournamentPrizePool : MBObjectBase
    {
      //  [SaveableProperty(300)]
        public ItemRoster Prizes { get; set; } = new ItemRoster();
  //      [SaveableProperty(100)]
        public string SelectedPrizeStringId { get; set; } = "";
    //    [SaveableProperty(200)]
        public int RemainingRerolls { get; set; } = TournamentConfiguration.Instance.PrizeConfiguration.MaxNumberOfRerollsPerTournament;
     //   [SaveableProperty(20)]
        public Town Town
        {
            get;
            set;
        }

    }
}
