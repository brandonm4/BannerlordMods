using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace TownArmorerOrderMod.Utils
{
    class ItemObjectTypeTuple
    {
        private string name;
        private ItemObject.ItemTypeEnum type;
        private string article;
        public string Name { get => name; set => name = value; }
        public ItemObject.ItemTypeEnum Type { get => type; set => type = value; }
        public string Article { get => article; set => article = value; }

        public ItemObjectTypeTuple(string article, string name, ItemObject.ItemTypeEnum type)
        {
            this.name = name;
            this.type = type;
            this.Article = article;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
