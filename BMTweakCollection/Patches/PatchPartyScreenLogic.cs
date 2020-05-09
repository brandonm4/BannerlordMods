using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

using TournamentsXPanded.Common.Patches;

namespace BMTweakCollection.Patches
{
    internal class RemoveItemFromItemRoster : PatchBase<RemoveItemFromItemRoster>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(PartyScreenLogic).GetMethod("RemoveItemFromItemRoster", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(RemoveItemFromItemRoster).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return false;
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

        public static bool Prefix(PartyScreenLogic __instance, ItemCategory itemCategory, int numOfItemsLeftToRemove)
        {
            ItemCategory itemCategory1;
            int numItemsRequired = numOfItemsLeftToRemove;
            ItemRosterElement[] copyOfAllElements = __instance.RightOwnerParty.ItemRoster.GetCopyOfAllElements();
            int totalItemCount = __instance.RightOwnerParty.ItemRoster.Count;
            ItemRosterElement[] cleanItemRoster = new ItemRosterElement[totalItemCount];
            Array.Copy(copyOfAllElements, 0, cleanItemRoster, 0, totalItemCount);
            Array.Sort<ItemRosterElement>(cleanItemRoster, (ItemRosterElement left, ItemRosterElement right) =>
            {
                EquipmentElement equipmentElement = left.EquipmentElement;
                int itemValue = equipmentElement.ItemValue;
                equipmentElement = right.EquipmentElement;
                return itemValue.CompareTo(equipmentElement.ItemValue);
            });
            ItemRosterElement[] itemRosterElementArray = cleanItemRoster;
            for (int i = 0; i < itemRosterElementArray.Length; i++)
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