////using HarmonyLib;
////using SandBox;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TaleWorlds.Core;
//using TaleWorlds.MountAndBlade;

//using HarmonyLib;
//using SandBox;
//using TaleWorlds.CampaignSystem;

//namespace BMTweakCollection.Patch
//{
//    [HarmonyPatch(typeof(TournamentFightMissionController), "GetTeamWeaponEquipmentList")]
//    public class TournamentFightMissionControllerPatch1
//    {
//        public TournamentFightMissionControllerPatch1()
//        {

//        }

//        public static bool Prefix(List<Equipment> __result, int teamSize, ref string[] ____weaponTemplatesIdTeamSizeOne, ref string[] ____weaponTemplatesIdTeamSizeTwo, ref string[] ____weaponTemplatesIdTeamSizeFour)
//        {

//            List<string> list;
//            List<Equipment> equipment = new List<Equipment>();
//            string stringId = PlayerEncounter.Settlement.Culture.StringId;
//            if (teamSize == 4)
//            {
//                list = ____weaponTemplatesIdTeamSizeFour.ToList<string>();
//            }
//            else
//            {
//                list = (teamSize == 2 ? ____weaponTemplatesIdTeamSizeTwo.ToList<string>() : ____weaponTemplatesIdTeamSizeOne.ToList<string>());
//            }
//            List<string> strs = list;
//            strs = strs.FindAll((string x) => x.Contains(stringId));
//            var bNoSpearSet = false;

            

//            while (!bNoSpearSet)
//            {
//                var battleSets = MBObjectManager.Instance.GetObject<CharacterObject>(strs[MBRandom.RandomInt(strs.Count)]).BattleEquipments;
//                equipment = new List<Equipment>();
//                foreach (Equipment battleEquipment in battleSets)
//                {
//                    var equipmentElements = (Traverse.Create(battleEquipment).Field("_itemSlots").GetValue() as EquipmentElement[]);
//                    var bHasHorse = false;
//                    var bHasSpear = false;

//                    if (battleEquipment.Horse.Item != null)
//                    {
//                        bHasHorse = true;
//                    }
//                    else
//                    {
//                        foreach (var ee in equipmentElements)
//                        {
//                            if (ee.Item != null && ee.Item.ItemType == ItemObject.ItemTypeEnum.Polearm)
//                            {
//                                bHasSpear = true;
//                            }
//                        }
//                    }

//                    if (bHasHorse || !bHasSpear)
//                    {
//                        Equipment equipment1 = new Equipment();
//                        equipment1.FillFrom(battleEquipment, true);
//                        equipment.Add(equipment1);
//                    }
//                }

//                if (equipment.Count == teamSize)
//                {
//                    bNoSpearSet = true;
//                }
//            }

//            //foreach (Equipment battleEquipment in battleSets)
//            //{
              
//            //}
        
//        __result = equipment;
//            return false;
//        }
//    /*
//private List<Equipment> GetTeamWeaponEquipmentList(int teamSize)
//    {
//        List<string> list;
//        List<Equipment> equipment = new List<Equipment>();
//        string stringId = PlayerEncounter.Settlement.Culture.StringId;
//        if (teamSize == 4)
//        {
//            list = this._weaponTemplatesIdTeamSizeFour.ToList<string>();
//        }
//        else
//        {
//            list = (teamSize == 2 ? this._weaponTemplatesIdTeamSizeTwo.ToList<string>() : this._weaponTemplatesIdTeamSizeOne.ToList<string>());
//        }
//        List<string> strs = list;
//        strs = strs.FindAll((string x) => x.Contains(stringId));
//        foreach (Equipment battleEquipment in MBObjectManager.Instance.GetObject<CharacterObject>(strs[MBRandom.RandomInt(strs.Count)]).BattleEquipments)
//        {
//            Equipment equipment1 = new Equipment();
//            equipment1.FillFrom(battleEquipment, true);
//            equipment.Add(equipment1);
//        }
//        return equipment;
//    }
//*/
//}
//}
