using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace TournamentsXPanded.Models
{
    public class PrizeItem
    {
        public string ItemStringId { get; set; }
        public string ItemModifierStringId { get; set; }

        public ItemRosterElement ToItemRosterElement()
        {
            var prizeItem = MBObjectManager.Instance.GetObject<ItemObject>(ItemStringId);
            ItemModifier itemModifier = null;
            if (!string.IsNullOrWhiteSpace(ItemModifierStringId))
                itemModifier = MBObjectManager.Instance.GetObject<ItemModifier>(ItemModifierStringId);
            return new ItemRosterElement(prizeItem, 1, itemModifier);
        }
    }
}
