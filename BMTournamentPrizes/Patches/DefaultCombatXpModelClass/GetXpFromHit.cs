using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Library;

using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Patches.DefaultCombatXpModelClass
{
    public class GetXpFromHit : PatchBase<GetXpFromHit>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(DefaultCombatXpModel).GetMethod("GetXpFromHit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo PatchMethodInfo = typeof(GetXpFromHit).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        private static bool Prefix(CharacterObject attackerTroop, CharacterObject attackedTroop, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType, out int xpAmount)
        {
            //            int num = attackedTroop.MaxHitPoints();
            //            xpAmount = MBMath.Round(0.4f * ((attackedTroop.GetPower() + 0.5f) * (float)(Math.Min(damage, num) + (isFatal ? num : 0))));

            //            if (missionType == CombatXpModel.MissionTypeEnum.SimulationBattle)
            //            {
            //#pragma warning disable CS1717 // Assignment made to same variable
            //                xpAmount = xpAmount;
            //#pragma warning restore CS1717 // Assignment made to same variable
            //            }
            //            if (missionType == CombatXpModel.MissionTypeEnum.PracticeFight)
            //            {
            //                xpAmount = MathF.Round((float)xpAmount * TournamentXPSettings.Instance.ArenaXPAdjustment);
            //            }
            //            if (missionType == CombatXpModel.MissionTypeEnum.Tournament)
            //            {
            //                xpAmount = MathF.Round((float)xpAmount * TournamentXPSettings.Instance.TournamentXPAdjustment);
            //            }


            float single;
            int num = attackedTroop.MaxHitPoints();
            float power = 0.4f * ((attackedTroop.GetPower() + 0.5f) * (Math.Min(damage, num) + (isFatal ? num : 0)));
            if (missionType == CombatXpModel.MissionTypeEnum.NoXp)
            {
                single = 0f;
            }
            else if (missionType == CombatXpModel.MissionTypeEnum.PracticeFight)
            {
                single = TournamentXPSettings.Instance.ArenaXPAdjustment;
            }
            else if (missionType == CombatXpModel.MissionTypeEnum.Tournament)
            {
                single = TournamentXPSettings.Instance.TournamentXPAdjustment;
            }
            else if (missionType == CombatXpModel.MissionTypeEnum.SimulationBattle)
            {
                single = 0.9f;
            }
            else
            {
                single = (missionType == CombatXpModel.MissionTypeEnum.Battle ? 1f : 1f);
            }
            xpAmount = MathF.Round(power * single);

            return false;


        }

        public override bool IsApplicable(Game game)
        {
            return (TournamentXPSettings.Instance.IsTournamentXPEnabled || TournamentXPSettings.Instance.IsArenaXPEnabled);
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
              prefix: new HarmonyMethod(PatchMethodInfo)
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
    }
}