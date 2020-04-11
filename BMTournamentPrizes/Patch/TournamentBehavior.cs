using BMTournamentPrizes;
using BMTournamentPrizes.Behaviors;
using BMTournamentPrizes.Models;
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
using TournamentLib.Models;

namespace BMTweakCollection.Patches
{

    //[HarmonyPatch(typeof(TournamentBehavior), "get_MaximumBetOdds")]
    //public class TournamentBehaviorPatchMaximumBetOdds
    //{
    //    [HarmonyPostfix]
    //    static float Postfix(float value)
    //    {
    //        return TournamentConfiguration.Instance.PrizeConfiguration.MaximumBetOdds;
    //    }
    //}


    [HarmonyPatch(typeof(TournamentBehavior), "CalculateBet")]
    [HarmonyPriority(Priority.Low)]
    public class TournamentBehaviorPatchCalculateBet
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {
            //var tb = Traverse.Create(__instance);

            var maxOdds = TournamentBehavior.MaximumOdd;
            if (TournamentConfiguration.Instance.PrizeConfiguration.MaximumBetOdds > 0)
            {
                maxOdds = TournamentConfiguration.Instance.PrizeConfiguration.MaximumBetOdds;
            }
            if (__instance.IsPlayerParticipating)
            {
                if (__instance.CurrentRound.CurrentMatch == null)
                {
                    //__instance.BetOdd = 0f;
                    //tb.Field("BetOdd").SetValue(0f);
                    typeof(TournamentBehavior).GetProperty("BetOdd").SetValue(__instance, 0f);

                    return false;
                }
                if (__instance.IsPlayerEliminated || !__instance.IsPlayerParticipating)
                {
                    // __instance.OverallExpectedDenars = 0;
                    //this.BetOdd = 0f;
                    // tb.Field("BetOdd").SetValue(0f);
                    // tb.Field("OverallExpectedDenars").SetValue(0);
                    typeof(TournamentBehavior).GetProperty("BetOdd").SetValue(__instance, 0f);
                    typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, 0);

                    return false;
                }

                /* Original */
                List<KeyValuePair<Hero, int>> leaderboard = Campaign.Current.TournamentManager.GetLeaderboard();
                int value = 0;
                int num = 0;
                for (int i = 0; i < leaderboard.Count; i++)
                {
                    if (leaderboard[i].Key == Hero.MainHero)
                    {
                        value = leaderboard[i].Value;
                    }
                    if (leaderboard[i].Value > num)
                    {
                        num = leaderboard[i].Value;
                    }
                }
                float level = 30f + (float)Hero.MainHero.Level + (float)Math.Max(0, value * 12 - num * 2);
                float single = 0f;
                float single1 = 0f;
                float level1 = 0f;
                TournamentMatch[] matches = __instance.CurrentRound.Matches;
                for (int j = 0; j < (int)matches.Length; j++)
                {
                    TournamentMatch tournamentMatch = matches[j];
                    foreach (TournamentTeam team in tournamentMatch.Teams)
                    {
                        float level2 = 0f;
                        foreach (TournamentParticipant participant in team.Participants)
                        {
                            if (participant.Character == CharacterObject.PlayerCharacter)
                            {
                                continue;
                            }
                            int value1 = 0;
                            if (participant.Character.IsHero)
                            {
                                for (int k = 0; k < leaderboard.Count; k++)
                                {
                                    if (leaderboard[k].Key == participant.Character.HeroObject)
                                    {
                                        value1 = leaderboard[k].Value;
                                    }
                                }
                            }
                            level2 += (float)(participant.Character.Level + Math.Max(0, value1 * 8 - num * 2));
                        }
                        if (team.Participants.Any<TournamentParticipant>((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter))
                        {
                            single1 = level2;
                            foreach (TournamentTeam tournamentTeam in tournamentMatch.Teams)
                            {
                                if (team == tournamentTeam)
                                {
                                    continue;
                                }
                                foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
                                {
                                    int num1 = 0;
                                    if (tournamentParticipant.Character.IsHero)
                                    {
                                        for (int l = 0; l < leaderboard.Count; l++)
                                        {
                                            if (leaderboard[l].Key == tournamentParticipant.Character.HeroObject)
                                            {
                                                num1 = leaderboard[l].Value;
                                            }
                                        }
                                    }
                                    level1 += (float)(tournamentParticipant.Character.Level + Math.Max(0, num1 * 8 - num * 2));

                                    //TournamentXP addon for Odd Calculations
                                    //Get armor bonus
                                    //level1 += tournamentParticipant.Character.GetArmArmorSum() * 2 + tournamentParticipant.Character.GetBodyArmorSum() * 3 + tournamentParticipant.Character.GetLegArmorSum() * 2;
                                    ////Get skills based 
                                    //level1 += (float)tournamentParticipant.Character.GetSkillValue(DefaultSkills.Bow)
                                    //    + (float)tournamentParticipant.Character.GetSkillValue(DefaultSkills.OneHanded)
                                    //    + (float)tournamentParticipant.Character.GetSkillValue(DefaultSkills.TwoHanded)
                                    //    + (float)tournamentParticipant.Character.GetSkillValue(DefaultSkills.Throwing)
                                    //    + (float)tournamentParticipant.Character.GetSkillValue(DefaultSkills.Polearm)
                                    //    + (float)tournamentParticipant.Character.GetSkillValue(DefaultSkills.Riding);
                                    //level1 += (float)tournamentParticipant.Character.HitPoints;
                                }
                            }
                        }
                        single += level2;
                    }
                }
                float single2 = (single1 + level) / (level1 + single1 + level);
                float single3 = level / (single1 + level + 0.5f * (single - (single1 + level1)));
                float single4 = single2 * single3;
                float single5 = MathF.Clamp((float)Math.Pow((double)(1f / single4), 0.75), 1.1f, maxOdds);
                typeof(TournamentBehavior).GetProperty("BetOdd").SetValue(__instance, (float)((int)(single5 * 10f)) / 10f);
                /* Original End */
            }
            return false;
        }

        static bool Prepare()
        {
            return TournamentConfiguration.Instance.PrizeConfiguration.OppenentDifficultyAffectsOdds;
        }
    }


    [HarmonyPriority(Priority.Low)]
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
    public class TournamentBehaviorOnPlayerWinTournamentPatch1
    {
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
                MessageBox.Show("Tournament XP Prize: WARNING", "No prize was detected for this tournament.  You should never see this message.  If you are, somehow the prize the game thinks you should get isn't found.  An alternate random item is being created just for you.");
                var prize = TournamentPrizePoolBehavior.GenerateTournamentPrize(__instance.TournamentGame);
                TournamentPrizePoolBehavior.SetTournamentSelectedPrize(__instance.TournamentGame, prize);
            }


            //Override Standard behavior
            if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
            {
                return false;
            }
            GainRenownAction.Apply(Hero.MainHero, __instance.TournamentGame.TournamentWinRenown, false);
            if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader != Hero.MainHero)
            {
                GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, 1f);
            }
            //Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(this._tournamentGame.Prize, 1, true);
            var currentPool = TournamentPrizePoolBehavior.GetTournamentPrizePool(__instance.Settlement);
            var prizeStringId = __instance.TournamentGame.Prize.StringId;

            try
            {
                if (currentPool.Prizes.Where(x => x.EquipmentElement.Item.StringId == prizeStringId).Count() > 0)
                {
                    Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(currentPool.Prizes.Where(x => x.EquipmentElement.Item.StringId == prizeStringId).First(), 1, true);
                }
                else
                {
                    //   Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(currentPool.SelectPrizeItemRosterElement, 1, true);
                    MessageBox.Show("Tournament XP Prize WARNING", "The stored Selected Prize does not equal the tournaments selected item.\nPlease send me a link to your savegame - if you have one right before winning the tournament - on nexus for research.");
                    Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(__instance.TournamentGame.Prize, 1, true);
                }
            }
            catch (Exception ex)
            {
                FileLog.Log("Tournament XP Error Giving Prize:\n" + ex.ToStringFull());
                Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(__instance.TournamentGame.Prize, 1, true);
            }
            

            if (__instance.OverallExpectedDenars > 0)
            {
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, __instance.OverallExpectedDenars, false);
            }
            Campaign.Current.TournamentManager.OnPlayerWinTournament(__instance.TournamentGame.GetType());

            return true;
        }


    }

    [HarmonyPatch(typeof(TournamentBehavior), "EndCurrentMatch")]
    public class TournamentBehaviourPatchBonusRewards
    {
        static bool Prefix(TournamentBehavior __instance)
        {


            //List<TournamentParticipant> GetWinners()
            var winners = (List <TournamentParticipant>)Traverse.Create(__instance.CurrentMatch).Method("GetWinners").GetValue();
            

            if (winners.Where(x => x.Character.HeroObject == Hero.MainHero).Count() > 0)
            {
                if (TournamentConfiguration.Instance.PrizeConfiguration.EnableRenownPerTroopTier)
                {
                    var renownbonus = 0f;
                    foreach (var team in __instance.CurrentMatch.Teams)
                    {
                        var teambonus = 0f;
                        foreach (var p in team.Participants)
                        {
                            teambonus += TournamentPrizePoolBehavior.GetRenownValue(p.Character);
                            if (p.Character.IsHero && p.Character.HeroObject == Hero.MainHero)
                            {
                                teambonus = 0;
                                break;
                            }
                        }
                        renownbonus += teambonus;
                    }
                    GainRenownAction.Apply(Hero.MainHero, renownbonus);
                }

                if (TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentMatchGold > 0)
                {
                    if (TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentMatchGoldImmediate)
                    {
                        GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentMatchGold, false);
                    }
                    else
                    {
                        typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentMatchGold);
                    }
                }
            }
            return true;
        }

       

        static bool Prepare()
        {
            return (TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentMatchGold > 0 || TournamentConfiguration.Instance.PrizeConfiguration.EnableRenownPerTroopTier);
        }
    }
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class TournamentBehaviorOnPlayerWinTournamentPatch2
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {

            typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinGold);
            return true;
        }
        static bool Prepare()
        {
            return TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinGold > 0;
        }

    }
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class TournamentBehaviorOnPlayerWinTournamentPatch3
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {
            GainRenownAction.Apply(Hero.MainHero, TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinRenown, false);
            return true;
        }
        static bool Prepare()
        {
            return TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinRenown > 0;

        }
    }
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class TournamentBehaviorOnPlayerWinTournamentPatch4
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {
            GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinInfluence);
            return true;
        }
        static bool Prepare()
        {
            return TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinInfluence > 0;
        }

    }
}


//[HarmonyPatch(typeof(TournamentBehavior), "GetTournamentPrize")]
//public class GetTournamentPrizePatchCompanionWins
//{
//    public static bool Prefix(ref TournamentBehavior __instance)
//    {
//        if (__instance.CurrentRound.CurrentMatch == null)
//        {
//            if (__instance.CurrentRoundIndex < 3)
//                return true;

//        }

//        return true;
//    }
//}


//    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinMatch")]
//#pragma warning disable RCS1102 // Make class static.
//    public class TournamentBehaviourPatchCharLevels
//#pragma warning restore RCS1102 // Make class static.
//    {
//        static bool Prefix(TournamentBehavior __instance, ref TournamentParticipant[] ____participants)
//        {
//            int numHeroLevels = 0;
//            int bonusMoney = 0;

//            foreach (var team in __instance.CurrentMatch.Teams)
//            {
//                var teamLevels = 0;
//                var teamHasPlayer = false;
//                foreach(var p in team.Participants)
//                {
//                    if (p.Character.IsHero && p.Character.HeroObject != null)
//                    {
//                        teamLevels += p.Character.Level;
//                        if (p.Character.HeroObject == Hero.MainHero)
//                        {
//                            teamHasPlayer = true;
//                            break;
//                        }
//                    }                    
//                }
//                if (!teamHasPlayer)
//                {
//                    numHeroLevels += teamLevels;
//                }
//            }

//            bonusMoney = numHeroLevels * TournamentConfiguration.Instance.PrizeConfiguration.TournamentBonusMoneyBaseNamedCharLevel;
//            typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + bonusMoney);
//            return true;
//        }

//        static bool Prepare()
//        {
//            return TournamentConfiguration.Instance.PrizeConfiguration.TournamentBonusMoneyBaseNamedCharLevel > 0;
//        }
//    }