using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMTweakCollection.Models
{
    public class BMRandomTweaksConfiguration
    {
        public  bool MaxHideoutTroopsEnabled { get; set; } = true;
        public  int MaxHideoutTroops { get; set; } = 20;

        public  bool ItemMultiplayerToSinglePlayerEnabled { get; set; } = true;

        public  bool CustomSmithingModelEnabled { get; set; } = true;
        public  int CustomSmithingXPDivisor { get; set; } = 8;

        public  bool RemoveAllEquippedHorses { get; set; }
        public int BonusTournamentGold { get; set; } = 500;

        
    }
}
