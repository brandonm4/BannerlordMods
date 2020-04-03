using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BMTournamentXP
{
    internal class DefaultSettlementGarrisonModel : SettlementGarrisonModel
    {
        private readonly static TextObject _townWallsText;

        private readonly static TextObject _moraleText;

        private readonly static TextObject _foodShortageText;

        private readonly static TextObject _surplusFoodText;

        private readonly static TextObject _recruitFromCenterNotablesText;

        private readonly static TextObject _recruitFromVillageNotablesText;

        private readonly static TextObject _villageBeingRaided;

        private readonly static TextObject _villageLooted;

        private readonly static TextObject _townIsUnderSiege;

        private readonly static TextObject _retiredText;

        private readonly static TextObject _paymentIsLess;

        private readonly static TextObject _issues;

        static DefaultSettlementGarrisonModel()
        {
            DefaultSettlementGarrisonModel._townWallsText = new TextObject("{=SlmhqqH8}Town Walls", null);
            DefaultSettlementGarrisonModel._moraleText = new TextObject("{=UjL7jVYF}Morale", null);
            DefaultSettlementGarrisonModel._foodShortageText = new TextObject("{=qTFKvGSg}Food Shortage", null);
            DefaultSettlementGarrisonModel._surplusFoodText = GameTexts.FindText("str_surplus_food", null);
            DefaultSettlementGarrisonModel._recruitFromCenterNotablesText = GameTexts.FindText("str_center_notables", null);
            DefaultSettlementGarrisonModel._recruitFromVillageNotablesText = GameTexts.FindText("str_village_notables", null);
            DefaultSettlementGarrisonModel._villageBeingRaided = GameTexts.FindText("str_village_being_raided", null);
            DefaultSettlementGarrisonModel._villageLooted = GameTexts.FindText("str_village_looted", null);
            DefaultSettlementGarrisonModel._townIsUnderSiege = GameTexts.FindText("str_villages_under_siege", null);
            DefaultSettlementGarrisonModel._retiredText = GameTexts.FindText("str_retired", null);
            DefaultSettlementGarrisonModel._paymentIsLess = GameTexts.FindText("str_payment_is_less", null);
            DefaultSettlementGarrisonModel._issues = new TextObject("{=D7KllIPI}Issues", null);
        }

        public DefaultSettlementGarrisonModel()
        {
        }

        public override int CalculateGarrisonChange(Settlement settlement, StatExplainer explanation = null)
        {
            return DefaultSettlementGarrisonModel.CalculateGarrisonChangeInternal(settlement, explanation);
        }

        private static int CalculateGarrisonChangeInternal(Settlement settlement, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
            if (settlement.IsTown || settlement.IsCastle)
            {
                float loyalty = settlement.Town.Loyalty;
                if (settlement.IsStarving)
                {
                    float foodChange = settlement.Town.FoodChange;
                    explainedNumber.Add((float)((!settlement.Town.Owner.IsStarving || foodChange >= 0f ? 0 : (int)(foodChange / 4f))), DefaultSettlementGarrisonModel._foodShortageText);
                }
                if (settlement.Town.GarrisonParty != null && ((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber) / (float)settlement.Town.GarrisonParty.Party.PartySizeLimit > settlement.Town.GarrisonParty.PaymentRatio)
                {
                    int num = 0;
                    do
                    {
                        num++;
                    }
                    while (((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber - (float)num) / (float)settlement.Town.GarrisonParty.Party.PartySizeLimit >= settlement.Town.GarrisonParty.PaymentRatio && (float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber - (float)num > 0f && num < 20);
                    explainedNumber.Add((float)(-num), DefaultSettlementGarrisonModel._paymentIsLess);
                }
                if (settlement.SiegeEvent != null)
                {
                    int num1 = 0;
                    foreach (Village boundVillage in settlement.BoundVillages)
                    {
                        if (boundVillage.VillageState != Village.VillageStates.Normal)
                        {
                            continue;
                        }
                        int num2 = 0;
                        foreach (Hero notable in boundVillage.Settlement.Notables)
                        {
                            if (notable.SupporterOf != settlement.OwnerClan)
                            {
                                continue;
                            }
                            num2++;
                        }
                        if (num2 > 0)
                        {
                            explainedNumber.Add((float)num2, boundVillage.Name);
                        }
                        num1 += num2;
                    }
                    if (num1 > 0)
                    {
                        explainedNumber.Add((float)num1, DefaultSettlementGarrisonModel._recruitFromVillageNotablesText);
                    }
                }
            }
            DefaultSettlementGarrisonModel.GetSettlementGarrisonChangeDueToIssues(settlement, ref explainedNumber);
            return (int)explainedNumber.ResultNumber;
        }

        public override int FindNumberOfTroopsToLeaveToGarrison(MobileParty mobileParty, Settlement settlement)
        {
            MobileParty garrisonParty = settlement.Town.GarrisonParty;
            float totalStrength = 0f;
            if (garrisonParty != null)
            {
                totalStrength = garrisonParty.Party.TotalStrength;
            }
            float single = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan);
            single = single * (settlement.IsTown ? 2f : 1f);
            if (settlement.OwnerClan.Leader == Hero.MainHero && (mobileParty.LeaderHero == null || mobileParty.LeaderHero.Clan != Clan.PlayerClan) || totalStrength >= single)
            {
                return 0;
            }
            int numberOfRegularMembers = mobileParty.Party.NumberOfRegularMembers;
            float numberOfWoundedRegularMembers = 1f + (float)mobileParty.Party.NumberOfWoundedRegularMembers / (float)mobileParty.Party.NumberOfRegularMembers;
            float partySizeLimit = (float)mobileParty.Party.PartySizeLimit * mobileParty.PaymentRatio;
            float single1 = (float)(Math.Pow((double)Math.Min(2f, (float)numberOfRegularMembers / partySizeLimit), 1.20000004768372) * 0.75);
            float single2 = (1f - totalStrength / single) * (1f - totalStrength / single);
            if (mobileParty.Army != null)
            {
                single2 = Math.Min(single2, 0.5f);
            }
            float single3 = 0.5f;
            if (settlement.OwnerClan == mobileParty.Leader.HeroObject.Clan || settlement.OwnerClan == mobileParty.Party.Owner.MapFaction.Leader.Clan)
            {
                single3 = 1f;
            }
            float single4 = (mobileParty.Army != null ? 1.25f : 1f);
            float single5 = 1f;
            List<float> singles = new List<float>(5);
            for (int i = 0; i < 5; i++)
            {
                singles.Add(Campaign.MapDiagonal * Campaign.MapDiagonal);
            }
            foreach (Kingdom all in Kingdom.All)
            {
                if (!all.IsKingdomFaction || !mobileParty.MapFaction.IsAtWarWith(all))
                {
                    continue;
                }
            Label0:
                foreach (Settlement settlement1 in all.Settlements)
                {
                    float single6 = settlement1.Position2D.DistanceSquared(mobileParty.Position2D);
                    int num = 0;
                    while (num < 5)
                    {
                        if (single6 >= singles[num])
                        {
                            num++;
                        }
                        else
                        {
                            for (int j = 4; j >= num + 1; j--)
                            {
                                singles[j] = singles[j - 1];
                            }
                            singles[num] = single6;
                            goto Label0;
                        }
                    }
                }
            }
            float single7 = 0f;
            for (int k = 0; k < 5; k++)
            {
                single7 += (float)Math.Sqrt((double)singles[k]);
            }
            single7 /= 5f;
            float single8 = Math.Max(0f, Math.Min(Campaign.MapDiagonal / 15f - Campaign.MapDiagonal / 30f, single7 - Campaign.MapDiagonal / 30f));
            float mapDiagonal = Campaign.MapDiagonal / 15f - Campaign.MapDiagonal / 30f;
            float single9 = single8 / mapDiagonal;
            float single10 = Math.Min(0.7f, single5 * single1 * single2 * single3 * single4 * numberOfWoundedRegularMembers);
            return MBRandom.RoundRandomized((float)numberOfRegularMembers * single10);
        }

        public override int FindNumberOfTroopsToTakeFromGarrison(MobileParty mobileParty, Settlement settlement)
        {
            MobileParty garrisonParty = settlement.Town.GarrisonParty;
            float totalStrength = 0f;
            if (garrisonParty == null)
            {
                return 0;
            }
            totalStrength = garrisonParty.Party.TotalStrength;
            float single = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan);
            single = single * (settlement.IsTown ? 2f : 1f);
            float partySizeLimit = (float)mobileParty.Party.PartySizeLimit * mobileParty.PaymentRatio;
            int numberOfAllMembers = mobileParty.Party.NumberOfAllMembers;
            float single1 = Math.Min(12f, partySizeLimit / (float)numberOfAllMembers) - 1f;
            float single2 = (float)Math.Pow((double)(totalStrength / single), 1.5);
            int totalRegulars = MBRandom.RoundRandomized(single1 * single2 * (mobileParty.LeaderHero.Clan.Leader == mobileParty.LeaderHero ? 2f : 1f));
            int num = 20;
            num = num * (settlement.IsTown ? 2 : 1);
            if (totalRegulars > garrisonParty.Party.MemberRoster.TotalRegulars - num)
            {
                totalRegulars = garrisonParty.Party.MemberRoster.TotalRegulars - num;
            }
            return totalRegulars;
        }

        private static void GetSettlementGarrisonChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
        {
            float single;
            if (IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementGarrison, settlement, out single))
            {
                result.Add(single, DefaultSettlementGarrisonModel._issues);
            }
        }
    }

    internal class DefaultHeirSelectionCalculationModel : HeirSelectionCalculationModel
    {
        private const int _maleHeirPoint = 10;

        private const int _eldestPoint = 5;

        private const int _youngestPoint = -5;

        private const int _directDescendentPoint = 10;

        private const int _collateralHeirPonit = 10;

        public const int HighestSkillPoint = 5;

        public DefaultHeirSelectionCalculationModel()
        {
        }

        public override int CalculateHeirSelectionPoint(Hero candidateHeir, Hero deadHero, ref Hero maxSkillHero, StatExplainer explanation = null)
        {
            return this.CalculateHeirSelectionPointInternal(candidateHeir, deadHero, ref maxSkillHero, explanation);
        }

        private int CalculateHeirSelectionPointInternal(Hero candidateHeir, Hero deadHero, ref Hero maxSkillHero, StatExplainer explanation = null)
        {
            float? nullable;
            float? nullable1;
            float? nullable2;
            IEnumerable<Hero> children;
            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
            if (!candidateHeir.IsFemale)
            {
                explainedNumber.Add(10f, null);
            }
            IOrderedEnumerable<Hero> heroes =
                from x in candidateHeir.Clan.Heroes
                where x != deadHero
                select x into h
                orderby h.Age
                select h;
            Hero hero = heroes.LastOrDefault<Hero>();
            if (hero != null)
            {
                nullable1 = new float?(hero.Age);
            }
            else
            {
                nullable = null;
                nullable1 = nullable;
            }
            float? nullable3 = nullable1;
            Hero hero1 = heroes.FirstOrDefault<Hero>();
            if (hero1 != null)
            {
                nullable2 = new float?(hero1.Age);
            }
            else
            {
                nullable = null;
                nullable2 = nullable;
            }
            float? nullable4 = nullable2;
            nullable = nullable3;
            if (!(candidateHeir.Age == nullable.GetValueOrDefault() & nullable.HasValue))
            {
                nullable = nullable4;
                if (candidateHeir.Age == nullable.GetValueOrDefault() & nullable.HasValue)
                {
                    explainedNumber.Add(-5f, null);
                }
            }
            else
            {
                explainedNumber.Add(5f, null);
            }
            if (deadHero.Father == candidateHeir || deadHero.Mother == candidateHeir || candidateHeir.Father == deadHero || candidateHeir.Mother == deadHero || candidateHeir.Father == deadHero.Father || candidateHeir.Mother == deadHero.Mother)
            {
                explainedNumber.Add(10f, null);
            }
            Hero father = deadHero.Father;
            while (father != null && father.Father != null)
            {
                father = father.Father;
            }
            if (father != null && father.Children != null)
            {
                if (father != null)
                {
                    children = father.Children;
                }
                else
                {
                    children = null;
                }
                if (DefaultHeirSelectionCalculationModel.DoesHaveSameBloodLine(children, candidateHeir))
                {
                    explainedNumber.Add(10f, null);
                }
            }
            int skillValue = 0;
            foreach (SkillObject allSkill in DefaultSkills.GetAllSkills())
            {
                skillValue += candidateHeir.GetSkillValue(allSkill);
            }
            int num = 0;
            foreach (SkillObject skillObject in DefaultSkills.GetAllSkills())
            {
                num += maxSkillHero.GetSkillValue(skillObject);
            }
            if (skillValue > num)
            {
                maxSkillHero = candidateHeir;
            }
            return (int)explainedNumber.ResultNumber;
        }

        private static bool DoesHaveSameBloodLine(IEnumerable<Hero> childeren, Hero candidateHeir)
        {
            bool flag;
            if (!childeren.Any<Hero>())
            {
                return false;
            }
            using (IEnumerator<Hero> enumerator = childeren.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    Hero current = enumerator.Current;
                    flag = (current != candidateHeir ? DefaultHeirSelectionCalculationModel.DoesHaveSameBloodLine(current.Children, candidateHeir) : true);
                }
                else
                {
                    return false;
                }
            }
            return flag;
        }
    }

    internal class DefaultSettlementTaxModel : SettlementTaxModel
    {
        public const float SettlementCommissionRateTown = 0.05f;

        public const float SettlementCommissionRateVillage = 0.5f;

        private readonly TextObject _prosperityText = GameTexts.FindText("str_prosperity", null);

        private readonly TextObject _issuesText = GameTexts.FindText("str_issues", null);

        public DefaultSettlementTaxModel()
        {
        }

        private float CalculateDailyTax(Town town, ref ExplainedNumber explainedNumber)
        {
            float prosperity = town.Prosperity;
            float single = 1f;
            if (town.Settlement.OwnerClan.Kingdom != null && town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.LandTax))
            {
                single += 0.05f;
            }
            int num = (int)(prosperity * 0.2f);
            explainedNumber.Add((float)num, this._prosperityText);
            return explainedNumber.ResultNumber;
        }

        private int CalculateDailyTaxInternal(Town town, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
            float single = this.CalculateDailyTax(town, ref explainedNumber);
            this.CalculatePolicyGoldCut(town, single, ref explainedNumber);
            if (town.Security <= (float)Campaign.Current.Models.SettlementSecurityModel.LowSecurityThresholdForTaxCorruption)
            {
                Campaign.Current.Models.SettlementSecurityModel.CalculateSecurityGoldCutDueToCorruption(town, single, ref explainedNumber);
            }
            if (town.IsTown)
            {
                foreach (Building building in town.Buildings)
                {
                    int buildingEffectAmount = building.GetBuildingEffectAmount(DefaultBuildingEffects.Tax);
                    explainedNumber.AddFactor((float)buildingEffectAmount / 100f, building.Name);
                }
                if (town.Settlement.OwnerClan.Leader.GetPerkValue(DefaultPerks.Trade.RapidDevelopment) && town.Settlement.OwnerClan.Leader.CurrentSettlement != null && town.Settlement.OwnerClan.Leader.CurrentSettlement == town.Settlement)
                {
                    explainedNumber.AddFactor(DefaultPerks.Trade.RapidDevelopment.SecondaryBonus / 100f, DefaultPerks.Trade.RapidDevelopment.Name);
                }
            }
            this.GetSettlementTaxChangeDueToIssues(town, ref explainedNumber);
            return (int)explainedNumber.ResultNumber;
        }

        private void CalculatePolicyGoldCut(Town town, float rawTax, ref ExplainedNumber explainedNumber)
        {
            if (town.MapFaction.IsKingdomFaction)
            {
                Kingdom mapFaction = (Kingdom)town.MapFaction;
                if (mapFaction.ActivePolicies.Contains(DefaultPolicies.Magistrates))
                {
                    explainedNumber.Add(-0.05f * rawTax, DefaultPolicies.Magistrates.Name);
                }
                if (mapFaction.ActivePolicies.Contains(DefaultPolicies.Cantons))
                {
                    explainedNumber.Add(-0.1f * rawTax, DefaultPolicies.Cantons.Name);
                }
                if (mapFaction.ActivePolicies.Contains(DefaultPolicies.Bailiffs))
                {
                    explainedNumber.Add(-0.5f, DefaultPolicies.Bailiffs.Name);
                }
                if (mapFaction.ActivePolicies.Contains(DefaultPolicies.TribunesOfThePeople))
                {
                    explainedNumber.Add(-0.05f * rawTax, DefaultPolicies.TribunesOfThePeople.Name);
                }
            }
        }

        public override int CalculateTownTax(Town town, StatExplainer explanation = null)
        {
            return this.CalculateDailyTaxInternal(town, explanation);
        }

        public override int CalculateVillageTaxFromIncome(Village village, int marketIncome)
        {
            return (int)((float)marketIncome * this.GetVillageTaxRatio());
        }

        private void GetSettlementTaxChangeDueToIssues(Town center, ref ExplainedNumber result)
        {
            float single;
            if (IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementTax, center.Owner.Settlement, out single))
            {
                result.Add(single, this._issuesText);
            }
        }

        public override float GetTownTaxRatio(Town town)
        {
            float single = 1f;
            if (town.Settlement.OwnerClan.Kingdom != null && town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CrownDuty))
            {
                single += 0.05f;
            }
            return 0.05f * single;
        }

        public override float GetVillageTaxRatio()
        {
            return 0.5f;
        }
    }
}
