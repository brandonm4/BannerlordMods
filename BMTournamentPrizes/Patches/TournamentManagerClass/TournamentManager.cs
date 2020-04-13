using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Models;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;


namespace TournamentsXPanded.Patch
{

    [HarmonyPatch(typeof(TournamentManager), "AddTournament")]
    public class TournamentManagerPatchAddTournament1
    {
        public static void Prefex(TournamentGame game)
        {
            //TournamentsXPandedMain.TournamentPrizeExpansionModel.ClearTournamentPrizes(game.Town.Settlement.StringId);
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
            return (TournamentXPSettings.Instance.EnablePrizeSelection);            
        }
    }

    [HarmonyPatch(typeof(TournamentManager), "ResolveTournament")]
    public class TournamentManagerPatchResolveTournament1
    {
        public static void Prefex(TournamentGame tournament)
        {
            TournamentPrizePoolBehavior.ClearTournamentPrizes(tournament.Town.Settlement);            
        }
        static bool Prepare()
        {
            return (TournamentXPSettings.Instance.EnablePrizeSelection);
        }
    }
}
