using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(DefaultPartyHealingModel), "GetDailyHealingHpForHeroes")]
    public class DefaultPartyHealingModelDailyHealingForHeros
    {
        public static bool GetDailyHealingHpForHeroes(ref float __result, MobileParty party, StatExplainer explanation, TextObject ____settlementText, TextObject ____starvingText, TextObject ____bushDoctorPerkText)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(2f, explanation, null);
            SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.HealingRateBonus, party, ref explainedNumber);
            if (party.CurrentSettlement != null || party.LastVisitedSettlement != null && party.LastVisitedSettlement.IsVillage && party.LastVisitedSettlement.Position2D.DistanceSquared(party.Position2D) < 1f)
            {
                
                explainedNumber.Add(5f, ____settlementText);
            }
            if (!party.IsGarrison && !party.IsMilitia)
            {
                if (party.PartyMoveMode != MoveModeType.Hold)
                {
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.WalkItOff, party, ref explainedNumber);
                }
                else
                {
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.TriageTent, party, ref explainedNumber);
                }
            }
            if (party.Morale >= Campaign.Current.Models.PartyMoraleModel.HighMoraleValue)
            {
                PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.BestMedicine, party, ref explainedNumber);
            }
            if (party.CurrentSettlement != null)
            {
                explainedNumber.Add(5f, ____settlementText);
                if (party.CurrentSettlement.IsTown && party.SiegeEvent == null && !party.CurrentSettlement.IsUnderSiege)
                {
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.GoodLodging, party, ref explainedNumber);
                }
            }
            else if (!party.IsMoving && party.LastVisitedSettlement != null && party.LastVisitedSettlement.IsVillage && party.LastVisitedSettlement.Position2D.DistanceSquared(party.Position2D) < 1f && !party.IsRaiding && !party.LastVisitedSettlement.IsUnderRaid && party.HasPerk(DefaultPerks.Medicine.BushDoctor))
            {
                explainedNumber.AddFactor(DefaultPerks.Medicine.BushDoctor.SecondaryBonus, ____bushDoctorPerkText);
            }
            if (party.Party.IsStarving && party.CurrentSettlement == null)
            {
                explainedNumber.Add(-5f,____starvingText);
            }
            __result = explainedNumber.ResultNumber;
            return false;
        }

        static bool Prepare()
        {
            return false;
        }
    }
}



/*
 *  public override float GetDailyHealingForRegulars(MobileParty party, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber;
            if (!party.Party.IsStarving)
            {
                int totalWoundedRegulars = party.MemberRoster.TotalWoundedRegulars;
                float single = 0.2f + (float)totalWoundedRegulars * 0.2f;
                explainedNumber = new ExplainedNumber(single, explanation, null);
                if (!party.IsGarrison)
                {
                    SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.HealingRateBonus, party, ref explainedNumber);
                }
                else if (party.CurrentSettlement.IsTown)
                {
                    SkillHelper.AddSkillBonusForTown(DefaultSkills.Medicine, DefaultSkillEffects.GovernorHealingRateBonus, party.CurrentSettlement.Town, ref explainedNumber);
                }
                if (!party.IsGarrison && !party.IsMilitia)
                {
                    if (party.IsMoving)
                    {
                        PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.WalkItOff, party, ref explainedNumber);
                    }
                    else
                    {
                        PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.TriageTent, party, ref explainedNumber);
                    }
                }
                if (party.Morale >= Campaign.Current.Models.PartyMoraleModel.HighMoraleValue)
                {
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.BestMedicine, party, ref explainedNumber);
                }
                if (party.CurrentSettlement != null)
                {
                    explainedNumber.Add(5f, DefaultPartyHealingModel._settlementText);
                    if (party.CurrentSettlement.IsTown && party.SiegeEvent == null && !party.CurrentSettlement.IsUnderSiege)
                    {
                        PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.GoodLodging, party, ref explainedNumber);
                    }
                }
                else if (!party.IsMoving && party.LastVisitedSettlement != null && party.LastVisitedSettlement.IsVillage && party.LastVisitedSettlement.Position2D.DistanceSquared(party.Position2D) < 1f && !party.IsRaiding && !party.LastVisitedSettlement.IsUnderRaid && party.HasPerk(DefaultPerks.Medicine.BushDoctor))
                {
                    explainedNumber.AddFactor(DefaultPerks.Medicine.BushDoctor.SecondaryBonus, DefaultPartyHealingModel._bushDoctorPerkText);
                }
            }
            else
            {
                float totalRegulars = (float)(-party.MemberRoster.TotalRegulars) * 0.1f;
                explainedNumber = new ExplainedNumber(totalRegulars, explanation, DefaultPartyHealingModel._starvingText);
            }
            return (float)Math.Round((double)explainedNumber.ResultNumber, 2);
        }

        public override float GetDailyHealingHpForHeroes(MobileParty party, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(2f, explanation, null);
            SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.HealingRateBonus, party, ref explainedNumber);
            if (party.CurrentSettlement != null || party.LastVisitedSettlement != null && party.LastVisitedSettlement.IsVillage && party.LastVisitedSettlement.Position2D.DistanceSquared(party.Position2D) < 1f)
            {
                explainedNumber.Add(5f, DefaultPartyHealingModel._settlementText);
            }
            if (!party.IsGarrison && !party.IsMilitia)
            {
                if (party.PartyMoveMode != MoveModeType.Hold)
                {
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.WalkItOff, party, ref explainedNumber);
                }
                else
                {
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.TriageTent, party, ref explainedNumber);
                }
            }
            if (party.Morale >= Campaign.Current.Models.PartyMoraleModel.HighMoraleValue)
            {
                PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.BestMedicine, party, ref explainedNumber);
            }
            if (party.CurrentSettlement != null)
            {
                explainedNumber.Add(5f, DefaultPartyHealingModel._settlementText);
                if (party.CurrentSettlement.IsTown && party.SiegeEvent == null && !party.CurrentSettlement.IsUnderSiege)
                {
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.GoodLodging, party, ref explainedNumber);
                }
            }
            else if (!party.IsMoving && party.LastVisitedSettlement != null && party.LastVisitedSettlement.IsVillage && party.LastVisitedSettlement.Position2D.DistanceSquared(party.Position2D) < 1f && !party.IsRaiding && !party.LastVisitedSettlement.IsUnderRaid && party.HasPerk(DefaultPerks.Medicine.BushDoctor))
            {
                explainedNumber.AddFactor(DefaultPerks.Medicine.BushDoctor.SecondaryBonus, DefaultPartyHealingModel._bushDoctorPerkText);
            }
            if (party.Party.IsStarving && party.CurrentSettlement == null)
            {
                explainedNumber.Add(-5f, DefaultPartyHealingModel._starvingText);
            }
            return explainedNumber.ResultNumber;
        }

    */