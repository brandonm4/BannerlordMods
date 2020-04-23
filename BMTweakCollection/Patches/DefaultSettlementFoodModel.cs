using HarmonyLib;

using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TournamentsXPanded.Common.Patches;

namespace BMTweakCollection.Patches.DefaultSettlementFoodModelClass
{

    public class CalculateTownFoodStocksChange : PatchBase<CalculateTownFoodStocksChange>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSettlementFoodModel).GetMethod("CalculateTownFoodStocksChange", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(CalculateTownFoodStocksChange).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        public override bool IsApplicable(Game game)
        {
            return true;
        }
        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            BMTweakCollectionSubModule.Harmony.Patch(TargetMethodInfo,
              postfix: new HarmonyMethod(PatchMethodInfo)
              {
                  priority = Priority.First,
                  //before = new[] { "that.other.harmony.user" }
              }
              );

            Applied = true;
        }

        public override void Reset()
        {
        }
        static void Postfix(ref float __result, ref Town town, ref StatExplainer explanation)
        {
            if (!(town.Settlement.SiegeEvent != null && town.IsUnderSiege &&
                town.Settlement.SiegeEvent.BesiegerCamp.SiegeParties.Any<PartyBase>((PartyBase party) => party.MobileParty == Hero.MainHero.PartyBelongedTo)))
            {
                ExplainedNumber en = new ExplainedNumber(__result, explanation);
                explanation?.Lines.Remove(explanation.Lines.Last());

                if (town.IsCastle)
                {
                    en.Add(2.0f, new TextObject("Military rations"));
                }
                else if (town.IsTown)
                {
                    en.Add(4.0f, new TextObject("Citizen food drive"));
                }

                __result = en.ResultNumber;
            }
        }
    }

    public class CalculateDailyFoodConsumptionf : PatchBase<CalculateDailyFoodConsumptionf>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultMobilePartyFoodConsumptionModel).GetMethod("CalculateDailyFoodConsumptionf", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(CalculateDailyFoodConsumptionf).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        public override bool IsApplicable(Game game)
        {
            return true;
        }
        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            BMTweakCollectionSubModule.Harmony.Patch(TargetMethodInfo,
              postfix: new HarmonyMethod(PatchMethodInfo)
              {
                  priority = Priority.First,
                  //before = new[] { "that.other.harmony.user" }
              }
              );

            Applied = true;
        }

        public override void Reset()
        {
        }
        static void Postfix(ref float __result, MobileParty party, StatExplainer explainer)
        {
            //For now only do hero
            if (party.LeaderHero == Hero.MainHero)
            {
                if (party.Scout != null)
                {
                    var sk = party.Scout.GetSkillValue(DefaultSkills.Scouting);
                    if (sk > 0)
                    {
                        var mod = 1.0f - (sk / 300.0f);
                        if (mod < 0.1f)
                        {
                            mod = 0.1f;
                        }

                        mod = MBRandom.RandomFloatRanged(mod, 1.0f);
                        var orig = __result;
                        var deduction = (orig - ((__result * mod))) * -1f;

                        ExplainedNumber en = new ExplainedNumber(__result, explainer);
                        explainer?.Lines.Remove(explainer.Lines.Last());
                        var textObject = new TextObject("{FIRST_NAME} scouted for food.", null);
                        textObject.SetTextVariable("FIRST_NAME", party.Scout.FirstName);
                        //textObject.SetTextVariable("REDUCTION_AMOUNT", (orig-__result).ToString());
                        en.Add(deduction, textObject);
                        __result = en.ResultNumber;
                    }
                }
            }
        }
    }

}