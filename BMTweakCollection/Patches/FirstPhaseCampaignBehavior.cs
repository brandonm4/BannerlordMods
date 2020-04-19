using HarmonyLib;
using StoryMode.Behaviors;
using StoryMode.StoryModePhases;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(FirstPhaseCampaignBehavior), "WeeklyTick")]
    public static class FirstPhaseCampaignBehaviorTimeLimitPatch
    {
        public static bool Prefix(FirstPhaseCampaignBehavior __instance)
        {
            if (FirstPhase.Instance != null && SecondPhase.Instance == null && FirstPhase.Instance.FirstPhaseStartTime.ElapsedYearsUntilNow > 99999f)
            {
                foreach (QuestBase list in Campaign.Current.QuestManager.Quests.ToList<QuestBase>())
                {
                    if (!list.IsSpecialQuest)
                    {
                        continue;
                    }
                    TextObject textObject = new TextObject("{=JTPmw3cb}You couldn't complete the quest in {YEAR} years.", null);
                    textObject.SetTextVariable("YEAR", 10);
                    list.CompleteQuestWithFail(textObject);
                }
            }
            return false;
        }
        private static bool Prepare()
        {
            return true;
        }
    }
}
