using HarmonyLib;

using SandBox.ViewModelCollection.Tournament;

using System;
using System.Linq;
using System.Reflection;

using TaleWorlds.Core;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.TournamentVMClass
{
    public class RefreshValues : PatchBase<RefreshValues>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentVM).GetMethod("RefreshValues", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(RefreshValues).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return TournamentXPSettings.Instance.EnableTournamentRandomSelection;
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
                  postfix: new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }

        public override void Reset()
        {
        }

        private static void Postfix(TournamentVM __instance)
        {
            try
            {
                var currentPool = TournamentPrizePoolBehavior.GetTournamentPrizePool(__instance.Tournament.TournamentGame.Town.Settlement);
                if (!string.IsNullOrWhiteSpace(currentPool.SelectedPrizeStringId))
                {
                    var ire = currentPool.Prizes.Where(x => x.EquipmentElement.Item.StringId == currentPool.SelectedPrizeStringId).First();

                    __instance.PrizeVisual = new ImageIdentifierVM(ire);
                    __instance.PrizeItemName = ire.EquipmentElement.GetModifiedItemName().ToString();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}