using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TournamentLib.Models;

namespace BMTournamentXP.Models
{
    public class TournamentCombatXpModel : CombatXpModel
    {
        public override float CaptainRadius
        {
            get
            {
                return 10f;
            }
        }   
        public override SkillObject GetSkillForWeapon(ItemObject item, int weaponUsageIndex)
        {
            SkillObject athletics = null;
            athletics = DefaultSkills.Athletics;
            if (item != null)
            {
                athletics = item.GetWeaponWithUsageIndex(weaponUsageIndex).RelevantSkill;
            }
            return athletics;
        }

        public override void GetXpFromHit(CharacterObject attackerTroop, CharacterObject attackedTroop, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType, out int xpAmount)
        {
            int num = attackedTroop.MaxHitPoints();
            xpAmount = MBMath.Round(0.4f * ((attackedTroop.GetPower() + 0.5f) * (float)(Math.Min(damage, num) + (isFatal ? num : 0))));
            if (missionType == CombatXpModel.MissionTypeEnum.SimulationBattle)
            {
                xpAmount *= 8;
            }
            if (missionType == CombatXpModel.MissionTypeEnum.PracticeFight)
            {
                xpAmount = MathF.Round((float)xpAmount * TournamentConfiguration.Instance.XPConfiguration.ArenaXPAdjustment);
            }
            if (missionType == CombatXpModel.MissionTypeEnum.Tournament)
            {
                xpAmount = MathF.Round((float)xpAmount * TournamentConfiguration.Instance.XPConfiguration.TournamentXPAdjustment);
            }
        }

        public override float GetXpMultiplierFromShotDifficulty(float shotDifficulty)
        {
            if (shotDifficulty > 14.4f)
            {
                shotDifficulty = 14.4f;
            }
            return MBMath.Lerp(0f, 2f, (shotDifficulty - 1f) / 13.4f, 1E-05f);
        }
    }
}