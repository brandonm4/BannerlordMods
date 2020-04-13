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
    class OnPlayerWinTournament : PatchBase<AfterStart>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentBehavior).GetMethod("OnPlayerWinTournament");

        private static readonly MethodInfo PatchMethodInfo = typeof(OnPlayerWinTournament).GetMethod(nameof(Prefix));
        public override bool IsApplicable(Game game)
        {
            return (TournamentXPSettings.Instance.EnablePrizeSelection);
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

        public static bool Prefix(ref TournamentBehavior __instance)
        {
            bool bDofix = false;
            if (__instance.TournamentGame.Prize == null)
            {
                bDofix = true;
            }
            else if (__instance.TournamentGame.Prize.ItemType == ItemObject.ItemTypeEnum.Invalid)
            {
                bDofix = true;
            }
            if (bDofix)
            {
                FileLog.Log("Tournament XP Prize: WARNING\nNo prize was detected for this tournament.  You should never see this message.  If you are, somehow the prize the game thinks you should get isn't found.  An alternate random item is being created just for you.");
                var prize = TournamentPrizePoolBehavior.GenerateTournamentPrize(__instance.TournamentGame);
                TournamentPrizePoolBehavior.SetTournamentSelectedPrize(__instance.TournamentGame, prize);
            }
            //Override Standard behavior
            if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
            {
                return false;
            }

            //Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(this._tournamentGame.Prize, 1, true);
            var currentPool = TournamentPrizePoolBehavior.GetTournamentPrizePool(__instance.Settlement);
            var prizeStringId = __instance.TournamentGame.Prize.StringId;
            try
            {
                if (currentPool.Prizes.Where(x => x.EquipmentElement.Item.StringId == prizeStringId).Count() > 0)
                {
                    if (!TournamentPrizePoolBehavior.TournamentReward.PrizeGiven)
                    {
                        Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(currentPool.Prizes.Where(x => x.EquipmentElement.Item.StringId == prizeStringId).First(), 1, true);
                        TournamentPrizePoolBehavior.TournamentReward.PrizeGiven = true;
                    }
                }
                else
                {
                    if (!TournamentPrizePoolBehavior.TournamentReward.PrizeGiven)
                    {
                        //   Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(currentPool.SelectPrizeItemRosterElement, 1, true);
                        FileLog.Log("Tournament XP Prize WARNING\nThe stored Selected Prize does not equal the tournaments selected item.\nPlease send me a link to your savegame - if you have one right before winning the tournament - on nexus for research.");
                        Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(__instance.TournamentGame.Prize, 1, true);
                        TournamentPrizePoolBehavior.TournamentReward.PrizeGiven = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!TournamentPrizePoolBehavior.TournamentReward.PrizeGiven)
                {
                    FileLog.Log("Tournament XP Error Giving Prize:\n" + ex.ToStringFull());
                    Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(__instance.TournamentGame.Prize, 1, true);
                }
            }


            if (__instance.OverallExpectedDenars > 0)
            {
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, __instance.OverallExpectedDenars, false);
            }
            GainRenownAction.Apply(Hero.MainHero, __instance.TournamentGame.TournamentWinRenown, false);
            if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader != Hero.MainHero)
            {
                GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, TournamentPrizePoolBehavior.TournamentReward.BonusInfluence);
            }

            Campaign.Current.TournamentManager.OnPlayerWinTournament(__instance.TournamentGame.GetType());

            return false;
        }

    }
}
