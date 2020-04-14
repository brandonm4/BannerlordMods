using TaleWorlds.Core;

namespace TournamentsXPanded.Models
{
    internal class TournamentEquipmentRestrictor
    {
        public bool IgnoreMounted { get; set; } = false;
        public string ExcludedItemTypeString { get; set; } = "";
        public ItemObject.ItemTypeEnum ItemType { get; set; } = ItemObject.ItemTypeEnum.Invalid;
        public string ReplacementStringId { get; set; }
    }
}