using HarmonyLib;

using SandBox.TournamentMissions.Missions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Patches.TournamentBehaviorClass
{
    public class OnPlayerWinTournament : PatchBase<OnPlayerWinTournament>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentBehavior).GetMethod("OnPlayerWinTournament", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(OnPlayerWinTournament).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            //return TournamentXPSettings.Instance.EnableItemModifiersForPrizes || TournamentXPSettings.Instance.BonusTournamentWinInfluence > 0 || TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament > 0;
            return true;
        }

        public override void Reset()
        {
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
              prefix: new HarmonyMethod(PatchMethodInfo)
              {
                  priority = Priority.First,
                  after = new[] { "mod.bannerlord.mipen", "com.dealman.tournament.patch" },
              }
              );

            Applied = true;
        }

        //REVISIT - convert to transpiler patch to just change our prize payment
        // All we really need to change is instead of giving an ItemObject - which has no ItemModifers, we give them an ItemRosterEquipement, which can have ItemModifiers
        private static bool Prefix(ref TournamentBehavior __instance)
        {
            //Override Standard behavior
            if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
            {
                return false;
            }
            var tournamentInfo = TournamentsXPandedBehavior.GetTournamentInfo(__instance.TournamentGame.Town);
            var settlement = __instance.Settlement;

            /* Give Gold, Influence, Renown */
            if (__instance.OverallExpectedDenars > 0)
            {
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, __instance.OverallExpectedDenars, false);
            }
            GainRenownAction.Apply(Hero.MainHero, __instance.TournamentGame.TournamentWinRenown, false);
            if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader != Hero.MainHero)
            {
                GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, tournamentInfo.Rewards.BonusInfluence);
            }
            /* Give Reputation */
            try
            {
                if (TournamentXPSettings.Instance.BonusReputationForTournamentWin > 0)
                {
                    List<Hero> heros = new List<Hero>();

                    foreach (var n in settlement.Notables)
                    {
                        if (!heros.Contains(n))
                            heros.Add(n);
                    }                    
                    foreach (var v in settlement.BoundVillages)
                    {
                        foreach (var n in v.Settlement.Notables)
                        {
                            if (!heros.Contains(n))
                                heros.Add(n);
                        }
                    }
                    if (settlement.OwnerClan.Heroes.Any<Hero>())
                    {
                        foreach (var n in settlement.OwnerClan.Heroes)
                        {
                            if (!heros.Contains(n))
                                heros.Add(n);
                        }
                    }
                    if (settlement.HeroesWithoutParty.Count > 0)
                    {
                        foreach (var n in settlement.HeroesWithoutParty)
                        {
                            if (!heros.Contains(n))
                                heros.Add(n);
                        }
                    }

                    foreach(var h in heros)
                    {
                        if (MBRandom.RandomFloat < .51f)
                        {
                            ChangeRelationAction.ApplyPlayerRelation(h, TournamentXPSettings.Instance.BonusReputationForTournamentWin, true, false);
                        }
                    }
                }

                MBTextManager.SetTextVariable("RELATION_VALUE", TournamentXPSettings.Instance.BonusReputationForTournamentWin, false);
                InformationManager.AddQuickInformation(new TextObject("{=o0qwDa0q}Your relation increased by {RELATION_VALUE} with nearby notables.", null), 0, null, "");
            }
            catch (Exception ex)
            {
                ErrorLog.Log("Error giving Reputation:\n" + ex.ToStringFull());
            }

            /* Give Item Prize */
            if (!TournamentXPSettings.Instance.EnableItemModifiersForPrizes)
            {
                if (!tournamentInfo.Rewards.PrizeGiven)
                {
                    try
                    {
                        Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(__instance.TournamentGame.Prize, 1, true);
                        tournamentInfo.Rewards.PrizeGiven = true;
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("Error assigning prize\n" + ex.ToStringFull());
                    }
                }
            }
            else
            {
                try
                {
                    if (!tournamentInfo.Rewards.PrizeGiven)
                    {
                        try
                        {
                            if (!String.IsNullOrWhiteSpace(tournamentInfo.SelectedPrizeStringId))
                            {
                                Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(tournamentInfo.SelectedPrizeItem.ToItemRosterElement().EquipmentElement, 1, true);
                                tournamentInfo.Rewards.PrizeGiven = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Log("Error assigning prize\n" + ex.ToStringFull());
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("ERROR: Tournament XPanded: OnPlayerWinTournament\nError Awarding Prize");
                    ErrorLog.Log("TournamentPrizePool:\n");
                    if (tournamentInfo != null)
                    {
                        ErrorLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(tournamentInfo));
                    }
                    ErrorLog.Log(ex.ToStringFull());
                }
            }

            if (!tournamentInfo.Rewards.PrizeGiven)
            {
                try
                {
                    Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(__instance.TournamentGame.Prize, 1, true);
                    tournamentInfo.Rewards.PrizeGiven = true;
                }
                catch (Exception ex2)
                {
                    ErrorLog.Log("Error assigning prize\n" + ex2.ToStringFull());
                }
            }

            tournamentInfo.ReRollsUsed = 0;
            Campaign.Current.TournamentManager.OnPlayerWinTournament(__instance.TournamentGame.GetType());

            return false;
        }
    }
}