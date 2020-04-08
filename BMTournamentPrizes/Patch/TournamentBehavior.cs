using BMTournamentPrizes;
using BMTournamentPrizes.Models;
using HarmonyLib;
using SandBox.TournamentMissions.Missions;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TournamentLib.Models;

namespace BMTweakCollection.Patches
{


    [HarmonyPatch(typeof(TournamentBehavior), "CalculateBet")]
    public class TournamentBehaviorPatchCalculateBet
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {

            //var tb = Traverse.Create(__instance);

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
                float single = 50f;
                float matchScore = 0f;
                float playerTeamScore = 0f;
                TournamentMatch[] matches = __instance.CurrentRound.Matches;
                for (int i = 0; i < (int)matches.Length; i++)
                {
                    foreach (TournamentTeam team in matches[i].Teams)
                    {
                        float level = 0f;
                        foreach (TournamentParticipant participant in team.Participants)
                        {
                            //level += (float)participant.Character.Level;
                            level += (float)participant.Character.Level;
                            //If they are a named hero, increase their score a bit
                            if (participant.Character.IsHero)
                            {
                                level += 50f;
                            }
                            //Tack on armor values
                            level += participant.Character.GetArmArmorSum() * 2 + participant.Character.GetBodyArmorSum() * 3 + participant.Character.GetLegArmorSum() * 2;
                            //Get skills based 
                            level += (float)participant.Character.GetSkillValue(DefaultSkills.Bow)
                                + (float)participant.Character.GetSkillValue(DefaultSkills.OneHanded)
                                + (float)participant.Character.GetSkillValue(DefaultSkills.TwoHanded)
                                + (float)participant.Character.GetSkillValue(DefaultSkills.Throwing)
                                + (float)participant.Character.GetSkillValue(DefaultSkills.Polearm)
                                + (float)participant.Character.GetSkillValue(DefaultSkills.Riding);
                            //bool hasBow, hasTwoH, hasOneH, hasHorse = false;

                            //Get skills based on equipment
                            //Unfortunately we don't know the equipped match equipment at the betting stage so have to use just all their skills for now
                            /*
                            for (var ie = 0; ie < 5; ie++)
                            {
                                var slotEquipment = participant.MatchEquipment.GetEquipmentFromSlot((EquipmentIndex)ie);
                                if (slotEquipment.Item != null)
                                {
                                    switch (slotEquipment.Item.ItemType)
                                    {
                                        case ItemObject.ItemTypeEnum.Bow:
                                            level += (float)participant.Character.GetSkillValue(DefaultSkills.Bow);
                                            break;
                                        case ItemObject.ItemTypeEnum.OneHandedWeapon:
                                            level += (float)participant.Character.GetSkillValue(DefaultSkills.OneHanded);
                                            break;
                                        case ItemObject.ItemTypeEnum.Thrown:
                                            level += (float)participant.Character.GetSkillValue(DefaultSkills.Throwing);
                                            break;
                                        case ItemObject.ItemTypeEnum.TwoHandedWeapon:
                                            level += (float)participant.Character.GetSkillValue(DefaultSkills.TwoHanded);
                                            break;
                                        case ItemObject.ItemTypeEnum.Polearm:
                                            level += (float)participant.Character.GetSkillValue(DefaultSkills.Polearm);
                                            break;
                                        case ItemObject.ItemTypeEnum.Crossbow:
                                            level += (float)participant.Character.GetSkillValue(DefaultSkills.Crossbow);
                                            break;

                                    }
                                }
                            }

                            if (participant.MatchEquipment.Horse.Item != null)
                            {                              
                                            level += (float)participant.Character.GetSkillValue(DefaultSkills.Riding);                              
                            }
                            */
                        }
                        if (team.Participants.Any<TournamentParticipant>((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter))
                        {
                            level += single;  //Human player gets an additional score increase
                            playerTeamScore = level;
                        }
                        matchScore += level;
                    }
                }
                float single3 = MathF.Clamp((float)Math.Sqrt((double)(matchScore / playerTeamScore)), 1.01f, TournamentConfiguration.Instance.PrizeConfiguration.MaximumBetOdds);
                //tb.Field("BetOdd").SetValue(Math.Min(single3, TournamentConfiguration.Instance.PrizeConfiguration.MaximumBetOdds));                
                typeof(TournamentBehavior).GetProperty("BetOdd").SetValue(__instance, Math.Min(single3, TournamentConfiguration.Instance.PrizeConfiguration.MaximumBetOdds));
            }
            return false;
        }

        static bool Prepare()
        {
            return TournamentConfiguration.Instance.PrizeConfiguration.OppenentDifficultyAffectsOdds;
        }
    }

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
                var prize = BMTournamentPrizesMain.TournamentPrizeExpansionModel.GenerateTournamentPrize(__instance.TournamentGame);
                BMTournamentPrizesMain.TournamentPrizeExpansionModel.SetTournamentSelectedPrize(__instance.TournamentGame, prize);
            }
            return true;
        }


    }

    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinMatch")]
    public class TournamentBehaviourPatchBonusGold
    {
        static bool Prefix(TournamentBehavior __instance)
        {
            if (TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentMatchGold > 0)
            {
                typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentMatchGold);
            }
            if (TournamentConfiguration.Instance.PrizeConfiguration.EnableRenownPerTroopTier)
            {
                var renownbonus = 0f;
                foreach (var team in __instance.LastMatch.Teams)
                {
                    var teambonus = 0f;
                    foreach (var p in team.Participants)
                    {
                        teambonus += GetRenownValue(p.Character);
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
            return true;
        }

        static float GetRenownValue(CharacterObject character)
        {
            var worth = 0f;
            if (character.IsHero)
            {
                worth += TournamentConfiguration.Instance.PrizeConfiguration.RenownPerHeroProperty[(int)RenownHeroTier.HeroBase];
                var hero = character.HeroObject;
                if (hero != null)
                {
                    if (hero.IsNoble)
                    {
                        worth += TournamentConfiguration.Instance.PrizeConfiguration.RenownPerHeroProperty[(int)RenownHeroTier.IsNoble];
                    }
                    if (hero.IsNotable)
                    {
                        worth += TournamentConfiguration.Instance.PrizeConfiguration.RenownPerHeroProperty[(int)RenownHeroTier.IsNotable];
                    }
                    if (hero.IsCommander)
                    {
                        worth += TournamentConfiguration.Instance.PrizeConfiguration.RenownPerHeroProperty[(int)RenownHeroTier.IsCommander];
                    }
                    if (hero.IsMinorFactionHero)
                    {
                        worth += TournamentConfiguration.Instance.PrizeConfiguration.RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                    }
                    if (hero.IsFactionLeader)
                    {
                        if (hero.MapFaction.IsKingdomFaction)
                            worth += TournamentConfiguration.Instance.PrizeConfiguration.RenownPerHeroProperty[(int)RenownHeroTier.IsMajorFactionLeader];
                        if (hero.MapFaction.IsMinorFaction)
                            worth += TournamentConfiguration.Instance.PrizeConfiguration.RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                    }
                }
            }
            else
            {
                worth += TournamentConfiguration.Instance.PrizeConfiguration.RenownPerTroopTier[character.Tier];
            }
            return worth;
        }

        static bool Prepare()
        {
            return (TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentMatchGold > 0 || TournamentConfiguration.Instance.PrizeConfiguration.EnableRenownPerTroopTier);
        }
    }
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
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
    public class TournamentBehaviorOnPlayerWinTournamentPatch3
    {
        public static bool Prefix(ref TournamentBehavior __instance)
        {
            GainRenownAction.Apply(Hero.MainHero, TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinRenown, false);
            return true;
        }
        static bool Prepare()
        {
            return TournamentConfiguration.Instance.PrizeConfiguration.BonusTournamentWinRenown + 3 > 3;

        }
    }
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinTournament")]
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