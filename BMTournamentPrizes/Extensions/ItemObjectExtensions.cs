using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BMTournamentPrizes.Extensions
{
    public static class ItemObjectExtensions
    {
        public static TextObject ToToolTipTextObject(this ItemObject item)
        {
            string mountdesc = "Speed: {SPEED}\nManeuver: {MANEUVER}\nHealth: {HEALTH}\n";

            string desc = "{NAME}\n";
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues.Add("NAME", item.Name.ToString());

            if (item.IsCraftedWeapon)
            {
                desc += "{WEAPON_TYPE}\n";
                keyValues.Add("WEAPON_TYPE", item.PrimaryWeapon.WeaponClass.ToString());
                if (item.PrimaryWeapon.SwingSpeed > 0)
                {
                    desc += "Swing Speed: {SWING_SPEED}\n";
                    keyValues.Add("SWING_SPEED", item.WeaponComponent.PrimaryWeapon.SwingSpeed.ToString());
                }
                if (item.PrimaryWeapon.SwingSpeed > 0)
                {
                    desc += "Swing Damage: {SWING_DAMAGE}\n";
                    keyValues.Add("SWING_DAMAGE", item.WeaponComponent.PrimaryWeapon.SwingDamage.ToString() + item.PrimaryWeapon.SwingDamageType.ToString().Substring(0, 1));
                }
                if (item.PrimaryWeapon.ThrustSpeed > 0)
                {
                    desc += "Thrust Speed: {THRUST_SPEED}\n";
                    keyValues.Add("THRUST_SPEED", item.WeaponComponent.PrimaryWeapon.ThrustSpeed.ToString());
                }
                if (item.PrimaryWeapon.ThrustSpeed > 0)
                {
                    desc += "Thrust Damage: {THRUST_DAMAGE}\n";
                    keyValues.Add("THRUST_DAMAGE", item.WeaponComponent.PrimaryWeapon.ThrustDamage.ToString() + item.PrimaryWeapon.ThrustDamageType.ToString().Substring(0, 1));
                }
                if (item.PrimaryWeapon.MissileSpeed > 0)
                {
                    desc += "Missile Speed: {Missile_SPEED}\n";
                    keyValues.Add("Missile_SPEED", item.WeaponComponent.PrimaryWeapon.MissileSpeed.ToString());
                }
                if (item.PrimaryWeapon.MissileSpeed > 0)
                {
                    desc += "Missile Damage: {Missile_DAMAGE}\n";
                    keyValues.Add("Missile_DAMAGE", item.WeaponComponent.PrimaryWeapon.MissileDamage.ToString());
                }
                if (item.PrimaryWeapon.WeaponLength > 0)
                {
                    desc += "Length: {LENGTH}\n";
                    keyValues.Add("LENGTH", item.WeaponComponent.PrimaryWeapon.WeaponLength.ToString());
                }
            }
            if (item.ArmorComponent != null)
            {
                if (item.ArmorComponent.HeadArmor > 0)
                {
                    desc += "Head Armor: {HEAD_ARMOR}\n";
                    keyValues.Add("HEAD_ARMOR", item.ArmorComponent.HeadArmor.ToString());
                }
                if (item.ArmorComponent.BodyArmor > 0)
                {
                    desc += "Body Armor: {BODY_ARMOR}\n";
                    keyValues.Add("BODY_ARMOR", item.ArmorComponent.BodyArmor.ToString());
                }
                if (item.ArmorComponent.ArmArmor > 0)
                {
                    desc += "Arm Armor: {ARM_ARMOR}\n";
                    keyValues.Add("ARM_ARMOR", item.ArmorComponent.ArmArmor.ToString());
                }
                if (item.ArmorComponent.LegArmor > 0)
                {
                    desc += "Leg Armor: {LEG_ARMOR}\n";
                    keyValues.Add("LEG_ARMOR", item.ArmorComponent.LegArmor.ToString());
                }
            }
            if (item.IsMountable)
            {
                desc += mountdesc;
                keyValues.Add("SPEED", item.HorseComponent.Speed.ToString());
                keyValues.Add("MANEUVER", item.HorseComponent.Maneuver.ToString());
                keyValues.Add("HEALTH", item.HorseComponent.HitPoints.ToString());

            }

            TextObject toolTip = new TextObject(desc);
            foreach (var k in keyValues.Keys)
            {
                toolTip.SetTextVariable(k, keyValues[k]);
            }
            return toolTip;
        }
    }
}
