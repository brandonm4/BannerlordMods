using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TournamentsXPanded.Common;
using TournamentsXPanded.Models;

namespace TournamentsXPanded.Extensions
{
    public static class SettingsExtensions
    {
        public static List<ItemObject.ItemTypeEnum> GetActivePrizeTypes(this TournamentXPSettings settings)
        {
            List<ItemObject.ItemTypeEnum> validTypes = new List<ItemObject.ItemTypeEnum>();
            if (settings.EnableItemType_BodyArmor)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.BodyArmor);
            }
            if (settings.EnableItemType_Bow)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Bow);
            }
            if (settings.EnableItemType_Cape)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Cape);
            }
            if (settings.EnableItemType_Crossbow)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Crossbow);
            }
            if (settings.EnableItemType_HandArmor)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.HandArmor);
            }
            if (settings.EnableItemType_HeadArmor)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.HeadArmor);
            }
            if (settings.EnableItemType_Horse)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Horse);
            }
            if (settings.EnableItemType_HorseHarness)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.HorseHarness);
            }
            if (settings.EnableItemType_LegArmor)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.LegArmor);
            }
            if (settings.EnableItemType_OneHandedWeapon)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.OneHandedWeapon);
            }
            if (settings.EnableItemType_Polearm)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Polearm);
            }
            if (settings.EnableItemType_Shield)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Shield);
            }
            if (settings.EnableItemType_Thrown)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Thrown);
            }
            if (settings.EnableItemType_TwoHandedWeapon)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.TwoHandedWeapon);
            }
            if (settings.EnableItemType_Arrow)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Arrows);
            }
            if (settings.EnableItemType_Bolt)
            {
                validTypes.Add(ItemObject.ItemTypeEnum.Bolts);
            }
            return validTypes;
        }
        public static float GetRenownValue(this TournamentXPSettings settings, CharacterObject character)
        {
            var worth = 0f;
            if (character.IsHero)
            {
                worth += settings.RenownPerHeroProperty[(int)RenownHeroTier.HeroBase];
                var hero = character.HeroObject;
                if (hero != null)
                {
                    if (hero.IsNoble)
                    {
                        worth += settings.RenownPerHeroProperty[(int)RenownHeroTier.IsNoble];
                    }
                    if (hero.IsNotable)
                    {
                        worth += settings.RenownPerHeroProperty[(int)RenownHeroTier.IsNotable];
                    }
                    if (hero.IsCommander)
                    {
                        worth += settings.RenownPerHeroProperty[(int)RenownHeroTier.IsCommander];
                    }
                    if (hero.IsMinorFactionHero)
                    {
                        worth += settings.RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                    }
                    if (hero.IsFactionLeader)
                    {
                        if (hero.MapFaction.IsKingdomFaction)
                        {
                            worth += settings.RenownPerHeroProperty[(int)RenownHeroTier.IsMajorFactionLeader];
                        }

                        if (hero.MapFaction.IsMinorFaction)
                        {
                            worth += settings.RenownPerHeroProperty[(int)RenownHeroTier.IsMinorFactionHero];
                        }
                    }
                }
            }
            else
            {
                worth += settings.RenownPerTroopTier[character.Tier];
            }
            return worth;
        }
    }
}
