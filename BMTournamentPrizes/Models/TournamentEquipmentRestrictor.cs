using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace BMTournamentPrizes.Models
{
    internal class TournamentEquipmentRestrictor
    {
        public bool IgnoreMounted { get; set; } = false;
        public string ExcludedItemTypeString { get; set; } = "";
        public ItemObject.ItemTypeEnum ItemType { get; set; } = ItemObject.ItemTypeEnum.Invalid;
        public string ReplacementStringId { get; set; }
    }
}
