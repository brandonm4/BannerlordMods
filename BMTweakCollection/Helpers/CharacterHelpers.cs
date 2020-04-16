using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BMTweakCollection.Helpers
{
    public partial class BMHelpers
    {
     public partial  class CharacterHelpers
        {
            public static CharacterObject GetCharacterWithHighestSkill(PartyBase party, SkillObject skill)
            {
                CharacterObject heroObject = null;
                int num = 0;
                for (int i = 0; i < party.MemberRoster.Count; i++)
                {
                    CharacterObject characterAtIndex = party.MemberRoster.GetCharacterAtIndex(i);
                    if (characterAtIndex.IsHero && !characterAtIndex.HeroObject.IsWounded)
                    {
                        int skillValue = characterAtIndex.GetSkillValue(skill);
                        if (skillValue >= num)
                        {
                            num = skillValue;
                            heroObject = characterAtIndex;
                        }
                    }
                }
                return heroObject ?? party.LeaderHero.CharacterObject;
            }
        }
    }
}
