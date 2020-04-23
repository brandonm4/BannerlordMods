using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace BMTweakCollection.Behaviors
{
    public class SmithingStanimaRecovery : CampaignBehaviorBase
    {
    
        public void HourlyTick()
        {
            bool currentSettlement;
            //foreach (KeyValuePair<Hero, CraftingCampaignBehavior.HeroCraftingRecord> _heroCraftingRecord in this._heroCraftingRecords)
            //{
            //    if (_heroCraftingRecord.Value.CraftingStamina >= 100)
            //    {
            //        continue;
            //    }
            //    MobileParty partyBelongedTo = _heroCraftingRecord.Key.PartyBelongedTo;
            //    if (partyBelongedTo != null)
            //    {
            //        currentSettlement = partyBelongedTo.CurrentSettlement;
            //    }
            //    else
            //    {
            //        currentSettlement = false;
            //    }
            //    if (!currentSettlement)
            //    {
            //        continue;
            //    }
            //    _heroCraftingRecord.Value.CraftingStamina = Math.Min(100, _heroCraftingRecord.Value.CraftingStamina + 5);
            //}
        }

        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}