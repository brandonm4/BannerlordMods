using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace BMTweakCollection.Models
{
    public class CustomTroopLimitModel : DefaultTroopCountLimitModel
    {
        private int _bmhideoutmaxtroops;
        public CustomTroopLimitModel()
        {
            _bmhideoutmaxtroops = BMTweakCollectionMain.Configuration.MaxHideoutTroops;
        }
        public override int GetHideoutBattlePlayerMaxTroopCount()
        {
            return _bmhideoutmaxtroops;
        }
    }
}
