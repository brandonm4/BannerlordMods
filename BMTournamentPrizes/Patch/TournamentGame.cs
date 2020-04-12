using BMTournamentPrizes.Behaviors;
using BMTournamentPrizes.Models;
using HarmonyLib;
using System;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TournamentLib.Extensions;
using TournamentLib.Models;

namespace BMTournamentPrizes.Patch
{
    [HarmonyPatch(typeof(TournamentGame), "GetTournamentPrize")]
    public class GetTournamentPrizePatch1
    {

        public static void Postfix(TournamentGame __instance, ref ItemObject __result)
        {
            //__result currently has stock item.          
            try
            {
                ItemObject prize;
                if (TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection
                    || TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled
                    || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode != PrizeListMode.Vanilla)
                {
                    TournamentPrizePoolBehavior.GetTournamentPrizePool(__instance.Town.Settlement, __result);
                    prize = TournamentPrizePoolBehavior.GenerateTournamentPrize(__instance, null, false);
                }
                else
                {
                    prize = TournamentPrizePoolBehavior.GetTournamentPrizeVanilla(__instance.Town.Settlement);
                }
                if (prize == null)
                {
                    MessageBox.Show("Tournament Prize System", "Error generating Tournament Prize. Reverting to vanilla system.");
                    return;
                }
                __result = prize;
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: Tournament Prize System: GetTournamentPrize");
                FileLog.Log(ex.ToStringFull());
            }
        }



        private static bool Prepare()
        {
            if (TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell
                || TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection
                || TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled
                || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode != PrizeListMode.Vanilla
                || TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeTypeFilterToLists)
            {
                return true;
            }
            return false;
        }


    }

    [HarmonyPatch(typeof(TournamentGame), "get_TournamentWinRenown")]
    public class TournamentGameGetTournamentWinRenownPatch
    {
        public static void Postfix(ref float __result)
        {
            __result += TournamentPrizePoolBehavior.TournamentReward.BonusRenown;
        }
        static bool Prepare()
        {
            return true;
        }
    }
}
