using BMTournamentPrize.Models;
using HarmonyLib;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(TournamentBehavior), "OnPlayerWinMatch")]
#pragma warning disable RCS1102 // Make class static.
    public class TournamentBehaviourPatchCharLevels
#pragma warning restore RCS1102 // Make class static.
    {
        static bool Prefix(TournamentBehavior __instance, ref TournamentParticipant[] ____participants)
        {
            int numHeroLevels = 0;
            int bonusMoney = 0;

            foreach(var p in ____participants)
            {
                if (p.Character.IsHero && p.Character.HeroObject != null && p.Character.HeroObject != Hero.MainHero)
                {
                    numHeroLevels += p.Character.HeroObject.Level;
                }
            }
            bonusMoney = numHeroLevels * BMTournamentPrizeConfiguration.Instance.TournamentBonusMoneyBaseNamedCharLevel;
            typeof(TournamentBehavior).GetProperty("OverallExpectedDenars").SetValue(__instance, __instance.OverallExpectedDenars + bonusMoney);
            return true;
        }      

        static bool Prepare()
        {
            return BMTournamentPrizeConfiguration.Instance.TournamentBonusMoneyBaseNamedCharLevel > 0;
        }
    }
}
