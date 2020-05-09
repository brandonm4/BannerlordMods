using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace BMTweakCollection.Behaviors
{
    internal class FixedCompanionSkillsBehavior : CampaignBehaviorBase
    {
        public FixedCompanionSkillsBehavior()
        {
        }

        private void DailyTickParty(MobileParty mobileParty)
        {
            if (mobileParty.LeaderHero != null)
            {
                if (mobileParty.EffectiveScout != null)
                {
                    mobileParty.EffectiveScout.AddSkillXp(DefaultSkills.Roguery, (float)(mobileParty.Party.NumberOfPrisoners / 2));
                }
                if (mobileParty.EffectiveSurgeon != null)
                {
                    mobileParty.EffectiveSurgeon.AddSkillXp(DefaultSkills.Steward, (float)(mobileParty.Party.NumberOfPackAnimals / 5));
                }
                if (mobileParty.EffectiveQuartermaster != null)
                {
                    mobileParty.EffectiveQuartermaster.AddSkillXp(DefaultSkills.Charm, Math.Max(0f, mobileParty.Morale - 70f) / 2f);
                }
                if (mobileParty.IsCaravan)
                {
                    mobileParty.LeaderHero.AddSkillXp(DefaultSkills.Steward, (float)mobileParty.Party.NumberOfRegularMembers);
                }
            }
        }

        private void OnDailyTick()
        {
            foreach (Town all in Town.All)
            {
                if (all.Governor != null)
                {
                    all.Governor.AddSkillXp(DefaultSkills.Trade, (float)(all.TradeTaxAccumulated / 100));
                    all.Governor.AddSkillXp(DefaultSkills.Steward, Math.Max(all.ProsperityChange + (float)(all.Settlement.BoundVillages.Count * 5), 0f));
                    all.Governor.AddSkillXp(DefaultSkills.Leadership, (float)(all.GetNumberOfTroops() / 100));
                    all.Governor.AddSkillXp(DefaultSkills.Charm, all.Loyalty / 5f);
                }
            }
        }

        private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
        {
            if (party != null)
            {
                if ((!party.IsCaravan || !settlement.IsTown ? false : party.LeaderHero != null))
                {
                    party.LeaderHero.AddSkillXp(DefaultSkills.Trade, 10f);
                    if (party.Party.NumberOfRegularMembers < party.Party.PartySizeLimit - 1)
                    {
                        if ((settlement.Town.MercenaryData.HasAvailableMercenary(Occupation.CaravanGuard) ? true : settlement.Town.MercenaryData.HasAvailableMercenary(Occupation.Mercenary)))
                        {
                            CharacterObject troopType = settlement.Town.MercenaryData.TroopType;
                            int num = 1;
                            if (party.Party.NumberOfRegularMembers < party.Party.PartySizeLimit / 2)
                            {
                                num = 5;
                            }
                            RecruitAction.ApplyRecruitMercenary(party, settlement, troopType, num);
                            party.LeaderHero.AddSkillXp(DefaultSkills.Leadership, (float)(5 * num));
                            MobileParty partyTradeGold = party;
                            partyTradeGold.PartyTradeGold = partyTradeGold.PartyTradeGold - 50 * num;
                        }
                    }
                }
            }
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void WeeklyTickEvent()
        {
        }
    }
}