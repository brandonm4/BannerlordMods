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

        public static bool Prefix(TournamentGame __instance, ref ItemObject __result)
        {
            try
            {
                __result = TournamentPrizeExpansion.GenerateTournamentPrize(__instance);
                if (__result == null)
                {
                    MessageBox.Show("Tournament Prize System", "Error generating Tournament Prize. Reverting to vanilla system.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: Tournament Prize System");
                FileLog.Log(ex.ToStringFull());
            }
            return false;
        }



        private static bool Prepare()
        {
            if (TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell
                || TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection
                || TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled
                || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode.IndexOf("custom", StringComparison.OrdinalIgnoreCase) >= 0
                || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode.IndexOf("town", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            return false;
        }


    }
}
