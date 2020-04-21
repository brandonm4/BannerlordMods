﻿using Newtonsoft.Json;
using TaleWorlds.Core;

namespace TournamentsXPanded.Models
{
    internal class TournamentEquipmentRestrictor
    {
        public int RuleOrder { get; set; } = 99;
        public bool TargetIgnoreMounted { get; set; } = false;
        public string TargetItemTypeString { get; set; } = "";
        public string TargetItemStringId { get; set; } = "";
        public string TargetCultureId { get; set; } = "";

        public bool ReplacementAddHorse { get; set; } = false;
        public string ReplacementItemTypeString { get; set; }        
        public string ReplacementItemStringId { get; set; }
        public string ReplacementCultureId { get; set; }

        [JsonIgnore]
        public ItemObject.ItemTypeEnum TargetItemType { get; set; } = ItemObject.ItemTypeEnum.Invalid;
        [JsonIgnore]
        public ItemObject.ItemTypeEnum ReplacementItemType { get; set; } = ItemObject.ItemTypeEnum.Invalid;
    }
}