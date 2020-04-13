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

namespace BMTweakCollection.Patches
{
  

    [HarmonyPatch(typeof(TournamentBehavior), "CalculateBet")]
    public class TournamentBehaviorPatchCalculateBet
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {
            //var tb = Traverse.Create(__instance);

            var maxOdds = TournamentBehavior.MaximumOdd;
            if (TournamentXPSettings.Instance.MaximumBetOdds > 0)
            {
                maxOdds = TournamentXPSettings.Instance.MaximumBetOdds;
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
                int heroLeaderBoardScore = 0;
                int topLeaderBoardScore = 0;
                for (int i = 0; i < leaderboard.Count; i++)
                {
                    if (leaderboard[i].Key == Hero.MainHero)
                    {
                        heroLeaderBoardScore = leaderboard[i].Value;
                    }
                    if (leaderboard[i].Value > topLeaderBoardScore)
                    {
                        topLeaderBoardScore = leaderboard[i].Value;
                    }
                }

                float playerteamThreat = 0f;
                float opponentTeamsThreat = 0f;
                float mainheroThreat = 0f;

                TournamentMatch[] matches = __instance.CurrentRound.Matches;
                for (int j = 0; j < (int)matches.Length; j++)
                {
                    TournamentMatch tournamentMatch = matches[j];
                    foreach (TournamentTeam team in tournamentMatch.Teams)
                    {
                        float teamThreat = 0f;
                        foreach (TournamentParticipant participant in team.Participants)
                        {
                            var playerThreat = TournamentPrizePoolBehavior.GetTournamentThreatLevel(participant.Character);

                            if (participant.Character.IsHero)
                            {
                                for (int k = 0; k < leaderboard.Count; k++)
                                {
                                    if (leaderboard[k].Key == participant.Character.HeroObject)
                                    {
                                        float value1 = leaderboard[k].Value;
                                        playerThreat += Math.Max(0, value1 * 8 - topLeaderBoardScore * 2);
                                    }
                                }
                                if (participant.Character == CharacterObject.PlayerCharacter)
                                {
                                    mainheroThreat = playerThreat;
                                }
                                else
                                {
                                    teamThreat += playerThreat;
                                }
                            }
                            if (team.Participants.Any<TournamentParticipant>((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter))
                            {
                                playerteamThreat = teamThreat;
                            }
                            else
                            {
                                opponentTeamsThreat += teamThreat;
                            }
                        }
                    }




                    float level = 30f + (float)Hero.MainHero.Level + (float)Math.Max(0, heroLeaderBoardScore * 12 - topLeaderBoardScore * 2);
                    float level1 = playerteamThreat + mainheroThreat;
                    float single2 = (playerteamThreat + level) / (level1 + playerteamThreat + level);
                    float single3 = level / (playerteamThreat + level + 0.5f * (opponentTeamsThreat - (playerteamThreat + level1)));
                    float single4 = single2 * single3;
                    float single5 = MathF.Clamp((float)Math.Pow((double)(1f / single4), 0.75), 1.1f, maxOdds);
                    typeof(TournamentBehavior).GetProperty("BetOdd").SetValue(__instance, (float)((int)(single5 * 10f)) / 10f);
                    /* Original End */
                }                
            }
            return false;
        }

        static bool Prepare()
        {
            return TournamentXPSettings.Instance.OppenentDifficultyAffectsOdds;
        }
    }
       
}
/*
 * 
 *     [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class TournamentBehaviorOnPlayerWinTournamentPatch2
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {

            typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + TournamentXPSettings.Instance.BonusTournamentWinGold);
            return true;
        }
        static bool Prepare()
        {
            return TournamentXPSettings.Instance.BonusTournamentWinGold > 0
                && TournamentXPSettings.Instance.EnablePrizeSelection == false;
        }

    }
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class TournamentBehaviorOnPlayerWinTournamentPatch3
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {
            GainRenownAction.Apply(Hero.MainHero, TournamentXPSettings.Instance.BonusTournamentWinRenown, false);
            return true;
        }
        static bool Prepare()
        {
            return TournamentXPSettings.Instance.BonusTournamentWinRenown > 0
               && TournamentXPSettings.Instance.EnablePrizeSelection == false;

        }
    }
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class TournamentBehaviorOnPlayerWinTournamentPatch4
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {
            GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, TournamentXPSettings.Instance.BonusTournamentWinInfluence);
            return true;
        }
        static bool Prepare()
        {
            return TournamentXPSettings.Instance.BonusTournamentWinInfluence > 0
                && TournamentXPSettings.Instance.EnablePrizeSelection == false;
        }

    }
*/

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

//            bonusMoney = numHeroLevels * TournamentXPSettings.Instance.TournamentBonusMoneyBaseNamedCharLevel;
//            typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + bonusMoney);
//            return true;
//        }

//        static bool Prepare()
//        {
//            return TournamentXPSettings.Instance.TournamentBonusMoneyBaseNamedCharLevel > 0;
//        }
//    }
