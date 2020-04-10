using BMTournamentPrizes.Behaviors;
using BMTournamentPrizes.Models;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TournamentLib.Models;

namespace BMTournamentPrizes.Patch
{

    [HarmonyPatch(typeof(TournamentManager), "AddTournament")]
    public class TournamentManagerPatchAddTournament1
    {
        public static void Prefex(TournamentGame game)
        {
            //BMTournamentPrizesMain.TournamentPrizeExpansionModel.ClearTournamentPrizes(game.Town.Settlement.StringId);
            if (game.Prize == null)
            {
                FileLog.Log("BMTournamentPrize: AddTournament Detected a missing prize.  Correcting with vanilla item.");
                //Do a final check to make sure nothing is missing
                var prize = TournamentPrizePoolBehavior.GetTournamentPrizeVanilla(game.Town.Settlement);
                TournamentPrizePoolBehavior.SetTournamentSelectedPrize(game, prize);
            }
        }
        static bool Prepare()
        {
            return (TournamentConfiguration.Instance.PrizeConfiguration.EnableConfigReloadRealTime || TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection);            
        }
    }

    [HarmonyPatch(typeof(TournamentManager), "ResolveTournament")]
    public class TournamentManagerPatchResolveTournament1
    {
        public static void Prefex(TournamentGame tournament)
        {
            TournamentPrizePoolBehavior.ClearTournamentPrizes(tournament.Town.Settlement.StringId);
        }
        static bool Prepare()
        {
            return (TournamentConfiguration.Instance.PrizeConfiguration.EnableConfigReloadRealTime || TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection);
        }
    }
}
