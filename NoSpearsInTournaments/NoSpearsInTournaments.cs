using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TournamentLib;
using TournamentLib.Models;

namespace NoSpearsInTournaments
{
    public class NoSpearsInTournamentsMain : BMSubModuleBase
    {      
        public List<TournamentEquipmentRestrictor> restrictors = new List<TournamentEquipmentRestrictor>();

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();          

            if (TournamentConfiguration.Instance.TournamentTweaks.TournamentEquipmentFilter)
            {
                //Eventually plan to let people define their own
                restrictors.Add(new TournamentEquipmentRestrictor
                {
                    IgnoreMounted = true,
                    ExcludedItemTypeString = "Polearm",
                    ReplacementStringId = "sturgia_sword_1_t2",
                });

                foreach (var d in restrictors)
                {
                    d.ItemType = (ItemObject.ItemTypeEnum)Enum.Parse(typeof(ItemObject.ItemTypeEnum), d.ExcludedItemTypeString);
                }
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (TournamentConfiguration.Instance.TournamentTweaks.TournamentEquipmentFilter)
            {
                ShowMessage("Tournament XPerience NoSpears Module Enabled");
            }

        }

        public override void OnGameInitializationFinished(Game game)
        {
            
            base.OnGameInitializationFinished(game);

            if (TournamentConfiguration.Instance.TournamentTweaks.TournamentEquipmentFilter)
            {
                string[] _weaponTemplatesIdTeamSizeOne = new String[] { "tournament_template_aserai_one_participant_set_v1", "tournament_template_battania_one_participant_set_v1", "tournament_template_battania_one_participant_set_v2", "tournament_template_empire_one_participant_set_v1", "tournament_template_khuzait_one_participant_set_v1", "tournament_template_khuzait_one_participant_set_v2", "tournament_template_vlandia_one_participant_set_v1", "tournament_template_vlandia_one_participant_set_v2", "tournament_template_vlandia_one_participant_set_v3", "tournament_template_sturgia_one_participant_set_v1", "tournament_template_sturgia_one_participant_set_v2" };

                string[] _weaponTemplatesIdTeamSizeTwo = new String[] { "tournament_template_aserai_two_participant_set_v1", "tournament_template_aserai_two_participant_set_v2", "tournament_template_aserai_two_participant_set_v3", "tournament_template_battania_two_participant_set_v1", "tournament_template_battania_two_participant_set_v2", "tournament_template_battania_two_participant_set_v3", "tournament_template_battania_two_participant_set_v4", "tournament_template_battania_two_participant_set_v5", "tournament_template_empire_two_participant_set_v1", "tournament_template_empire_two_participant_set_v2", "tournament_template_empire_two_participant_set_v3", "tournament_template_khuzait_two_participant_set_v1", "tournament_template_khuzait_two_participant_set_v2", "tournament_template_khuzait_two_participant_set_v3", "tournament_template_vlandia_two_participant_set_v1", "tournament_template_vlandia_two_participant_set_v2", "tournament_template_vlandia_two_participant_set_v3", "tournament_template_vlandia_two_participant_set_v4", "tournament_template_sturgia_two_participant_set_v1", "tournament_template_sturgia_two_participant_set_v2", "tournament_template_sturgia_two_participant_set_v3" };

                string[] _weaponTemplatesIdTeamSizeFour = new String[] { "tournament_template_aserai_four_participant_set_v1", "tournament_template_aserai_four_participant_set_v2", "tournament_template_aserai_four_participant_set_v3", "tournament_template_aserai_four_participant_set_v4", "tournament_template_battania_four_participant_set_v1", "tournament_template_battania_four_participant_set_v2", "tournament_template_battania_four_participant_set_v3", "tournament_template_empire_four_participant_set_v1", "tournament_template_empire_four_participant_set_v2", "tournament_template_empire_four_participant_set_v3", "tournament_template_khuzait_four_participant_set_v1", "tournament_template_khuzait_four_participant_set_v2", "tournament_template_khuzait_four_participant_set_v3", "tournament_template_vlandia_four_participant_set_v1", "tournament_template_vlandia_four_participant_set_v2", "tournament_template_vlandia_four_participant_set_v3", "tournament_template_vlandia_four_participant_set_v4", "tournament_template_sturgia_four_participant_set_v1", "tournament_template_sturgia_four_participant_set_v2", "tournament_template_sturgia_four_participant_set_v3" };

                RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeOne);
                RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeTwo);
                RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeFour);
            }
        }

        private void RemoveTournamentSpearFootSets(string[] templates)
        {
            foreach (var r in restrictors)
            {
                foreach (var t in templates)
                {
                    foreach (Equipment battleEquipment in MBObjectManager.Instance.GetObject<CharacterObject>(t).BattleEquipments)
                    {                        
                        if (r.IgnoreMounted && battleEquipment.Horse.Item != null)
                        {
                            break;
                        }
                        if (battleEquipment.GetEquipmentFromSlot(EquipmentIndex.Weapon0).Item.ItemType == r.ItemType)
                        {
                            var itemRoster = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>(r.ReplacementStringId));
                            battleEquipment.AddEquipmentToSlotWithoutAgent(EquipmentIndex.Weapon0, itemRoster.EquipmentElement);
                        }
                    }
                }
            }
        }
    }

    public class TournamentEquipmentRestrictor
    {
        public bool IgnoreMounted { get; set; } = false;
        public string ExcludedItemTypeString { get; set; } = "";
        public ItemObject.ItemTypeEnum ItemType { get; set; } = ItemObject.ItemTypeEnum.Invalid;
        public string ReplacementStringId { get; set; }
    }
}
