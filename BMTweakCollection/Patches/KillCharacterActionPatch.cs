using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TournamentsXPanded.Common.Patches;

namespace BMTweakCollection.Patches
{
    internal class KillCharacterActionPatch : PatchBase<KillCharacterActionPatch>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(KillCharacterAction).GetMethod("ApplyInternal", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(KillCharacterActionPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return true;
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            BMTweakCollectionSubModule.Harmony.Patch(TargetMethodInfo,
              postfix: new HarmonyMethod(PatchMethodInfo)
              {
                  priority = Priority.First,
                  //before = new[] { "that.other.harmony.user" }
              }
              );

            Applied = true;
        }

        public override void Reset()
        {
        }
       
        static bool Prefix(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail actionDetail, bool showNotification)
        {
            bool mapEvent;
            bool siegeEvent;
            if (!victim.IsAlive)
            {
                return false;
            }
            victim.EncyclopediaText = (TextObject)typeof(KillCharacterAction).GetMethod("CreateObituary", BindingFlags.NonPublic | BindingFlags.Static| BindingFlags.DeclaredOnly).Invoke(null, new object[] { victim, actionDetail });
            MobileParty partyBelongedTo = victim.PartyBelongedTo;
            if (partyBelongedTo != null)
            {
                mapEvent = partyBelongedTo.MapEvent != null;
            }
            else
            {
                mapEvent = false;
            }
            if (!mapEvent)
            {
                MobileParty mobileParty = victim.PartyBelongedTo;
                if (mobileParty != null)
                {
                    siegeEvent = mobileParty.SiegeEvent != null;
                }
                else
                {
                    siegeEvent = false;
                }
                if (!siegeEvent)
                {
                    if (victim.IsHumanPlayerCharacter && victim.DeathMark == KillCharacterAction.KillCharacterActionDetail.None)
                    {
                        victim.MakeWounded(killer, actionDetail);
                        typeof(CampaignEventDispatcher).GetMethod("OnBeforeMainCharacterDied", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Invoke(CampaignEventDispatcher.Instance, null);
                        //CampaignEventDispatcher.Instance.OnBeforeMainCharacterDied();
                        return false;
                    }
                    StatisticsDataLogHelper.AddLog(StatisticsDataLogHelper.LogAction.KillCharacterAction, new Object[] { victim });
                    if (victim.Clan.Leader == victim && victim != Hero.MainHero)
                    {
                        if (victim.Clan.Heroes.Any<Hero>((Hero x) => {
                            if (x.IsChild || x == victim || !x.IsAlive)
                            {
                                return false;
                            }
                            return x.IsNoble;
                        }))
                        {
                            ChangeClanLeaderAction.ApplyWithoutSelectedNewLeader(victim.Clan);
                        }
                        else if (victim.Clan.Kingdom != null && victim.Clan.Kingdom.RulingClan == victim.Clan)
                        {
                            Clan clan = (Clan)typeof(KillCharacterAction).GetMethod("SelectHeirClanForKingdom", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly).Invoke(null, new object[] { victim.Clan.Kingdom, true });
                            //Clan clan = KillCharacterAction.SelectHeirClanForKingdom(victim.Clan.Kingdom, true);
                            if (clan == null)
                            {
                                DestroyKingdomAction.Apply(victim.Clan.Kingdom);
                            }
                            else
                            {
                                victim.Clan.Kingdom.RulingClan = clan;
                            }
                        }
                    }
                    if (victim.PartyBelongedTo != null && victim.PartyBelongedTo.Leader == victim.CharacterObject)
                    {
                        if (victim.PartyBelongedTo.Army != null)
                        {
                            if (victim.PartyBelongedTo.Army.LeaderParty != victim.PartyBelongedTo)
                            {
                                victim.PartyBelongedTo.Army = null;
                            }
                            else
                            {
                                victim.PartyBelongedTo.Army.DisperseArmy(Army.ArmyDispersionReason.ArmyLeaderIsDead);
                            }
                        }
                        victim.PartyBelongedTo.SetMoveModeHold();
                    }
                    typeof(KillCharacterAction).GetMethod("MakeDead", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly).Invoke(null, new object[] { victim, true });
                    //KillCharacterAction.MakeDead(victim, true);
                    if (victim.GovernorOf != null)
                    {
                        ChangeGovernorAction.ApplyByGiveUpCurrent(victim);
                    }
                    if (actionDetail != KillCharacterAction.KillCharacterActionDetail.Executed)
                    {
                        if (killer != null && !victim.Clan.IsMapFaction)
                        {
                            foreach (Hero all in Hero.All)
                            {
                                if (!all.IsAlive || all == victim || all.IsNoble && all.Clan.Leader != all)
                                {
                                    continue;
                                }
                                if (all.Clan != victim.Clan)
                                {
                                    if (!victim.IsFriend(all))
                                    {
                                        continue;
                                    }
                                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(killer, all, -10, all.IsNoble);
                                }
                                else
                                {
                                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(killer, all, -40, true);
                                }
                            }
                        }
                    }
                    else if (actionDetail == KillCharacterAction.KillCharacterActionDetail.Executed && killer == Hero.MainHero)
                    {
                        if (victim.GetTraitLevel(DefaultTraits.Honor) >= 0 )
                        {
                            //TraitLevelingHelper.OnLordExecuted();
                            foreach (Hero hero in Hero.All)
                            {
                                if (!hero.IsAlive || hero == victim || hero.IsNoble && hero.Clan.Leader != hero)
                                {
                                    continue;
                                }
                                if (hero.Clan == victim.Clan)
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(hero, -60, true, true);
                                }
                                else if (victim.IsFriend(hero))
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(hero, -30, true, hero.IsNoble);
                                }
                                else if (hero.MapFaction != victim.MapFaction || hero.CharacterObject.Occupation != Occupation.Lord)
                                {
                                    if (hero.GetTraitLevel(DefaultTraits.Honor) <= 0 || !hero.IsNoble)
                                    {
                                        continue;
                                    }
                                    ChangeRelationAction.ApplyPlayerRelation(hero, -10, true, hero.IsNoble);
                                }
                                else
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(hero, -10, true, false);
                                }
                            }
                        }
                        else
                        {
                            foreach (Hero all1 in Hero.All)
                            {
                                if (!all1.IsAlive || all1 == victim || all1.IsNoble && all1.Clan.Leader != all1)
                                {
                                    continue;
                                }
                                if (all1.Clan == victim.Clan)
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(all1, -30, true, true);
                                }
                                else if (!victim.IsFriend(all1))
                                {
                                    if (all1.MapFaction != victim.MapFaction || all1.CharacterObject.Occupation != Occupation.Lord)
                                    {
                                        continue;
                                    }
                                    ChangeRelationAction.ApplyPlayerRelation(all1, -5, true, all1.IsNoble);
                                }
                                else
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(all1, -15, true, all1.IsNoble);
                                }
                            }
                        }
                    }
                    if (!victim.Clan.IsDeactivated && !victim.Clan.Heroes.Any<Hero>((Hero x) => {
                        if (x.IsChild || x == victim)
                        {
                            return false;
                        }
                        //return x.IsAlive;
                        return false;
                    }))
                    {
                        DestroyClanAction.Apply(victim.Clan);
                    }

                    typeof(CampaignEventDispatcher).GetMethod("OnHeroKilled", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Invoke(CampaignEventDispatcher.Instance, new object[] { victim, killer, actionDetail, showNotification});
                    //CampaignEventDispatcher.Instance.OnHeroKilled(victim, killer, actionDetail, showNotification);
                    if (victim.CurrentSettlement != null && victim.StayingInSettlementOfNotable != null)
                    {
                        victim.StayingInSettlementOfNotable = null;
                    }
                    return false;
                }
            }
            victim.MakeWounded(killer, actionDetail);
            return false;
        }
    }
}


/*
 * private static void ApplyInternal(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail actionDetail, bool showNotification)
        {
            bool mapEvent;
            bool siegeEvent;
            if (!victim.IsAlive)
            {
                return;
            }
            victim.EncyclopediaText = KillCharacterAction.CreateObituary(victim, actionDetail);
            MobileParty partyBelongedTo = victim.PartyBelongedTo;
            if (partyBelongedTo != null)
            {
                mapEvent = partyBelongedTo.MapEvent;
            }
            else
            {
                mapEvent = false;
            }
            if (!mapEvent)
            {
                MobileParty mobileParty = victim.PartyBelongedTo;
                if (mobileParty != null)
                {
                    siegeEvent = mobileParty.SiegeEvent;
                }
                else
                {
                    siegeEvent = false;
                }
                if (!siegeEvent)
                {
                    if (victim.IsHumanPlayerCharacter && victim.DeathMark == KillCharacterAction.KillCharacterActionDetail.None)
                    {
                        victim.MakeWounded(killer, actionDetail);
                        CampaignEventDispatcher.Instance.OnBeforeMainCharacterDied();
                        return;
                    }
                    StatisticsDataLogHelper.AddLog(StatisticsDataLogHelper.LogAction.KillCharacterAction, new Object[] { victim });
                    if (victim.Clan.Leader == victim && victim != Hero.MainHero)
                    {
                        if (victim.Clan.Heroes.Any<Hero>((Hero x) => {
                            if (x.IsChild || x == victim || !x.IsAlive)
                            {
                                return false;
                            }
                            return x.IsNoble;
                        }))
                        {
                            ChangeClanLeaderAction.ApplyWithoutSelectedNewLeader(victim.Clan);
                        }
                        else if (victim.Clan.Kingdom != null && victim.Clan.Kingdom.RulingClan == victim.Clan)
                        {
                            Clan clan = KillCharacterAction.SelectHeirClanForKingdom(victim.Clan.Kingdom, true);
                            if (clan == null)
                            {
                                DestroyKingdomAction.Apply(victim.Clan.Kingdom);
                            }
                            else
                            {
                                victim.Clan.Kingdom.RulingClan = clan;
                            }
                        }
                    }
                    if (victim.PartyBelongedTo != null && victim.PartyBelongedTo.Leader == victim.CharacterObject)
                    {
                        if (victim.PartyBelongedTo.Army != null)
                        {
                            if (victim.PartyBelongedTo.Army.LeaderParty != victim.PartyBelongedTo)
                            {
                                victim.PartyBelongedTo.Army = null;
                            }
                            else
                            {
                                victim.PartyBelongedTo.Army.DisperseArmy(Army.ArmyDispersionReason.ArmyLeaderIsDead);
                            }
                        }
                        victim.PartyBelongedTo.SetMoveModeHold();
                    }
                    KillCharacterAction.MakeDead(victim, true);
                    if (victim.GovernorOf != null)
                    {
                        ChangeGovernorAction.ApplyByGiveUpCurrent(victim);
                    }
                    if (actionDetail != KillCharacterAction.KillCharacterActionDetail.Executed)
                    {
                        if (killer != null && !victim.Clan.IsMapFaction)
                        {
                            foreach (Hero all in Hero.All)
                            {
                                if (!all.IsAlive || all == victim || all.IsNoble && all.Clan.Leader != all)
                                {
                                    continue;
                                }
                                if (all.Clan != victim.Clan)
                                {
                                    if (!victim.IsFriend(all))
                                    {
                                        continue;
                                    }
                                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(killer, all, -10, all.IsNoble);
                                }
                                else
                                {
                                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(killer, all, -40, true);
                                }
                            }
                        }
                    }
                    else if (actionDetail == KillCharacterAction.KillCharacterActionDetail.Executed && killer == Hero.MainHero)
                    {
                        if (victim.GetTraitLevel(DefaultTraits.Honor) >= 0)
                        {
                            TraitLevelingHelper.OnLordExecuted();
                            foreach (Hero hero in Hero.All)
                            {
                                if (!hero.IsAlive || hero == victim || hero.IsNoble && hero.Clan.Leader != hero)
                                {
                                    continue;
                                }
                                if (hero.Clan == victim.Clan)
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(hero, -60, true, true);
                                }
                                else if (victim.IsFriend(hero))
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(hero, -30, true, hero.IsNoble);
                                }
                                else if (hero.MapFaction != victim.MapFaction || hero.CharacterObject.Occupation != Occupation.Lord)
                                {
                                    if (hero.GetTraitLevel(DefaultTraits.Honor) <= 0 || !hero.IsNoble)
                                    {
                                        continue;
                                    }
                                    ChangeRelationAction.ApplyPlayerRelation(hero, -10, true, hero.IsNoble);
                                }
                                else
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(hero, -10, true, false);
                                }
                            }
                        }
                        else
                        {
                            foreach (Hero all1 in Hero.All)
                            {
                                if (!all1.IsAlive || all1 == victim || all1.IsNoble && all1.Clan.Leader != all1)
                                {
                                    continue;
                                }
                                if (all1.Clan == victim.Clan)
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(all1, -30, true, true);
                                }
                                else if (!victim.IsFriend(all1))
                                {
                                    if (all1.MapFaction != victim.MapFaction || all1.CharacterObject.Occupation != Occupation.Lord)
                                    {
                                        continue;
                                    }
                                    ChangeRelationAction.ApplyPlayerRelation(all1, -5, true, all1.IsNoble);
                                }
                                else
                                {
                                    ChangeRelationAction.ApplyPlayerRelation(all1, -15, true, all1.IsNoble);
                                }
                            }
                        }
                    }
                    if (!victim.Clan.IsDeactivated && !victim.Clan.Heroes.Any<Hero>((Hero x) => {
                        if (x.IsChild || x == victim)
                        {
                            return false;
                        }
                        return x.IsAlive;
                    }))
                    {
                        DestroyClanAction.Apply(victim.Clan);
                    }
                    CampaignEventDispatcher.Instance.OnHeroKilled(victim, killer, actionDetail, showNotification);
                    if (victim.CurrentSettlement != null && victim.StayingInSettlementOfNotable != null)
                    {
                        victim.StayingInSettlementOfNotable = null;
                    }
                    return;
                }
            }
            victim.MakeWounded(killer, actionDetail);
        }

    */