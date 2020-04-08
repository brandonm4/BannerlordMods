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
                    var prize = TournamentPrizeExpansion.GenerateTournamentPrize(__instance);
                    if (prize == null)
                    {
                        MessageBox.Show("Tournament Prize System", "Error generating Tournament Prize. Reverting to vanilla system.");
                        return;
                    }
                    __result = prize;
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: Tournament Prize System");
                    FileLog.Log(ex.ToStringFull());
                }            
        }



        private static bool Prepare()
        {
            if (TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell
                || TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection
                || TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled
                || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode != PrizeListMode.Vanilla)                
            {
                return true;
            }
            return false;
        }


    }
}
