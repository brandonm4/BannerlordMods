using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BMItemTweaks
{
    public class BMItemTweaksMain : MBSubModuleBase
    {
        protected bool _removeHorses = false;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            Campaign gameType = game.GameType as Campaign;
            if (gameType != null)
            {
                foreach(ItemObject i in gameType.Items)
                {
                    if (!i.NotMerchandise)
                    {
                        typeof(ItemObject).GetProperty("MultiplayerItem").SetValue(i, false);
                    }                                        
                }                                           
            }
        }
    }
}
