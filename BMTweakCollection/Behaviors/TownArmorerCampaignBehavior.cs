using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TownArmorerOrderMod.Utils;

namespace TownArmorerOrderMod
{
    public class TownArmorerCampaignBehavior : CampaignBehaviorBase
    {
        private ItemObjectTypeTuple selectedType;
        private ItemObjectTierTuple selectedTier;
        private ItemObject selectedItem;
        private int goldPaidInAdvance;
        private Dictionary<Settlement, QuestBase> currentQuests;

        public override void RegisterEvents()
        {
            //CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            currentQuests = new Dictionary<Settlement, QuestBase>();
            starter.AddPlayerLine("armor_craft_player_line", "weaponsmith_talk_player", "armor_craft_request", "Can you craft me an armor ?", new ConversationSentence.OnConditionDelegate(ArmorSmithDialogCondition), null);
            //add armor types dialogs
            starter.AddDialogLine("armor_craft_response_1", "armor_craft_request", "armor_type_choice", "What kind of armor ?", null, new ConversationSentence.OnConsequenceDelegate(ArmorSmithDialogOnTypeChoiceConsequence), 100, null);
            starter.AddRepeatablePlayerLine("armor_craft_type_choice", "armor_type_choice", "armor_tier_choice", "{TYPE}", new ConversationSentence.OnConditionDelegate(ArmorSmithDialogOnTypeChoiceCondition), new ConversationSentence.OnConsequenceDelegate(ArmorSmithDialogOnTypeChoiceUpdateSelected));
            starter.AddPlayerLine("armor_craft_type_choice_return", "armor_type_choice", "weaponsmith_begin", "Nevermind", null, null);

            //add armor tier dialogs
            starter.AddDialogLine("armor_craft_tier_choice", "armor_tier_choice", "armor_tier_choice_repeat", "So you want {TYPE}? Fine. Help me narrow it down, which armor tier are you going for ?", new ConversationSentence.OnConditionDelegate(ArmorSmithDialogOnTierChoiceDialogCondition), new ConversationSentence.OnConsequenceDelegate(ArmorSmithDialogOnTierChoiceConsequence));
            starter.AddRepeatablePlayerLine("armor_craft_tier_choice", "armor_tier_choice_repeat", "armor_item_choice", "{TIER}", new ConversationSentence.OnConditionDelegate(ArmorSmithDialogOnTierchoiceCondition), new ConversationSentence.OnConsequenceDelegate(ArmorSmithDialogOnTierChoiceUpdateSelected));
            starter.AddPlayerLine("armor_craft_tier_choice_return", "armor_tier_choice_repeat", "weaponsmith_begin", "Nevermind", null, null);

            //add armor item dialogs
            starter.AddDialogLine("armor_craft_item_choice", "armor_item_choice", "armor_item_choice_repeat", "So that will be {TYPEANDTIER}. What are you looking for in particular ?", new ConversationSentence.OnConditionDelegate(ArmorSmithDialogOnTypeAndTierChoiceCondition), new ConversationSentence.OnConsequenceDelegate(ArmorSmithDialogOnItemChoiceConsequence));
            starter.AddRepeatablePlayerLine("armor_craft_type_choice", "armor_item_choice_repeat", "armor_pay", "{ITEM}", new ConversationSentence.OnConditionDelegate(ArmorSmithDialogOnItemChoiceCondition), new ConversationSentence.OnConsequenceDelegate(ArmorSmithDialogOnItemChoiceUpdateSelected));
            starter.AddPlayerLine("armor_craft_type_choice_return", "armor_item_choice_repeat", "weaponsmith_begin", "Nevermind", null, null);
            //add pay half dialog
            starter.AddDialogLine("armor_craft_pay", "armor_pay", "armor_pay_choice", "{ITEM} it is then, that will cost you {COST}.\nYou will have to pay half of the price now, wait a week for me to finish your piece then come back here to pick it up and pay the rest of the price.", new ConversationSentence.OnConditionDelegate(ArmorSmithDialogOnPayCondition), null);
            starter.AddPlayerLine("armor_craft_pay_response", "armor_pay_choice", "quest_starter_dialog", "Sounds good, here's your money.", null, new ConversationSentence.OnConsequenceDelegate(ArmorSmithDialogOnPay), 100, new ConversationSentence.OnClickableConditionDelegate(ArmorSmithDialogCanPayCondition));
            starter.AddPlayerLine("armor_craft_pay_response_return", "armor_pay_choice", "weaponsmith_begin", "I changed my mind", null, null);
        }

        private void ArmorSmithDialogOnPay()
        {
            if (currentQuests.ContainsKey(Settlement.CurrentSettlement))
            {
                currentQuests.Remove(Settlement.CurrentSettlement);
            }
            currentQuests.Add(Settlement.CurrentSettlement, new ArmorCraftingQuest($"armor_crafting_quest_id_{Settlement.CurrentSettlement.Name}_{CampaignTime.Now}", Hero.MainHero, Settlement.CurrentSettlement, CampaignTime.DaysFromNow(38f), selectedItem, goldPaidInAdvance));
        }

        private bool ArmorSmithDialogCanPayCondition(out TextObject explanation)
        {
            bool condition = Campaign.Current.MainParty.Leader.HeroObject.Gold >= goldPaidInAdvance;
            if (!condition)
            {
                explanation = new TextObject("You don't have enough denars !");
            }
            else
            {
                explanation = new TextObject("");
            }
            return condition;
        }

        private void ArmorSmithDialogOnItemChoiceUpdateSelected()
        {
            selectedItem = ConversationSentence.LastSelectedRepeatObject as ItemObject;
        }

        private bool ArmorSmithDialogOnPayCondition()
        {
            MBTextManager.SetTextVariable("ITEM", selectedItem.Name);
            MBTextManager.SetTextVariable("COST", selectedItem.Value);
            goldPaidInAdvance = selectedItem.Value / 2;
            return true;
        }

        private void ArmorSmithDialogOnTierChoiceUpdateSelected()
        {
            selectedTier = ConversationSentence.LastSelectedRepeatObject as ItemObjectTierTuple;
        }

        private void ArmorSmithDialogOnTypeChoiceUpdateSelected()
        {
            selectedType = ConversationSentence.LastSelectedRepeatObject as ItemObjectTypeTuple;
        }

        private bool ArmorSmithDialogOnTierChoiceDialogCondition()
        {
            MBTextManager.SetTextVariable("TYPE", selectedType.Article + " " + selectedType.Name);
            return true;
        }

        private bool ArmorSmithDialogOnTypeAndTierChoiceCondition()
        {
            MBTextManager.SetTextVariable("TYPEANDTIER", "a " + selectedTier.ToString() + " " + selectedType.Name);
            return true;
        }

        private bool ArmorSmithDialogOnTierchoiceCondition()
        {
            ConversationSentence.SelectedRepeatLine.SetTextVariable("TIER", ConversationSentence.SelectedRepeatObject.ToString());
            return true;
        }
        private void ArmorSmithDialogOnTierChoiceConsequence()
        {
            List<ItemObjectTierTuple> tierList = new List<ItemObjectTierTuple>();
            tierList.Add(new ItemObjectTierTuple("tier 1", ItemObject.ItemTiers.Tier1));
            tierList.Add(new ItemObjectTierTuple("tier 2", ItemObject.ItemTiers.Tier2));
            tierList.Add(new ItemObjectTierTuple("tier 3", ItemObject.ItemTiers.Tier3));
            tierList.Add(new ItemObjectTierTuple("tier 4", ItemObject.ItemTiers.Tier4));
            tierList.Add(new ItemObjectTierTuple("tier 5", ItemObject.ItemTiers.Tier5));
            tierList.Add(new ItemObjectTierTuple("tier 6", ItemObject.ItemTiers.Tier6));
            ConversationSentence.ObjectsToRepeatOver = tierList;
        }

        private bool ArmorSmithDialogOnItemChoiceCondition()
        {
            if (ConversationSentence.SelectedRepeatObject is ItemObject)
            {
                ConversationSentence.SelectedRepeatLine.SetTextVariable("ITEM", (ConversationSentence.SelectedRepeatObject as ItemObject).Name);
                return true;
            }
            return false;
        }

        private void ArmorSmithDialogOnItemChoiceConsequence()
        {
            List<ItemObject> itemList = new List<ItemObject>();
            List<CultureObject> cultureList = new List<CultureObject>();
            Game.Current.ObjectManager.GetAllInstancesOfObjectType<ItemObject>(ref itemList);
            Game.Current.ObjectManager.GetAllInstancesOfObjectType<CultureObject>(ref cultureList);
            ItemObject[] iteratingList = new ItemObject[itemList.Count];
            itemList.CopyTo(iteratingList);
            itemList = new List<ItemObject>();
            foreach (ItemObject item in iteratingList)
            {
                if (item.Type == selectedType.Type && (item.Culture == Settlement.CurrentSettlement.Culture  || item.Culture.GetCultureCode() == CultureCode.AnyOtherCulture) && item.Tier == selectedTier.Tier)
                {
                    itemList.Add(item);
                }
            }
            ConversationSentence.ObjectsToRepeatOver = itemList;
        }

        private bool ArmorSmithDialogOnTypeChoiceCondition()
        {
            ConversationSentence.SelectedRepeatLine.SetTextVariable("TYPE", (ConversationSentence.SelectedRepeatObject as ItemObjectTypeTuple).Article + " " + (ConversationSentence.SelectedRepeatObject as ItemObjectTypeTuple).Name);
            return true;
        }

        private void ArmorSmithDialogOnTypeChoiceConsequence()
        {
            List<ItemObjectTypeTuple> itemTypes = new List<ItemObjectTypeTuple>();
            itemTypes.Add(new ItemObjectTypeTuple("an", "armor", ItemObject.ItemTypeEnum.BodyArmor));
            itemTypes.Add(new ItemObjectTypeTuple("a", "helmet", ItemObject.ItemTypeEnum.HeadArmor));
            itemTypes.Add(new ItemObjectTypeTuple("a", "cape", ItemObject.ItemTypeEnum.Cape));
            itemTypes.Add(new ItemObjectTypeTuple("a", "pair of boots", ItemObject.ItemTypeEnum.LegArmor));
            itemTypes.Add(new ItemObjectTypeTuple("a", "pair of gauntlets", ItemObject.ItemTypeEnum.HandArmor));
            ConversationSentence.ObjectsToRepeatOver = itemTypes;
        }

        private bool ArmorSmithDialogCondition()
        {
            return CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Armorer && (!currentQuests.ContainsKey(Settlement.CurrentSettlement) || currentQuests[Settlement.CurrentSettlement].IsFinalized);
        }

        internal class ArmorCraftingQuest : QuestBase
        {
            private Settlement settlement;
            private ItemObject craftedObject;
            private JournalLog sumOwedObjective;
            private JournalLog timeSpentObjective;
            private DialogFlow startDialog;
            private DialogFlow endDialog;


            private TextObject title;

            private int daysSinceCraftingStarted;

            private int daysToCraft;

            private bool isInitialized;

            private bool hasWaitedLongEnough;

            private int amountPaidInAdvance;

            private MBGUID objectId;

            private MBGUID settlementId;


            public ArmorCraftingQuest(string questId, Hero questGiver, Settlement settlement, CampaignTime duration, ItemObject craftedObject, int amountPaidInAdvance) : base(questId, questGiver, duration, 0)
            {
                this.craftedObject = craftedObject;
                this.amountPaidInAdvance = amountPaidInAdvance;
                isInitialized = false;
                this.settlement = settlement;
                settlementId = settlement.Id;
                objectId = craftedObject.Id;
                SetDialogs();
                Campaign.Current.ConversationManager.AddDialogFlow(startDialog, this);
                Campaign.Current.ConversationManager.AddDialogFlow(endDialog, this);
                CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
                CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.onDailyTick));
                CampaignEvents.HeroOrPartyTradedGold.AddNonSerializedListener(this, new Action<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool>(this.HeroOrPartyTradedGold));
                daysSinceCraftingStarted = 0;
                daysToCraft = 8;
                title = new TextObject($"Waiting for your {craftedObject.Name} to be crafted at {settlement.Name}");
                hasWaitedLongEnough = false;
            }

            private void HeroOrPartyTradedGold((Hero, PartyBase) arg1, (Hero, PartyBase) arg2, (int, string) arg3, bool arg4)
            {
                if (arg1.Item1 != null && arg1.Item1.Name == Hero.MainHero.Name)
                {
                    sumOwedObjective.UpdateCurrentProgress(Math.Min(Campaign.Current.MainParty.Leader.HeroObject.Gold, craftedObject.Value - amountPaidInAdvance));
                }
            }

            private void onDailyTick()
            {
                daysSinceCraftingStarted++;
                timeSpentObjective.UpdateCurrentProgress(Math.Min(daysSinceCraftingStarted, daysToCraft));
                if (timeSpentObjective.HasBeenCompleted() && !hasWaitedLongEnough)
                {
                    base.AddLog(new TextObject($"Your {craftedObject.Name} should be ready, go talk to the armorer at {settlement.Name} with the rest of the sum to get it"));
                    hasWaitedLongEnough = true;
                }
            }

            private void OnHourlyTick()
            {
                sumOwedObjective.UpdateCurrentProgress(Math.Min(Campaign.Current.MainParty.Leader.HeroObject.Gold, craftedObject.Value - amountPaidInAdvance));
            }

            public override TextObject Title => title;

            public override bool IsRemainingTimeHidden => false;

            protected override void InitializeQuestOnGameLoad()
            {
                LoadQuestData();
                this.SetDialogs();
                Campaign.Current.ConversationManager.AddDialogFlow(startDialog, this);
                Campaign.Current.ConversationManager.AddDialogFlow(endDialog, this);
                CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
                CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.onDailyTick));
                CampaignEvents.HeroOrPartyTradedGold.AddNonSerializedListener(this, new Action<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool>(this.HeroOrPartyTradedGold));
            }

            private void LoadQuestData()
            {
                sumOwedObjective = base.JournalEntries[0];
                timeSpentObjective = base.JournalEntries[1];
                craftedObject = Campaign.Current.ObjectManager.GetObject(objectId) as ItemObject;
                settlement = Campaign.Current.ObjectManager.GetObject(settlementId) as Settlement;
            }

            protected override void SetDialogs()
            {
                startDialog = DialogFlow.CreateDialogFlow("quest_starter_dialog").NpcLine("Thank you. Your armor should be done in about a week.\nYou'll have a month to bring me the rest of the sum you owe, otherwise I'll be forced to sell your armor to someone else").Condition(new ConversationSentence.OnConditionDelegate(CheckIdCondition)).Consequence(new ConversationSentence.OnConsequenceDelegate(QuestStartConsequence)).CloseDialog();
                endDialog = DialogFlow.CreateDialogFlow("weaponsmith_talk_player", 200).PlayerLine("I came for my order").Condition(new ConversationSentence.OnConditionDelegate(CheckSettlementCondition)).BeginNpcOptions().NpcOption($"Your {craftedObject.Name.ToLower()} is ready. I need you to pay the rest of the sum, {craftedObject.Value - amountPaidInAdvance}", new ConversationSentence.OnConditionDelegate(OrderIsFinishedConditon)).BeginPlayerOptions().PlayerOption("Sure, here's the sum I owe you").ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(PlayerHasEnoughMoneyCondition)).NpcLine("Great! May it serves you well in your journeys.").Consequence(new ConversationSentence.OnConsequenceDelegate(PlayerPaidForItemConsequence)).CloseDialog()
                    .PlayerOption("Not Yet").NpcLine("Hurry up then, or I will have to sell it to someone else").CloseDialog().EndPlayerOptions()
                    .NpcOption("I'm still working on it, come back in a few days", new ConversationSentence.OnConditionDelegate(OrderIsNotFinishedConditon)).CloseDialog().EndNpcOptions();
            }

            private bool CheckIdCondition()
            {
                return !isInitialized;
            }

            private bool CheckSettlementCondition()
            {
                return Settlement.CurrentSettlement == settlement && !this.IsFinalized;
            }

            private void PlayerPaidForItemConsequence()
            {
                base.AddLog(new TextObject($"You successfully completed the transaction"));
                Campaign.Current.MainParty.Leader.HeroObject.ChangeHeroGold(-(craftedObject.Value - amountPaidInAdvance));
                PlayerPaidNotification(craftedObject.Value - amountPaidInAdvance);
                Campaign.Current.MainParty.ItemRoster.Add(new ItemRosterElement(craftedObject, 1));
                InformationManager.DisplayMessage(new InformationMessage($"{craftedObject.Name} added to {Campaign.Current.MainParty.Leader.HeroObject.Name}'s inventory"));
                base.CompleteQuestWithSuccess();
            }

            private void PlayerPaidNotification(int amount)
            {
                TextObject textObject = new TextObject("You paid {CHANGE}{GOLD_ICON}", null);
                textObject.SetTextVariable("CHANGE", amount);
                textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"Icons\\Coin@2x\">");
                string soundEventPath = "event:/ui/notification/coins_negative";
                InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), soundEventPath));
            }

            private bool PlayerHasEnoughMoneyCondition(out TextObject explanation)
            {
                bool condition = Campaign.Current.MainParty.Leader.HeroObject.Gold >= craftedObject.Value - amountPaidInAdvance;
                if (!condition)
                {
                    explanation = new TextObject("You don't have enough money!");
                }
                else
                {
                    explanation = new TextObject("");
                }
                return condition;
            }

            private bool OrderIsNotFinishedConditon()
            {
                return !timeSpentObjective.HasBeenCompleted();
            }

            private bool OrderIsFinishedConditon()
            {
                return timeSpentObjective.HasBeenCompleted();
            }

            private void QuestStartConsequence()
            {
                base.StartQuest();
                isInitialized = true;
                Campaign.Current.MainParty.Leader.HeroObject.ChangeHeroGold(-amountPaidInAdvance);
                PlayerPaidNotification(craftedObject.Value - amountPaidInAdvance);
                this.sumOwedObjective = base.AddDiscreteLog(new TextObject($"You have paid {amountPaidInAdvance} to a blacksmith in {settlement.Name} to have a piece of armor crafted.\nYou owe him {craftedObject.Value - amountPaidInAdvance}"),
                    new TextObject("Money owed to the armorer", null), Math.Min(Campaign.Current.MainParty.Leader.HeroObject.Gold, craftedObject.Value - amountPaidInAdvance), craftedObject.Value - amountPaidInAdvance, null, true);
                timeSpentObjective = base.AddDiscreteLog(new TextObject($"He said it will take him a week to make your {craftedObject.Name.ToLower()}, and that you have up to a month to come back to {settlement.Name} to pick it up"),
                    new TextObject("Time remaining", null), 0, daysToCraft, null, true);
            }
        }

       

        public override void SyncData(IDataStore dataStore)
        {

        }
    }
}
