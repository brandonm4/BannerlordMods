using System.Collections.Generic;

using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace XPanded.Common.Extensions
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
                if (IsWeaponCouchable(item))
                {
                    desc += "Couchable";
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

        internal static bool IsWeaponCouchable(ItemObject weapon)
        {
            bool flag = false;
            using (IEnumerator<WeaponComponentData> enumerator = weapon.Weapons.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!MBItem.GetItemIsPassiveUsage(enumerator.Current.ItemUsage))
                    {
                        continue;
                    }
                    flag = true;
                    break;
                }         
            }
            return flag;
        }

        public static TextObject ToToolTipTextObject(this EquipmentElement equipmentElement)
        {
            string mountdesc = "Speed: {SPEED}\nManeuver: {MANEUVER}\nHealth: {HEALTH}\n";

            string desc = "{NAME}\n";
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues.Add("NAME", equipmentElement.GetModifiedItemName().ToString());

            if (equipmentElement.Item.IsCraftedWeapon)
            {
                desc += "{WEAPON_TYPE}\n";
                keyValues.Add("WEAPON_TYPE", equipmentElement.Item.PrimaryWeapon.WeaponClass.ToString());
                if (equipmentElement.Item.PrimaryWeapon.SwingSpeed > 0)
                {
                    desc += "Swing Speed: {SWING_SPEED}\n";
                    keyValues.Add("SWING_SPEED", equipmentElement.Item.WeaponComponent.PrimaryWeapon.SwingSpeed.ToString());
                }
                if (equipmentElement.Item.PrimaryWeapon.SwingSpeed > 0)
                {
                    desc += "Swing Damage: {SWING_DAMAGE}\n";
                    keyValues.Add("SWING_DAMAGE", equipmentElement.Item.WeaponComponent.PrimaryWeapon.SwingDamage.ToString() + equipmentElement.Item.PrimaryWeapon.SwingDamageType.ToString().Substring(0, 1));
                }
                if (equipmentElement.Item.PrimaryWeapon.ThrustSpeed > 0)
                {
                    desc += "Thrust Speed: {THRUST_SPEED}\n";
                    keyValues.Add("THRUST_SPEED", equipmentElement.Item.WeaponComponent.PrimaryWeapon.ThrustSpeed.ToString());
                }
                if (equipmentElement.Item.PrimaryWeapon.ThrustSpeed > 0)
                {
                    desc += "Thrust Damage: {THRUST_DAMAGE}\n";
                    keyValues.Add("THRUST_DAMAGE", equipmentElement.Item.WeaponComponent.PrimaryWeapon.ThrustDamage.ToString() + equipmentElement.Item.PrimaryWeapon.ThrustDamageType.ToString().Substring(0, 1));
                }
                if (equipmentElement.Item.PrimaryWeapon.MissileSpeed > 0)
                {
                    desc += "Missile Speed: {Missile_SPEED}\n";
                    keyValues.Add("Missile_SPEED", equipmentElement.Item.WeaponComponent.PrimaryWeapon.MissileSpeed.ToString());
                }
                if (equipmentElement.Item.PrimaryWeapon.MissileSpeed > 0)
                {
                    desc += "Missile Damage: {Missile_DAMAGE}\n";
                    keyValues.Add("Missile_DAMAGE", equipmentElement.Item.WeaponComponent.PrimaryWeapon.MissileDamage.ToString());
                }
                if (equipmentElement.Item.PrimaryWeapon.WeaponLength > 0)
                {
                    desc += "Length: {LENGTH}\n";
                    keyValues.Add("LENGTH", equipmentElement.Item.WeaponComponent.PrimaryWeapon.WeaponLength.ToString());
                }
            }
            if (equipmentElement.Item.ArmorComponent != null)
            {
                if (equipmentElement.GetHeadArmor() > 0)
                {
                    desc += "Head Armor: {HEAD_ARMOR}\n";
                    keyValues.Add("HEAD_ARMOR", equipmentElement.GetHeadArmor().ToString());
                }
                if (equipmentElement.GetBodyArmorHuman() > 0)
                {
                    desc += "Body Armor: {BODY_ARMOR}\n";
                    keyValues.Add("BODY_ARMOR", equipmentElement.GetBodyArmorHuman().ToString());
                }
                if (equipmentElement.GetBodyArmorHorse() > 0)
                {
                    desc += "Body Armor: {BODY_ARMOR_HORSE}\n";
                    keyValues.Add("BODY_ARMOR_HORSE", equipmentElement.GetBodyArmorHorse().ToString());
                }
                if (equipmentElement.GetArmArmor() > 0)
                {
                    desc += "Arm Armor: {ARM_ARMOR}\n";
                    keyValues.Add("ARM_ARMOR", equipmentElement.GetArmArmor().ToString());
                }
                if (equipmentElement.GetLegArmor() > 0)
                {
                    desc += "Leg Armor: {LEG_ARMOR}\n";
                    keyValues.Add("LEG_ARMOR", equipmentElement.GetLegArmor().ToString());
                }
            }
            if (equipmentElement.Item.IsMountable)
            {
                desc += mountdesc;
                keyValues.Add("SPEED", equipmentElement.Item.HorseComponent.Speed.ToString());
                keyValues.Add("MANEUVER", equipmentElement.Item.HorseComponent.Maneuver.ToString());
                keyValues.Add("HEALTH", equipmentElement.Item.HorseComponent.HitPoints.ToString());
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