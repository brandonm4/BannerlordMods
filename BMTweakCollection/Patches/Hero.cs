using BMTweakCollection.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(Hero), "AddSkillXp")]
    public class AddSkillXpPatch
    {
        private static double GetMultiplier(Hero hero, int skillLevel, SkillObject skill)
        {
            if ((hero == Hero.MainHero
                 || (hero.PartyBelongedTo != null && hero.PartyBelongedTo == Hero.MainHero.PartyBelongedTo))
                 && BMTweakCollectionSubModule.Configuration.MainPartySkillMods.ContainsKey(skill)
                 )
            {
                return BMTweakCollectionSubModule.Configuration.MainPartySkillMods[skill];
            }

            return Math.Max(1, 0.0315769 * Math.Pow(skillLevel, 1.020743));
        }

        static bool Prefix(Hero __instance, SkillObject skill, int xpAmount)
        {
            try
            {
                HeroDeveloper hd = (HeroDeveloper)(typeof(Hero).GetField("_heroDeveloper", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance));
                if (hd != null)
                {
                    if (xpAmount > 0)
                    {
                        double multiplier = GetMultiplier(__instance, __instance.GetSkillValue(skill), skill);
                        int newXpAmount = (int)Math.Ceiling(xpAmount * multiplier);
                        hd.AddSkillXp(skill, newXpAmount);
                    }
                    else
                        hd.AddSkillXp(skill, xpAmount);
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        static bool Prepare()
        {
            return true;
        }
    }
}
