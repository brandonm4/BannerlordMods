using BMTournamentPrizes.Extensions;
using BMTournamentPrize.Models;
using BMTournamentPrizes.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using Helpers;
using System.Windows.Forms;

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
            catch(Exception ex)
            {                
                FileLog.Log("ERROR: Tournament Prize System");
                FileLog.Log(ex.ToStringFull());
            }
            return false;
        }

       

        private static bool Prepare()
        {
            if (BMTournamentPrizeConfiguration.Instance.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell
                || BMTournamentPrizeConfiguration.Instance.EnablePrizeSelection
                || BMTournamentPrizeConfiguration.Instance.TournamentPrizeRerollEnabled
                || BMTournamentPrizeConfiguration.Instance.PrizeListMode.IndexOf("custom", StringComparison.OrdinalIgnoreCase) >= 0
                || BMTournamentPrizeConfiguration.Instance.PrizeListMode.IndexOf("town", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            return false;
        }

       
    }
}
