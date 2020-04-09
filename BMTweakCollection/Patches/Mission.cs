using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(Mission), "SpawnTroop")]
    public class MissionNoHorsePatch
    {
        public static bool Prefix(ref bool spawnWithHorse, ref bool forceDismounted)
        {
            spawnWithHorse = false;
            forceDismounted = true;
            return true;
        }
        static bool Prepare()
        {
            return false;
        }
    }
}
