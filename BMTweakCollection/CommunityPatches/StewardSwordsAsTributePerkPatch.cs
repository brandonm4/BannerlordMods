using System;
using System.Linq;
using System.Reflection;
using BMTweakCollection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class StewardSwordsAsTributePatch : PatchBase<StewardSwordsAsTributePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySizeLimitModel).GetMethod("CalculateMobilePartyMemberSizeLimit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardSwordsAsTributePatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "7fHHThQr");

    public override void Apply(Game game) {
      if (Applied) return;

      BMTweakCollectionSubModule._harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

            return true;
    }

    // ReSharper disable once InconsistentNaming
    private static void Postfix(ref int __result, MobileParty party, StatExplainer explanation) {
      var perk = ActivePatch._perk;
      var hero = party.LeaderHero;
      if (hero == null)
        return;
      if (!(hero?.GetPerkValue(perk) ?? false))
        return;

      var kingdomClans = hero.Clan?.Kingdom?.Clans;

      if (kingdomClans == null)
        return;

      var extra = (int) Math.Max(0, (kingdomClans.Count() - 1) * perk.PrimaryBonus);

      if (extra <= 0)
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      explainedNumber.Add(extra, perk.Name);
      __result = (int) explainedNumber.ResultNumber;
    }

  }

}