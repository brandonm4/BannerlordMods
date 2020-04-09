using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace GrandTheftHorse.Behaviors
{
    public class GrandTheftHorseMissionLogic : MissionLogic
    {
        public override void AfterStart()
        {
            GrandTheftHorseSubmodule.InBattle = true;
        }
        protected override void OnEndMission()
        {
            GrandTheftHorseSubmodule.InBattle = false;
        }
    }
}
