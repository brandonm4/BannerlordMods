using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BMTweakCollection.Patches
{
	[HarmonyPatch(typeof(PartyScreenLogic), "RemoveItemFromItemRoster")]
	internal class PatchPartyScreenLogic
	{
		public PatchPartyScreenLogic()
		{
		}

		public static bool Prefix(PartyScreenLogic __instance, ItemCategory itemCategory, int numOfItemsLeftToRemove)
		{
			ItemCategory itemCategory1;
			int numItemsRequired = numOfItemsLeftToRemove;
			ItemRosterElement[] copyOfAllElements = __instance.RightOwnerParty.ItemRoster.GetCopyOfAllElements();
			int totalItemCount = __instance.RightOwnerParty.ItemRoster.Count;
			ItemRosterElement[] cleanItemRoster = new ItemRosterElement[totalItemCount];
			Array.Copy((Array)copyOfAllElements, 0, cleanItemRoster, 0, totalItemCount);
			Array.Sort<ItemRosterElement>(cleanItemRoster, (ItemRosterElement left, ItemRosterElement right) => {
				EquipmentElement equipmentElement = left.EquipmentElement;
				int itemValue = equipmentElement.ItemValue;
				equipmentElement = right.EquipmentElement;
				return itemValue.CompareTo(equipmentElement.ItemValue);
			});
			ItemRosterElement[] itemRosterElementArray = cleanItemRoster;
			for (int i = 0; i < (int)itemRosterElementArray.Length; i++)
			{
				ItemRosterElement rosterElement = itemRosterElementArray[i];
				ItemObject item = rosterElement.EquipmentElement.Item;
				if (item != null)
				{
					itemCategory1 = item.ItemCategory;
				}
				else
				{
					itemCategory1 = null;
				}
				if (itemCategory1 == itemCategory)
				{
					int amountToRemove = Math.Min(numItemsRequired, rosterElement.Amount);
					__instance.RightOwnerParty.ItemRoster.AddToCounts(rosterElement.EquipmentElement, -amountToRemove, true);
					numItemsRequired -= amountToRemove;
					if (numItemsRequired == 0)
					{
						break;
					}
				}
			}
			return false;
		}
	}
}