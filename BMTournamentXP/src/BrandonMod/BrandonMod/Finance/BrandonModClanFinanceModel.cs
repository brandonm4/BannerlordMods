using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;

namespace BrandonMod
{
    public class BMClanFinanceModel : DefaultClanFinanceModel
    {
        private static TextObject _mercnaryStr;

        public BMClanFinanceModel() : base()
        {
            BMClanFinanceModel._mercnaryStr = new TextObject("{=qcaaJLhx}Mercenary Contract", null);
        }


        

        public override void CalculateClanIncome(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals = false)
        {
            //float single = 0f;
            //foreach (Town fortification in clan.Fortifications)
            //{
            //    TextObject textObject = new TextObject("{=YnvU7tWg} {PARTY_NAME} taxes", null);
            //    textObject.SetTextVariable("PARTY_NAME", fortification.Name);
            //    float single1 = (float)Campaign.Current.Models.SettlementTaxModel.CalculateTownTax(fortification, null);
            //    goldChange.Add(single1, textObject);
            //    single += single1;
            //}
            //if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan && clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.WarTax))
            //{
            //    goldChange.Add(single, DefaultPolicies.WarTax.Name);
            //}
            //if (clan.IsUnderMercenaryService && clan.Leader != null && clan.Leader.MapFaction.IsKingdomFaction && clan.Leader.MapFaction.Leader != null)
            //{
            //    int influence = (int)(clan.Influence * 0.1f);
            //    int mercenaryAwardMultiplier = influence * clan.MercenaryAwardMultiplier;
            //    goldChange.Add((float)mercenaryAwardMultiplier, BMClanFinanceModel._mercnaryStr);
            //    if (applyWithdrawals)
            //    {
            //        Clan influence1 = clan;
            //        influence1.Influence = influence1.Influence - (float)influence;
            //    }
            //}
            //foreach (Village village in clan.Villages)
            //{
            //    int num = (village.VillageState == Village.VillageStates.Looted || village.VillageState == Village.VillageStates.BeingRaided ? 0 : (int)((float)village.TradeTaxAccumulated / this.RevenueSmoothenFraction()));
            //    if (clan.Kingdom != null && clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.LandTax))
            //    {
            //        goldChange.Add((float)(-num) * 0.05f, DefaultPolicies.LandTax.Name);
            //    }
            //    TextObject textObject1 = new TextObject("{=YnvU7tWg} {PARTY_NAME} taxes", null);
            //    textObject1.SetTextVariable("PARTY_NAME", village.Name);
            //    goldChange.Add((float)num, textObject1);
            //    if (!applyWithdrawals)
            //    {
            //        continue;
            //    }
            //    Village tradeTaxAccumulated = village;
            //    tradeTaxAccumulated.TradeTaxAccumulated = tradeTaxAccumulated.TradeTaxAccumulated - num;
            //}
            //foreach (Town town in clan.Fortifications)
            //{
            //    TextObject textObject2 = new TextObject("{=oICap7jL} {SETTLEMENT_NAME} tariff", null);
            //    textObject2.SetTextVariable("SETTLEMENT_NAME", town.Name);
            //    int tradeTaxAccumulated1 = (int)((float)town.TradeTaxAccumulated / this.RevenueSmoothenFraction());
            //    if (town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Trade.ContentTrades))
            //    {
            //        ExplainedNumber explainedNumber = new ExplainedNumber((float)tradeTaxAccumulated1, null);
            //        PerkHelper.AddPerkBonusForTown(DefaultPerks.Trade.ContentTrades, town, ref explainedNumber);
            //        tradeTaxAccumulated1 = TaleWorlds.Library.MathF.Round(explainedNumber.ResultNumber);
            //    }
            //    goldChange.Add((float)tradeTaxAccumulated1, textObject2);
            //    if (!applyWithdrawals)
            //    {
            //        continue;
            //    }
            //    Town town1 = town;
            //    town1.TradeTaxAccumulated = town1.TradeTaxAccumulated - tradeTaxAccumulated1;
            //}
            this.CalculateHeroIncomeFromWorkshops(clan.Leader, ref goldChange, applyWithdrawals);
            //foreach (MobileParty party in clan.Parties)
            //{
            //    if (!party.IsActive || !party.IsLordParty && !party.IsGarrison && !party.IsCaravan)
            //    {
            //        continue;
            //    }
            //    int partyTradeGold = (party.LeaderHero == null || party.LeaderHero == clan.Leader || party.IsCaravan || party.LeaderHero.Gold < 10000 ? 0 : (int)((float)(party.LeaderHero.Gold - 10000) / 10f));
            //    if (party.IsCaravan)
            //    {
            //        partyTradeGold = (int)((float)(party.PartyTradeGold - 10000) / 10f);
            //    }
            //    if (partyTradeGold <= 0)
            //    {
            //        continue;
            //    }
            //    TextObject textObject3 = new TextObject("{=uyvxafSw} {PARTY_NAME} exceed gold", null);
            //    textObject3.SetTextVariable("PARTY_NAME", party.Name);
            //    goldChange.Add((float)partyTradeGold, textObject3);
            //    if (!applyWithdrawals)
            //    {
            //        continue;
            //    }
            //    if (!party.IsCaravan)
            //    {
            //        GiveGoldAction.ApplyBetweenCharacters(party.LeaderHero, null, partyTradeGold, true);
            //    }
            //    else
            //    {
            //        if (party.LeaderHero.Clan.Leader.GetPerkValue(DefaultPerks.Trade.GreatInvestor) & applyWithdrawals)
            //        {
            //            party.LeaderHero.Clan.AddRenown(DefaultPerks.Trade.GreatInvestor.PrimaryBonus, true);
            //        }
            //        party.PartyTradeGold -= partyTradeGold;
            //    }
            //}
        }

        public override int CalculateNotableDailyGoldChange(Hero hero, bool applyWithdrawals)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(0f, null);
            this.BMCalculateHeroIncomeFromAssets(hero, ref explainedNumber, applyWithdrawals);
            return (int)explainedNumber.ResultNumber;
        }

        private void BMCalculateHeroIncomeFromAssets(Hero hero, ref ExplainedNumber goldChange, bool applyWithdrawals)
        {
            //int num = 0;
            //foreach (MobileParty ownedCaravan in hero.OwnedCaravans)
            //{
            //    if (!ownedCaravan.IsCaravan || ownedCaravan.PartyTradeGold <= 10000)
            //    {
            //        continue;
            //    }
            //    int num1 = Campaign.Current.Models.ClanFinanceModel.CalculateOwnerIncomeFromCaravan(ownedCaravan);
            //    num = num1;
            //    if (applyWithdrawals)
            //    {
            //        ownedCaravan.PartyTradeGold -= num1;
            //        SkillLevelingManager.OnTradeProfitMade(hero, num1);
            //    }
            //    if (num1 <= 0)
            //    {
            //        continue;
            //    }
            //    TextObject textObject = new TextObject("{=DGONdmFg} {PARTY_NAME} profit", null);
            //    textObject.SetTextVariable("PARTY_NAME", ownedCaravan.Name);
            //    goldChange.Add((float)num, textObject);
            //}
            this.CalculateHeroIncomeFromWorkshops(hero, ref goldChange, applyWithdrawals);
            //if (hero.CurrentSettlement != null)
            //{
            //    foreach (CommonArea commonArea in hero.CurrentSettlement.CommonAreas)
            //    {
            //        if (commonArea.Owner != hero)
            //        {
            //            continue;
            //        }
            //        goldChange.Add(100f, commonArea.Name);
            //    }
            //}
        }

        private void CalculateHeroIncomeFromWorkshops(Hero hero, ref ExplainedNumber goldChange, bool applyWithdrawals)
        {
            int num = 0;
            int num1 = 0;
            foreach (Workshop ownedWorkshop in hero.OwnedWorkshops)
            {
                int num2 = Campaign.Current.Models.ClanFinanceModel.CalculateOwnerIncomeFromWorkshop(ownedWorkshop);
                if (num2 > 500)
                {
                    num2 = 500;
                }
                num = num2;
                if (applyWithdrawals && num2 > 0)
                {
                    ownedWorkshop.ChangeGold(-num2);
                }
                if (num2 <= 0)
                {
                    continue;
                }
                TextObject textObject = new TextObject("{=Vg7glbwp} {WORKSHOP_NAME} at {SETTLEMENT_NAME}", null);
                textObject.SetTextVariable("SETTLEMENT_NAME", ownedWorkshop.Settlement.Name);
                textObject.SetTextVariable("WORKSHOP_NAME", ownedWorkshop.Name);
                goldChange.Add((float)num, textObject);
                if (!(hero.Clan.Leader.GetPerkValue(DefaultPerks.Trade.ArtisanCommunity) & applyWithdrawals))
                {
                    continue;
                }
                num1++;
            }
            if (hero.Clan.Leader.GetPerkValue(DefaultPerks.Trade.ArtisanCommunity) & applyWithdrawals)
            {
                hero.Clan.AddRenown((float)num1 * DefaultPerks.Trade.ArtisanCommunity.PrimaryBonus, true);
            }
        }

    }
}
