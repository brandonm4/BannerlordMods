using TournamentsXPanded;
using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Models;
using HarmonyLib;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TournamentLib.Extensions;
using System.Reflection;

namespace TournamentsXPanded.Patches.TournamentBehaviorClass
{
    public class OnPlayerWinTournament : PatchBase<OnPlayerWinTournament>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentBehavior).GetMethod("OnPlayerWinTournament", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(OnPlayerWinTournament).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        public override bool IsApplicable(Game game)
        {
            return TournamentXPSettings.Instance.EnableItemModifiersForPrizes || TournamentXPSettings.Instance.BonusTournamentWinInfluence > 0;
        }
        public override void Reset() { }

        public override void Apply(Game game)
        {
            if (Applied) return;
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
        public static bool Prefix(ref TournamentBehavior __instance)
        {        
            //Override Standard behavior
            if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
            {
                return false;
            }

            /* Give Gold, Influence, Renown */
            if (__instance.OverallExpectedDenars > 0)
            {
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, __instance.OverallExpectedDenars, false);
            }
            GainRenownAction.Apply(Hero.MainHero, __instance.TournamentGame.TournamentWinRenown, false);
            if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader != Hero.MainHero)
            {
                GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, TournamentPrizePoolBehavior.TournamentReward.BonusInfluence);
            }

            /* Give Item Prize */
            if (!TournamentXPSettings.Instance.EnableItemModifiersForPrizes)
            {
                if (!TournamentPrizePoolBehavior.TournamentReward.PrizeGiven)
                {
                    Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(__instance.TournamentGame.Prize, 1, true);
                    TournamentPrizePoolBehavior.TournamentReward.PrizeGiven = true;
                }
            }
            else
            {
                TournamentPrizePool currentPool = null;
                string prizeStringId = "";
                try
                {
                    if (!TournamentPrizePoolBehavior.TournamentReward.PrizeGiven)
                    {
                        currentPool = TournamentPrizePoolBehavior.GetTournamentPrizePool(__instance.Settlement);
                        prizeStringId = __instance.TournamentGame.Prize.StringId;
                        Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(currentPool.Prizes.Where(x => x.EquipmentElement.Item.StringId == prizeStringId).First(), 1, true);
                        TournamentPrizePoolBehavior.TournamentReward.PrizeGiven = true;
                    }
                }
                catch(Exception ex)
                {
                    FileLog.Log("ERROR: Tournament XPanded: OnPlayerWinTournament\nError Awarding Prize");
                    
                    FileLog.Log("TournamentPrizePool:\n");
                    if (currentPool != null)
                        FileLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(currentPool));
                    FileLog.Log(ex.ToStringFull());

                    if (!TournamentPrizePoolBehavior.TournamentReward.PrizeGiven)
                    {
                        Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(__instance.TournamentGame.Prize, 1, true);
                        TournamentPrizePoolBehavior.TournamentReward.PrizeGiven = true;
                    }
                }
            }    
            Campaign.Current.TournamentManager.OnPlayerWinTournament(__instance.TournamentGame.GetType());

            return false;
        }

    }
}
