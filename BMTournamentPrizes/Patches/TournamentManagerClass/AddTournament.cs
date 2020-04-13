using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Models;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using System.Reflection;
using TaleWorlds.Core;

namespace TournamentsXPanded.Patches.TournamentManagerClass
{
    class AddTournament : PatchBase<AddTournament>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentManager).GetMethod("AddTournament", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(AddTournament).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return (TournamentXPSettings.Instance.EnablePrizeSelection);
        }
        public override void Reset() { }

        public override void Apply(Game game)
        {
            if (Applied) return;
            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
                  prefix: new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }
    
        static void Prefix(TournamentGame game)
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
     
    }
}
