using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace TownArmorerOrderMod.Utils
{
    public class ItemObjectTierTuple
    {
        private string name;
        private ItemObject.ItemTiers tier;
        public string Name { get => name; set => name = value; }
        public ItemObject.ItemTiers Tier { get => tier; set => tier = value; }

        public ItemObjectTierTuple(string name, ItemObject.ItemTiers type)
        {
            this.name = name;
            this.tier = type;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
