using BMTweakCollection.LootTweaks;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace LootEveryone
{
	[HarmonyPatch(typeof(InventoryManager), "OpenScreenAsLoot")]
	public class AddToLoot
	{
		public AddToLoot()
		{
		}

		public static void Prefix(Dictionary<PartyBase, ItemRoster> itemRostersToLoot)
		{
			foreach (EquipmentElement equipmentElement in LootBehavior.LootedItems)
			{
				itemRostersToLoot[PartyBase.MainParty].AddToCounts(equipmentElement.Item, 1, true);
			}
            var cnt = LootBehavior.LootedItems.Count;
            LootBehavior.LootedItems.Clear();
			InformationManager.DisplayMessage(new InformationMessage(string.Concat(cnt, " Items added to inventory")));
		}
	}
}