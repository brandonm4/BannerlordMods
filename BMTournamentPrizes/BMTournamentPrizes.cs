using BMTournamentPrizes.Behaviors;
using BMTournamentPrizes.Models;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TournamentLib;
using TournamentLib.Models;

namespace BMTournamentPrizes
{
    public class BMTournamentPrizesMain : BMSubModuleBase
    {

        private List<TournamentEquipmentRestrictor> restrictors = new List<TournamentEquipmentRestrictor>();
        protected override void OnSubModuleLoad()
        {
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

            ShowMessage("Tournament XPerience Prize Module Loaded");

        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {            
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
                //gameStarterObject.AddModel(new TournamentPrizeExpansion());              
                MBObjectManager.Instance.RegisterType<TournamentPrizePool>("TournamentPrizePool", "TournamentPrizePools", true);                
                if (campaignGameStarter != null)
                {
                    campaignGameStarter.AddBehavior(new TournamentPrizePoolBehavior());
                }

                try
                {
                    var h = new Harmony("com.darkspyre.bannerlord.tournamentprizes");
                    h.PatchAll();
                }
                catch (Exception exception1)
                {
                    string message;
                    Exception exception = exception1;
                    string str = exception.Message;
                    Exception innerException = exception.InnerException;
                    if (innerException != null)
                    {
                        message = innerException.Message;
                    }
                    else
                    {
                        message = null;
                    }
                    MessageBox.Show(string.Concat("Tournament XP Prizes Error patching:\n", str, " \n\n", message));
                }
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
}
