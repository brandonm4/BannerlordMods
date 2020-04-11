using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BMTweakCollection.Patches
{
    [HarmonyPatch(typeof(PartyScreenLogic), "IsExecutable")]
    public class PartyScreenLogicTroopIsExecutablePatch
    {
        public static void Postfix(ref bool __result, PartyScreenLogic.TroopType troopType, CharacterObject character, PartyScreenLogic.PartyRosterSide side)
        {
            if (troopType == PartyScreenLogic.TroopType.Prisoner)
            {
                __result = true;
            }
        }
        static bool Prepare()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PartyScreenLogic), "GetExecutableReasonText")]
    public class PartyScreenLogicTroopExecuteReasonPatch
    {
        public static void Postfix(ref string __result, CharacterObject character)        
        {
            if (!character.IsHero)
            {
                //    return GameTexts.FindText("str_cannot_execute_nonhero", null).ToString();
                __result = "Execute Troop";
            }
            //return GameTexts.FindText("str_execute_prisoner", null).ToString();
        }
        static bool Prepare()
        {
            return false;
        }
    }



    [HarmonyPatch(typeof(PartyScreenLogic), "ExecuteTroop")]
    public class PartyScreenLogic_LootLord
    {
        public PartyScreenLogic_LootLord()
        {
        }

        public static void Postfix(PartyScreenLogic.PartyCommand command)
        {
            Random random = new Random();
            CharacterObject character = command.Character;
            for (int i = 0; i < 12; i++)
            {
                if (character.HeroObject.BattleEquipment[i].Item != null)
                {
                    ItemRoster itemRoster = PartyBase.MainParty.ItemRoster;
                    EquipmentElement item = character.HeroObject.BattleEquipment[i];
                    itemRoster.AddToCounts(item.Item, 1, true);
                    item = character.HeroObject.BattleEquipment[i];
                    InformationManager.DisplayMessage(new InformationMessage(string.Concat(item.Item.Name.ToString(), " Added to inventory")));
                }
            }
        }

        static bool Prepare()
        {
            return false;
        }
    }    
}