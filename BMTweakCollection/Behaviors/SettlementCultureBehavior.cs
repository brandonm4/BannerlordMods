using System;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace BMTweakCollection.Behaviors
{
    class SettlementVariablesBehaviorMod : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener((object)this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedMod));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener((object)this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.OnClanChangedKingdomMod));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener((object)this, new Action<CampaignGameStarter>(this.OnGameLoaded));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        public void OnSettlementOwnerChangedMod(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            if (!Campaign.Current.GameStarted)
                return;

            changeSettlementCulture(settlement);
        }

        public void OnClanChangedKingdomMod(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            if (clan.Settlements == null || !clan.Settlements.Any())
                return;

            foreach (var settlement in clan.Settlements.Where(x => x.IsVillage || x.IsCastle || x.IsTown))
                changeSettlementCulture(settlement);
        }

        public void OnGameLoaded(CampaignGameStarter starter)
        {
            if (!Campaign.Current.GameStarted || Campaign.Current.Settlements == null)
                return;

            foreach (Settlement settlement in Campaign.Current.Settlements.Where(x => x.IsVillage || x.IsCastle || x.IsTown))
                changeSettlementCulture(settlement);
        }

        void changeSettlementCulture(Settlement settlement)
        {
            if (!(settlement.IsTown || settlement.IsCastle || settlement.IsVillage))
                return;

            if (!(settlement.OwnerClan == Clan.PlayerClan))
                return;

            var ownerCulture = settlement.OwnerClan?.Kingdom?.Culture ?? settlement.OwnerClan.Culture;

            if (settlement.Culture != ownerCulture)
                settlement.Culture = ownerCulture;

            // Attempt to set attached villages
            if (settlement.BoundVillages != null)
            {
                foreach (Village attached in settlement.BoundVillages)
                {
                    if (attached.Settlement == null)
                        continue;

                    changeSettlementCulture(attached.Settlement);
                }
            }
        }
    }
}
