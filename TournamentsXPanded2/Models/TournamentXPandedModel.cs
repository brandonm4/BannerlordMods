using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
#if VERSION130
using TaleWorlds.ObjectSystem;
#endif
using TaleWorlds.SaveSystem;

namespace TournamentsXPanded.Models
{
    public class TournamentXPandedModel
    {
        public List<PrizeItem> PrizePool { get; set; }
        public string SelectedPrizeStringId { get; set; }
        public int ReRollsUsed { get; set; }
        public string SettlementStringId { get; set; }
        public string TournamentTypeId { get; set; }
        public bool Active { get; set; }
        
        [JsonIgnore]
        public TournamentReward Rewards { get; set; }

        [JsonIgnore]
        public PrizeItem SelectedPrizeItem
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(SelectedPrizeStringId))
                {
                    return PrizePool.Where(x => x.ItemStringId == SelectedPrizeStringId).FirstOrDefault();
                }
                return null;
            }
        }
        
        [JsonIgnore]
        public Settlement Settlement
        {
            get
            {
                return Campaign.Current.Settlements.Where(x => x.StringId == SettlementStringId).FirstOrDefault();
            }
        }

        public TournamentXPandedModel()
        {
            PrizePool = new List<PrizeItem>();
            SelectedPrizeStringId = "";
            ReRollsUsed = 0;
            TournamentTypeId = "melee";
            SettlementStringId = "";
            Rewards = new TournamentReward();
            Active = false;
        }             
    }

  
}
