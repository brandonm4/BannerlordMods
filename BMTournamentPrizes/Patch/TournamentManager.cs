using BMTournamentPrize.Models;
using BMTournamentPrizes.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;

namespace BMTournamentPrizes.Patch
{

    [HarmonyPatch(typeof(TournamentManager), "AddTournament")]
    public class TournamentManagerPatchAddTournament1
    {
        public static void Prefex(TournamentGame game)
        {
            TournamentPrizeExpansion.ClearTournamentPrizes(game.Town.Settlement.StringId);
        }
        static bool Prepare()
        {
            return (BMTournamentPrizeConfiguration.Instance.EnableConfigReloadRealTime || BMTournamentPrizeConfiguration.Instance.EnablePrizeSelection);
        }
    }

    [HarmonyPatch(typeof(TournamentManager), "ResolveTournament")]
    public class TournamentManagerPatchResolveTournament1
    {
        public static void Prefex(TournamentGame tournament)
        {
            TournamentPrizeExpansion.ClearTournamentPrizes(tournament.Town.Settlement.StringId);
        }
        static bool Prepare()
        {
            return (BMTournamentPrizeConfiguration.Instance.EnableConfigReloadRealTime || BMTournamentPrizeConfiguration.Instance.EnablePrizeSelection);
        }
    }
}
