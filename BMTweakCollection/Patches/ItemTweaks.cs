using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace BMTweakCollection.Patches
{
    public class ItemTweaks
    {
        public static void MakeCivilianSaddles(int maxArmorValue)
        {
            List<ItemObject> saddles = new List<ItemObject>();

            MBObjectManager.Instance.GetAllInstancesOfObjectType<ItemObject>(ref saddles);
            saddles = saddles.Where(x => x.HasArmorComponent && x.ArmorComponent.Item.HasSaddleComponent).ToList();

            foreach (var item in saddles)
            {
                 if (!item.ItemFlags.HasFlag(ItemFlags.Civilian) && item.ArmorComponent.BodyArmor <= maxArmorValue)
                {
                    Traverse.Create(item).Field("ItemFlags").SetValue(item.ItemFlags | ItemFlags.Civilian);
                    
                }
            }
        }
    }
}
