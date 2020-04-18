using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;

namespace BMTweakCollection.Behaviors
{
    public class DynamicBanditSizeCampaignBehavior : CampaignBehaviorBase
    {
        public void DailyTick()
        {
            List<Clan> clanList = Clan.BanditFactions.ToList<Clan>();

            foreach (var clan in clanList)
            {
                foreach (var banditParty in clan.Parties)
                {
                    //  banditParty.MemberRoster.Troops
                }
            }
        }

        public void AdjustBanditPartySize(MobileParty bandparty)
        {
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}