using HarmonyLib;

using StoryMode.Behaviors;
using StoryMode.StoryModePhases;

using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

using TournamentsXPanded.Common.Patches;

namespace BMTweakCollection.Patches
{
    public class WeeklyTick : PatchBase<WeeklyTick>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(FirstPhaseCampaignBehavior).GetMethod("WeeklyTick", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(WeeklyTick).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

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

        private static bool Prefix()
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
    }
}