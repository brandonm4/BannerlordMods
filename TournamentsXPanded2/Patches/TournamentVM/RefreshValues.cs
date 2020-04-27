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
                var info = TournamentsXPandedBehavior.GetTournamentInfo(__instance.Tournament.TournamentGame.Town);
                if (!string.IsNullOrWhiteSpace(info.SelectedPrizeStringId))
                {
                    var ire = info.SelectedPrizeItem.ToItemRosterElement();
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