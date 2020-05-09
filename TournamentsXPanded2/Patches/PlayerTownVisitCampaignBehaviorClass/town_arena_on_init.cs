using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.Towns;
using TaleWorlds.Core;
using TaleWorlds.Localization;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.PlayerTownVisitCampaignBehaviorClass
{
    internal class town_arena_on_init : PatchBase<town_arena_on_init>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(PlayerTownVisitCampaignBehavior).GetMethod("town_arena_on_init", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfoP = typeof(town_arena_on_init).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfoT = typeof(town_arena_on_init).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return TournamentXPSettings.Instance.EnableItemModifiersForPrizes;
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
                  prefix: new HarmonyMethod(PatchMethodInfoP)
              //  ,transpiler:new HarmonyMethod(PatchMethodInfoT)
              );

            Applied = true;
        }

        public override void Reset()
        {
        }

        //private static bool Prefix(PlayerTownVisitCampaignBehavior __instance, MenuCallbackArgs args)
        //{
        //    //Move name generation to top of code block
        //    TextObject name = new TextObject("");
        //    if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.HasTournament)
        //    {
        //        var tournamentInfo = TournamentsXPandedBehavior.GetTournamentInfo(Settlement.CurrentSettlement.Town);
        //        name = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town).Prize.Name;

        //        if (!string.IsNullOrWhiteSpace(tournamentInfo.SelectedPrizeStringId))
        //        {
        //            name = tournamentInfo.SelectedPrizeItem.ToItemRosterElement().EquipmentElement.GetModifiedItemName();
        //        }
        //    }
        //    return true;
        //}
        private static bool Prefix(PlayerTownVisitCampaignBehavior __instance, MenuCallbackArgs args)
        {
            //PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
            //Traverse.Create(__instance).Method("UpdateMenuLocations").GetValue(new object[] { args.MenuContext.GameMenu.StringId });

            typeof(PlayerTownVisitCampaignBehavior).GetMethod("UpdateMenuLocations", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly).Invoke(__instance, new object[] { args.MenuContext.GameMenu.StringId });

            if (Settlement.CurrentSettlement == null || !Settlement.CurrentSettlement.IsTown || Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town) == null || !Campaign.Current.IsDay)
            {
                MBTextManager.SetTextVariable("ADDITIONAL_STRING", GameTexts.FindText("str_town_empty_arena_text", null), false);
            }
            else
            {
                TextObject name = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town).Prize.Name;

                //Need to turn this into a transpiler instead
                var tournamentInfo = TournamentsXPandedBehavior.GetTournamentInfo(Settlement.CurrentSettlement.Town);
                if (!string.IsNullOrWhiteSpace(tournamentInfo.SelectedPrizeStringId))
                {
                    name = tournamentInfo.SelectedPrizeItem.ToItemRosterElement().EquipmentElement.GetModifiedItemName();
                }

                MBTextManager.SetTextVariable("ITEM", name, false);
                MBTextManager.SetTextVariable("ADDITIONAL_STRING", GameTexts.FindText("str_town_new_tournament_text", null), false);
            }

            //var checknext = (bool)Traverse.Create(__instance).Method("CheckAndOpenNextLocation").GetValue(new object[] { args });
            var checknext = (bool)typeof(PlayerTownVisitCampaignBehavior).GetMethod("CheckAndOpenNextLocation", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly).Invoke(__instance, new object[] { args });
            //if (PlayerTownVisitCampaignBehavior.CheckAndOpenNextLocation(args))
            if (checknext)
            {
                return false;
            }
            args.MenuTitle = new TextObject("{=mMU3H6HZ}Arena", null);
            return false;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //Chop out the existing TextObject name and use the new one.
            var codes = instructions.ToList();
            var opCount = 0;
            var targetNode = 0;
            foreach (var ci in instructions)
            {
                if (ci.operand as string == "ITEM")
                {
                    targetNode = opCount - 2;
                    break;
                }
                opCount++;
            }
            var rem = codes[targetNode];
            codes.Remove(rem);
            return codes.AsEnumerable();
        }
    }
}