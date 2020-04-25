using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
#if VERSION130
using TaleWorlds.ObjectSystem;
#endif
using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;
using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Patches.TournamentFightMissionControllerClass
{
    public class GetTeamWeaponEquipmentList : PatchBase<GetTeamWeaponEquipmentList>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentFightMissionController).GetMethod("GetTeamWeaponEquipmentList", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        //private static readonly MethodInfo PatchMethodInfoTransPile = typeof(GetTournamentPrize).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(GetTeamWeaponEquipmentList).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return TournamentXPSettings.Instance.TournamentEquipmentFilter;
        }

        public override void Reset()
        {
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            TournamentsXPandedSubModule.Harmony.Patch(TargetMethodInfo,
          //transpiler: new HarmonyMethod(PatchMethodInfoTransPile),
          prefix: new HarmonyMethod(PatchMethodInfo)
              );

            Applied = true;
        }      
        private static bool Prefix(TournamentFightMissionController __instance, ref List<Equipment> __result, int teamSize)
        {

            List<string> list;
            List<Equipment> equipment = new List<Equipment>();
            string stringId = PlayerEncounter.Settlement.Culture.StringId;

            var list4 = (string[])Traverse.Create(__instance).Field("_weaponTemplatesIdTeamSizeFour").GetValue();
            var list2 = (string[])Traverse.Create(__instance).Field("_weaponTemplatesIdTeamSizeTwo").GetValue();
            var list1 = (string[])Traverse.Create(__instance).Field("_weaponTemplatesIdTeamSizeOne").GetValue();
            //var list2 = (string[])typeof(TournamentArcheryMissionController).GetField("_weaponTemplatesIdTeamSizeTwo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            //var list1 = (string[])typeof(TournamentArcheryMissionController).GetField("_weaponTemplatesIdTeamSizeOne", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            if (teamSize == 4)
            {
                //list = __instance._weaponTemplatesIdTeamSizeFour.ToList<string>();
                list = list4.ToList<string>();
            }
            else
            {
                list = (teamSize == 2 ? list2.ToList<string>() : list1.ToList<string>());
            }
            List<string> strs = list;
            strs = strs.FindAll((string x) => x.Contains(stringId));

            var template = MBObjectManager.Instance.GetObject<CharacterObject>(strs[MBRandom.RandomInt(strs.Count)]);
            List<Equipment> battleEquipments = ApplyEquipmentRules(template);

            foreach (Equipment battleEquipment in battleEquipments)
            {
                Equipment equipment1 = new Equipment();
                equipment1.FillFrom(battleEquipment, true);
                equipment.Add(equipment1);
            }
            __result = equipment;

            return false;
        }

        private static List<Equipment> ApplyEquipmentRules(CharacterObject template)
        {
            List<Equipment> equipments = new List<Equipment>();
            foreach (Equipment battleEquipment in template.BattleEquipments)
            {
                var equipment = battleEquipment.Clone();
                equipments.Add(equipment);
            }


                foreach (var r in TournamentsXPandedSubModule.restrictors.OrderBy(x => x.RuleOrder))
            {
                if (!string.IsNullOrWhiteSpace(r.TargetCultureId))
                {
                    if (template.Culture.StringId != r.TargetCultureId)
                    {
                        continue;
                    }
                }
                try
                {
                    foreach (Equipment battleEquipment in equipments)
                    {                        
                        if (r.TargetIgnoreMounted && battleEquipment.Horse.Item != null)
                        {                          
                            continue;
                        }
                        if (battleEquipment.GetEquipmentFromSlot(EquipmentIndex.Weapon0).Item.ItemType == r.TargetItemType)
                        {
                            if (r.ReplacementAddHorse)
                            {
                                var horseid = ItemObject.All.Where(x => x.Culture == template.Culture && x.ItemType == ItemObject.ItemTypeEnum.Horse).GetRandomElement().StringId;
                                var itemRoster = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>(horseid));
                                battleEquipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Horse, itemRoster.EquipmentElement);
                            }

                            if (r.ReplacementItemType != ItemObject.ItemTypeEnum.Invalid)
                            {
                                var repitem = ItemObject.All.Where(x => x.Culture == template.Culture && x.ItemType == r.ReplacementItemType).GetRandomElement();
                                var itemRoster = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>(repitem.StringId));
                                battleEquipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon0, itemRoster.EquipmentElement);
                            }
                            else
                                if (!string.IsNullOrWhiteSpace(r.ReplacementItemStringId))
                            {
                                var itemRoster = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>(r.ReplacementItemStringId));
                                battleEquipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon0, itemRoster.EquipmentElement);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("TournamentsXPanded\nError Applying Rule Equipement Filter\n" + ex.ToStringFull());
                    ErrorLog.Log("Error Applying Rule Equipement Filter\n" + ex.ToStringFull());
                }

            }
            return equipments;
        }
    }
}