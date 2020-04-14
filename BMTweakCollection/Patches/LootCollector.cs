using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BMTweakCollection.Patches
{

    //public static class LootCollectorInfo
    //{
    //public static Type ClassType = Type.GetType("TaleWorlds.CampaignSystem.LootCollector, TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
    //}


    // [HarmonyPatch(Type.GetType("TaleWorlds.CampaignSystem.LootCollector, TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"))]

    // [HarmonyPatch(LootCollectorInfo.ClassType, "GetXpFromHit")]
    public class LootCollectorPatch
    {
        // make sure DoPatching() is called at start either by
        // the mod loader or by your injector
        internal static Type LootCollectorType;

        public static void DoPatching()
        {
            LootCollectorType = Type.GetType("TaleWorlds.CampaignSystem.LootCollector, TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            var mOriginal = AccessTools.Method(LootCollectorType, "GiveShareOfLootToParty");
            var prefix = typeof(LootCollectorPatch).GetMethod("GiveShareOfLootToPartyPre");

            MethodInfo TargetMethodInfo = LootCollectorType.GetMethod("GiveShareOfLootToParty", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            MethodInfo PatchMethodInfo = typeof(LootCollectorPatch).GetMethod(nameof(GiveShareOfLootToPartyPre), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

            try
            {
                //  var _harmony = new Harmony("com.darkspyre.bannerlord.tweakcol2");
                BMTweakCollectionSubModule.Harmony.Patch(TargetMethodInfo,
                      prefix: new HarmonyMethod(PatchMethodInfo));
            }
            catch (Exception exception1)
            {
                string message;
                Exception exception = exception1;
                string str = exception.Message;
                Exception innerException = exception.InnerException;
                if (innerException != null)
                {
                    message = innerException.Message;
                }
                else
                {
                    message = null;
                }
                MessageBox.Show(string.Concat("Tournament XP Error patching:\n", str, " \n\n", message));
            }

        }


        internal static bool GiveShareOfLootToPartyPre(ref object __instance, PartyBase partyToReceiveLoot, PartyBase winnerParty, float lootAmount)
        {
            //var ___LootedMembers= Traverse.Create<LootCollectorType>(__instance).Field("LootedMembers").GetValue()
            var ___LootedMembers = LootCollectorType.GetProperty("LootedMembers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) as TroopRoster;
            var ___LootedPrisoners = LootCollectorType.GetProperty("LootedPrisoners", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) as TroopRoster;
            var ___CasualtiesInBattle = LootCollectorType.GetProperty("CasualtiesInBattle", BindingFlags.Public | BindingFlags.Instance).GetValue(__instance) as TroopRoster;
            var ___LootedItems = LootCollectorType.GetProperty("LootedItems", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) as ItemRoster;
            

            bool flag = winnerParty == PartyBase.MainParty;
            List<TroopRosterElement> troopRosterElements = new List<TroopRosterElement>();
            foreach (TroopRosterElement lootedMember in ___LootedMembers)
            {
                int number = lootedMember.Number;
                CharacterObject character = lootedMember.Character;
                for (int i = 0; i < number; i++)
                {
                    if (MBRandom.RandomFloat < lootAmount)
                    {
                        TroopRosterElement troopRosterElement = new TroopRosterElement(character)
                        {
                            Number = 1,
                            WoundedNumber = 1
                        };
                        troopRosterElements.Add(troopRosterElement);
                    }
                }
            }
            foreach (TroopRosterElement troopRosterElement1 in troopRosterElements)
            {
                ___LootedMembers.AddToCounts(troopRosterElement1.Character, -1, false, 0, 0, true, -1);
            }
            foreach (TroopRosterElement troopRosterElement2 in troopRosterElements)
            {
                if (!troopRosterElement2.Character.IsHero)
                {
                    partyToReceiveLoot.PrisonRoster.AddToCounts(troopRosterElement2.Character, troopRosterElement2.Number, false, 0, 0, true, -1);
                }
                else if (!partyToReceiveLoot.IsMobile)
                {
                    TakePrisonerAction.Apply(partyToReceiveLoot, troopRosterElement2.Character.HeroObject);
                }
                else
                {
                    TakePrisonerAction.Apply(partyToReceiveLoot, troopRosterElement2.Character.HeroObject);
                }
            }
            ICollection<ItemRosterElement> itemRosterElements = new List<ItemRosterElement>();
            for (int j = ___LootedItems.Count<ItemRosterElement>() - 1; j >= 0; j--)
            {
                ItemRosterElement elementCopyAtIndex = ___LootedItems.GetElementCopyAtIndex(j);
                int num = 0;
                EquipmentElement equipmentElement = elementCopyAtIndex.EquipmentElement;
                ItemObject item = equipmentElement.Item;
                equipmentElement = elementCopyAtIndex.EquipmentElement;
                ItemRosterElement itemRosterElement = new ItemRosterElement(item, 1, equipmentElement.ItemModifier);
                for (int k = 0; k < elementCopyAtIndex.Amount; k++)
                {
                    if (MBRandom.RandomFloat < lootAmount)
                    {
                        itemRosterElements.Add(itemRosterElement);
                        num++;
                    }
                }
                ___LootedItems.AddToCounts(itemRosterElement, -num, true);
            }
            partyToReceiveLoot.ItemRoster.Add(itemRosterElements);
            for (int l = ___LootedPrisoners.Count<TroopRosterElement>() - 1; l >= 0; l--)
            {
                int elementNumber = ___LootedPrisoners.GetElementNumber(l);
                CharacterObject characterAtIndex = ___LootedPrisoners.GetCharacterAtIndex(l);
                int num1 = 0;
                for (int m = 0; m < elementNumber; m++)
                {
                    if (MBRandom.RandomFloat < lootAmount)
                    {
                        partyToReceiveLoot.MemberRoster.AddToCounts(characterAtIndex, 1, false, 0, 0, true, -1);
                        num1++;
                    }
                }
                ___LootedPrisoners.AddToCounts(characterAtIndex, -num1, false, 0, 0, true, -1);
            }
            ICollection<TroopRosterElement> troopRosterElements1 = new List<TroopRosterElement>();
            for (int n = ___CasualtiesInBattle.Count<TroopRosterElement>() - 1; n >= 0; n--)
            {
                int elementNumber1 = ___CasualtiesInBattle.GetElementNumber(n);
                CharacterObject characterObject = ___CasualtiesInBattle.GetCharacterAtIndex(n);
                int num2 = 0;
                TroopRosterElement troopRosterElement3 = new TroopRosterElement(characterObject);
                for (int o = 0; o < elementNumber1; o++)
                {
                    if (MBRandom.RandomFloat < lootAmount)
                    {
                        troopRosterElements1.Add(troopRosterElement3);
                        num2++;
                    }
                }
                ___CasualtiesInBattle.AddToCounts(characterObject, -num2, false, 0, 0, true, -1);
            }
            ExplainedNumber explainedNumber = new ExplainedNumber(1f, null);
            if (winnerParty.MobileParty != null && winnerParty.MobileParty.Leader != null)
            {
                //Get the best looter
                if (winnerParty.MobileParty == MobileParty.MainParty)
                {
                    SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Roguery, DefaultSkillEffects.RogueryLootBonus, GetCharacterWithHighestSkill(winnerParty, DefaultSkills.Roguery), ref explainedNumber, true);
                }
                else
                {
                    SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Roguery, DefaultSkillEffects.RogueryLootBonus, winnerParty.MobileParty.Leader, ref explainedNumber, true);
                }
            }
            if (flag)
            {
                IEnumerable<ItemRosterElement> itemRosterElements1 = LootCasualties(troopRosterElements1, explainedNumber.ResultNumber, flag);
                partyToReceiveLoot.ItemRoster.Add(itemRosterElements1);
            }
            else if (partyToReceiveLoot.LeaderHero != null)
            {
                int gold = ConvertLootToGold(LootCasualties(troopRosterElements1, 0.5f));
                gold = MBMath.Round((float)gold * 0.5f * explainedNumber.ResultNumber);
                GiveGoldAction.ApplyBetweenCharacters(null, partyToReceiveLoot.LeaderHero, gold, false);
                return false;
            }

            return false;
        }

        private static int ConvertLootToGold(IEnumerable<ItemRosterElement> lootedItemsRecoveredFromCasualties)
        {
            int num = 0;
            foreach (ItemRosterElement lootedItemsRecoveredFromCasualty in lootedItemsRecoveredFromCasualties)
            {
                int amount = lootedItemsRecoveredFromCasualty.Amount;
                EquipmentElement equipmentElement = lootedItemsRecoveredFromCasualty.EquipmentElement;
                num = num + amount * MBMath.Round((float)equipmentElement.GetBaseValue() * 0.5f);
            }
            return num;
        }


        private static IEnumerable<ItemRosterElement>LootCasualties2(ICollection<TroopRosterElement> shareFromCasualties, float lootChance)
        {
            // MobileParty.GetMainPartySkillCounsellor(DefaultSkills.Roguery).GetSkillValue(DefaultSkills.Roguery)
            ItemRoster itemRosters = new ItemRoster();
            Dictionary<string, int> loots = new Dictionary<string, int>();
            lootChance = MathF.Clamp(lootChance * 1.3f, 20f, 95f);

            foreach (TroopRosterElement casualty in shareFromCasualties)
            {
                Equipment randomEquipment = GetRandomEquipment(casualty.Character);
                var potentialLootItems = GetItemsFromEquipmentSlots(randomEquipment);
                foreach(ItemObject item in potentialLootItems)
                {
                    float rdm = MBRandom.RandomFloatRanged(100f);
                    if (rdm < lootChance)
                    {
                        if (loots.ContainsKey(item.StringId))
                        {
                            loots[item.StringId] += 1;
                        }
                        else
                        {
                            loots.Add(item.StringId, 1);
                        }
                    }
                }
            }
            foreach(var stringId in loots.Keys)
            {
                itemRosters.Add(new ItemRosterElement(MBObjectManager.Instance.GetObject<ItemObject>(stringId), loots[stringId]));
            }
            return itemRosters;
        }
        private static IEnumerable<ItemRosterElement> LootCasualties(ICollection<TroopRosterElement> shareFromCasualties, float lootFactor, bool playerWin = false)
        {
            EquipmentElement equipmentElement;
            ItemModifier randomModifierWithTarget;
            ItemModifier itemModifier;
            ItemRoster itemRosters = new ItemRoster();
            List<EquipmentElement> equipmentElements = new List<EquipmentElement>();
            foreach (TroopRosterElement shareFromCasualty in shareFromCasualties)
            {
                //for (int i = 0; i < 1; i++)
                {
                    Equipment randomEquipment = GetRandomEquipment(shareFromCasualty.Character);
                    equipmentElements.Clear();
                    int num = MBRandom.RoundRandomized(lootFactor);
                    float lootSkill = lootFactor / .25f;

                    for (int j = 0; j < num; j++)
                    {
                        float expectedLootedItemValue = 30f;
                        if (playerWin)
                        {
                            float valLevel = (float)Math.Max(CharacterObject.PlayerCharacter.Level, shareFromCasualty.Character.Level);
                            expectedLootedItemValue = 0.8f * (30f + lootFactor + (float)(valLevel * valLevel));
                        }
                        else
                        {
                            expectedLootedItemValue = ItemHelper.GetExpectedLootedItemValue(shareFromCasualty.Character);
                        }
                        EquipmentElement randomItem = GetRandomItem(randomEquipment,expectedLootedItemValue);
                        if (randomItem.Item != null && !randomItem.Item.NotMerchandise && equipmentElements.Count<EquipmentElement>((EquipmentElement x) => x.Item.Type == randomItem.Item.Type) == 0)
                        {
                            equipmentElements.Add(randomItem);
                        }
                    }
                    for (int k = 0; k < equipmentElements.Count; k++)
                    {
                        EquipmentElement item = equipmentElements[k];
                        ItemRosterElement itemRosterElement = new ItemRosterElement(item.Item, 1, null);
                        float single = 30f; // ItemHelper.GetExpectedLootedItemValue(shareFromCasualty.Character);
                        if (playerWin)
                        {
                            float valLevel = (float)Math.Max(CharacterObject.PlayerCharacter.Level, shareFromCasualty.Character.Level);
                            single = 0.8f * 30f + (float)(valLevel * valLevel);
                        }
                        else
                        {
                            single = ItemHelper.GetExpectedLootedItemValue(shareFromCasualty.Character);
                        }

                        EquipmentElement equipmentElement1 = itemRosterElement.EquipmentElement;
                        if (!equipmentElement1.Item.HasHorseComponent)
                        {
                            equipmentElement1 = itemRosterElement.EquipmentElement;
                            if (equipmentElement1.Item.HasArmorComponent)
                            {
                                equipmentElement1 = itemRosterElement.EquipmentElement;
                                equipmentElement = itemRosterElement.EquipmentElement;
                                ItemModifierGroup itemModifierGroup = equipmentElement.Item.ArmorComponent.ItemModifierGroup;
                                if (itemModifierGroup != null)
                                {
                                    equipmentElement = itemRosterElement.EquipmentElement;
                                    randomModifierWithTarget = itemModifierGroup.GetRandomModifierWithTarget(single / (float)equipmentElement.GetBaseValue(), 1f);
                                }
                                else
                                {
                                    randomModifierWithTarget = null;
                                }
                                equipmentElement1.SetModifier(randomModifierWithTarget);
                            }
                        }
                        else
                        {
                            equipmentElement1 = itemRosterElement.EquipmentElement;
                            equipmentElement = itemRosterElement.EquipmentElement;
                            ItemModifierGroup itemModifierGroup1 = equipmentElement.Item.HorseComponent.ItemModifierGroup;
                            if (itemModifierGroup1 != null)
                            {
                                equipmentElement = itemRosterElement.EquipmentElement;
                                itemModifier = itemModifierGroup1.GetRandomModifierWithTarget(single / (float)equipmentElement.GetBaseValue(), 1f);
                            }
                            else
                            {
                                itemModifier = null;
                            }
                            equipmentElement1.SetModifier(itemModifier);
                        }
                        itemRosters.Add(itemRosterElement);
                    }
                }
            }
            return itemRosters;
        }


        private static Equipment GetRandomEquipment(CharacterObject ch)
        {
            if (ch.IsHero)
            {
                return ch.FirstBattleEquipment;
            }
            return ch.BattleEquipments.ToList<Equipment>()[MBRandom.RandomInt(ch.BattleEquipments.Count<Equipment>())];
        }

        private static List<ItemObject> GetItemsFromEquipmentSlots(Equipment equipment)
        {
            List<ItemObject> items = new List<ItemObject>();
            for (int i =0; i<12; i++)
            {
                ItemObject item = equipment.GetEquipmentFromSlot((EquipmentIndex)i).Item;
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items;
        }

        private static CharacterObject GetCharacterWithHighestSkill(PartyBase party, SkillObject skill)
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


        public static EquipmentElement GetRandomItem(Equipment equipment, float targetValue = 0f)
        {
            EquipmentElement item;
            int num = 0;
            for (int i = 0; i < 12; i++)
            {
                item = equipment[i];
                if (item.Item != null)
                {
                    item = equipment[i];
                    if (!item.Item.NotMerchandise)
                    {
                        num++;
                    }
                }
            }
            if (num > 0)
            {
                for (int j = 0; j < 12; j++)
                {
                    EquipmentElement equipmentElement = equipment[j];
                    if (equipmentElement.Item != null)
                    {
                        item = equipment[j];
                        if (!item.Item.NotMerchandise)
                        {
                            float value = (float)equipmentElement.Item.Value + 0.1f;
                            float single = targetValue / (Math.Max(targetValue, value) * (float)num);
                            if (MBRandom.RandomFloat < single)
                            {
                                return equipmentElement;
                            }
                            num--;
                        }
                    }
                }
            }
            item = new EquipmentElement();
            return item;
        }
    }
}
