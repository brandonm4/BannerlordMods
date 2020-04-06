//using HarmonyLib;
//using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

using HarmonyLib;
using SandBox;
using TaleWorlds.CampaignSystem;

namespace BMTweakCollection.Patch
{
    /*
    [HarmonyPatch(typeof(TournamentFightMissionController), "GetTeamWeaponEquipmentList")]
    public class TournamentFightMissionControllerRemoveSpearsFromTournament
    {     
        public static bool Prefix(List<Equipment> __result, int teamSize, ref string[] ____weaponTemplatesIdTeamSizeOne, ref string[] ____weaponTemplatesIdTeamSizeTwo, ref string[] ____weaponTemplatesIdTeamSizeFour)
        {
            List<string> list;
            List<Equipment> equipment = new List<Equipment>();
            string stringId = PlayerEncounter.Settlement.Culture.StringId;
            if (teamSize == 4)
            {
                list = ____weaponTemplatesIdTeamSizeFour.ToList<string>();
            }
            else
            {
                list = (teamSize == 2 ? ____weaponTemplatesIdTeamSizeTwo.ToList<string>() : ____weaponTemplatesIdTeamSizeOne.ToList<string>());
            }
            List<string> strs = list;
            strs = strs.FindAll((string x) => x.Contains(stringId));
            foreach (Equipment battleEquipment in MBObjectManager.Instance.GetObject<CharacterObject>(strs[MBRandom.RandomInt(strs.Count)]).BattleEquipments)
            {
                Equipment equipment1 = new Equipment();
                equipment1.FillFrom(battleEquipment, true);
                if (equipment1.GetEquipmentFromSlot(EquipmentIndex.Weapon0).Item.ItemType == ItemObject.ItemTypeEnum.Polearm
                    && equipment1.Horse.Item == null)
                {
                    ItemObject sword = Game.Current.ObjectManager.GetObject<ItemObject>("sturgia_sword_1_t2");
                    equipment1.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon0, new EquipmentElement(sword));
                }                    
                equipment.Add(equipment1);
            }
            
            __result = equipment;
            return false;
        }
        */
        /*
    private List<Equipment> GetTeamWeaponEquipmentList(int teamSize)
        {
            List<string> list;
            List<Equipment> equipment = new List<Equipment>();
            string stringId = PlayerEncounter.Settlement.Culture.StringId;
            if (teamSize == 4)
            {
                list = this._weaponTemplatesIdTeamSizeFour.ToList<string>();
            }
            else
            {
                list = (teamSize == 2 ? this._weaponTemplatesIdTeamSizeTwo.ToList<string>() : this._weaponTemplatesIdTeamSizeOne.ToList<string>());
            }
            List<string> strs = list;
            strs = strs.FindAll((string x) => x.Contains(stringId));
            foreach (Equipment battleEquipment in MBObjectManager.Instance.GetObject<CharacterObject>(strs[MBRandom.RandomInt(strs.Count)]).BattleEquipments)
            {
                Equipment equipment1 = new Equipment();
                equipment1.FillFrom(battleEquipment, true);
                equipment.Add(equipment1);
            }
            return equipment;
        }    
    }
    */
}

