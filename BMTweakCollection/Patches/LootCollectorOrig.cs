using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
    internal class LootCollector
    {
        public TroopRoster CasualtiesInBattle
        {
            get;
            private set;
        }

        internal ItemRoster LootedItems
        {
            get;
            private set;
        }

        internal TroopRoster LootedMembers
        {
            get;
            private set;
        }

        internal TroopRoster LootedPrisoners
        {
            get;
            private set;
        }

        internal LootCollector()
        {
            this.LootedMembers = new TroopRoster();
            this.LootedPrisoners = new TroopRoster();
            this.LootedItems = new ItemRoster();
            this.CasualtiesInBattle = new TroopRoster();
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

        private static Equipment GetRandomEquipment(CharacterObject ch)
        {
            if (ch.IsHero)
            {
                return ch.FirstBattleEquipment;
            }
            return ch.BattleEquipments.ToList<Equipment>()[MBRandom.RandomInt(ch.BattleEquipments.Count<Equipment>())];
        }

        internal void GiveShareOfLootToParty(PartyBase partyToReceiveLoot, PartyBase winnerParty, float lootAmount)
        {
            bool flag = winnerParty == PartyBase.MainParty;
            List<TroopRosterElement> troopRosterElements = new List<TroopRosterElement>();
            foreach (TroopRosterElement lootedMember in this.LootedMembers)
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
                this.LootedMembers.AddToCounts(troopRosterElement1.Character, -1, false, 0, 0, true, -1);
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
            for (int j = this.LootedItems.Count<ItemRosterElement>() - 1; j >= 0; j--)
            {
                ItemRosterElement elementCopyAtIndex = this.LootedItems.GetElementCopyAtIndex(j);
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
                this.LootedItems.AddToCounts(itemRosterElement, -num, true);
            }
            partyToReceiveLoot.ItemRoster.Add(itemRosterElements);
            for (int l = this.LootedPrisoners.Count<TroopRosterElement>() - 1; l >= 0; l--)
            {
                int elementNumber = this.LootedPrisoners.GetElementNumber(l);
                CharacterObject characterAtIndex = this.LootedPrisoners.GetCharacterAtIndex(l);
                int num1 = 0;
                for (int m = 0; m < elementNumber; m++)
                {
                    if (MBRandom.RandomFloat < lootAmount)
                    {
                        partyToReceiveLoot.MemberRoster.AddToCounts(characterAtIndex, 1, false, 0, 0, true, -1);
                        num1++;
                    }
                }
                this.LootedPrisoners.AddToCounts(characterAtIndex, -num1, false, 0, 0, true, -1);
            }
            ICollection<TroopRosterElement> troopRosterElements1 = new List<TroopRosterElement>();
            for (int n = this.CasualtiesInBattle.Count<TroopRosterElement>() - 1; n >= 0; n--)
            {
                int elementNumber1 = this.CasualtiesInBattle.GetElementNumber(n);
                CharacterObject characterObject = this.CasualtiesInBattle.GetCharacterAtIndex(n);
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
                this.CasualtiesInBattle.AddToCounts(characterObject, -num2, false, 0, 0, true, -1);
            }
            ExplainedNumber explainedNumber = new ExplainedNumber(1f, null);
            if (winnerParty.MobileParty != null && winnerParty.MobileParty.Leader != null)
            {
                SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Roguery, DefaultSkillEffects.RogueryLootBonus, winnerParty.MobileParty.Leader, ref explainedNumber, true);
            }
            if (flag)
            {
                IEnumerable<ItemRosterElement> itemRosterElements1 = this.LootCasualties(troopRosterElements1, explainedNumber.ResultNumber);
                partyToReceiveLoot.ItemRoster.Add(itemRosterElements1);
            }
            else if (partyToReceiveLoot.LeaderHero != null)
            {
                int gold = LootCollector.ConvertLootToGold(this.LootCasualties(troopRosterElements1, 0.5f));
                gold = MBMath.Round((float)gold * 0.5f * explainedNumber.ResultNumber);
                GiveGoldAction.ApplyBetweenCharacters(null, partyToReceiveLoot.LeaderHero, gold, false);
                return;
            }
        }

        private IEnumerable<ItemRosterElement> LootCasualties(ICollection<TroopRosterElement> shareFromCasualties, float lootFactor)
        {
            EquipmentElement equipmentElement;
            ItemModifier randomModifierWithTarget;
            ItemModifier itemModifier;
            ItemRoster itemRosters = new ItemRoster();
            List<EquipmentElement> equipmentElements = new List<EquipmentElement>();
            foreach (TroopRosterElement shareFromCasualty in shareFromCasualties)
            {
                for (int i = 0; i < 1; i++)
                {
                    Equipment randomEquipment = LootCollector.GetRandomEquipment(shareFromCasualty.Character);
                    equipmentElements.Clear();
                    int num = MBRandom.RoundRandomized(lootFactor);
                    for (int j = 0; j < num; j++)
                    {
                        float expectedLootedItemValue = ItemHelper.GetExpectedLootedItemValue(shareFromCasualty.Character);
                        EquipmentElement randomItem = randomEquipment.GetRandomItem(expectedLootedItemValue);
                        if (randomItem.Item != null && !randomItem.Item.NotMerchandise && equipmentElements.Count<EquipmentElement>((EquipmentElement x) => x.Item.Type == randomItem.Item.Type) == 0)
                        {
                            equipmentElements.Add(randomItem);
                        }
                    }
                    for (int k = 0; k < equipmentElements.Count; k++)
                    {
                        EquipmentElement item = equipmentElements[k];
                        ItemRosterElement itemRosterElement = new ItemRosterElement(item.Item, 1, null);
                        float single = ItemHelper.GetExpectedLootedItemValue(shareFromCasualty.Character);
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

        internal void MakeFreedHeroesEscape(TroopRoster freedTroops)
        {
            for (int i = freedTroops.Count<TroopRosterElement>() - 1; i >= 0; i--)
            {
                CharacterObject characterAtIndex = freedTroops.GetCharacterAtIndex(i);
                if (characterAtIndex.IsHero)
                {
                    if (!characterAtIndex.IsPlayerCharacter)
                    {
                        EndCaptivityAction.ApplyByReleasedAfterBattle(characterAtIndex.HeroObject, null, null);
                    }
                    freedTroops.RemoveTroop(characterAtIndex, 1, new UniqueTroopDescriptor(), 0);
                }
            }
        }

        public void MakePrisonerHeroesEscape(float escapeChance)
        {
            foreach (TroopRosterElement troopRosterElement in this.LootedMembers.RemoveIf((TroopRosterElement lordElement) => {
                if (!lordElement.Character.IsHero || lordElement.Character.HeroObject.IsHumanPlayerCharacter)
                {
                    return false;
                }
                if (lordElement.Character.HeroObject.NeverBecomePrisoner)
                {
                    return true;
                }
                return MBRandom.RandomFloat < escapeChance;
            }))
            {
                MakeHeroFugitiveAction.Apply(troopRosterElement.Character.HeroObject);
                troopRosterElement.Character.HeroObject.DaysLeftToRespawn = 2;
                Debug.Print(String.Concat((object)"[OZANDEBUG] ", troopRosterElement.Character.HeroObject.Name, (object)" is escaped and now fugitive."), 0, Debug.DebugColor.DarkRed, (ulong)256);
            }
        }
    }
}