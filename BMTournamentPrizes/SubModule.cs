using HarmonyLib;

using ModLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Models;

namespace TournamentsXPanded
{
    public partial class TournamentsXPandedSubModule : MBSubModuleBase
    {
        private List<TournamentEquipmentRestrictor> restrictors = new List<TournamentEquipmentRestrictor>();
        public static string ModuleFolderName { get; } = "TournamentsXPanded";

        protected override void OnSubModuleLoad()
        {
            try
            {
                FileDatabase.Initialise(ModuleFolderName);
                var modnames = Utilities.GetModulesNames().ToList();
                if (modnames.Contains("ModLib"))
                {
                 //   SettingsDatabase.RegisterSettings(TournamentXPSettings.Instance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("TournamentsXPanded failed to initialize settings data.\n\n" + ex.ToStringFull());
            }
            string customfile = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "CustomPrizeItems.json");

            if (File.Exists(customfile))
            {
                var configtxt = File.ReadAllText(customfile);
                TournamentPrizePoolBehavior.CustomTourneyItems = JsonConvert.DeserializeObject<List<string>>(configtxt);
            }
            //Need to convert my enums to ints for this to work.
            // SettingsDatabase.RegisterSettings(TournamentXPSettings.Instance, ModuleFolderName);

            string logpath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs");
            FileLog.logPath = logpath;

            if (TournamentXPSettings.Instance.TournamentEquipmentFilter)
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

            //Will be handled by ModLib
            //TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("ModOptionsMenu", new TextObject("Mod Options"), 9990, () =>
            //{
            //    ScreenManager.PushScreen(new ModOptionsGauntletScreen());
            //}, false));
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            ShowMessage("Tournaments XPanded Loaded");
            //var loadedMods = new List<ModuleInfo>();
            //foreach (var moduleName in Utilities.GetModulesNames())
            //{
            //    var moduleInfo = new ModuleInfo();
            //    moduleInfo.Load(moduleName);
            //    loadedMods.Add(moduleInfo);
            //}


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
            }
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            //if (TournamentXPSettings.Instance.DebugMode)
            //{
            //    DoDebugPopup();
            //}
        }

        public override void OnGameInitializationFinished(Game game)
        {
            ApplyPatches(game);
            if (TournamentXPSettings.Instance.TournamentEquipmentFilter)
            {
                string[] _weaponTemplatesIdTeamSizeOne = new String[] { "tournament_template_aserai_one_participant_set_v1", "tournament_template_battania_one_participant_set_v1", "tournament_template_battania_one_participant_set_v2", "tournament_template_empire_one_participant_set_v1", "tournament_template_khuzait_one_participant_set_v1", "tournament_template_khuzait_one_participant_set_v2", "tournament_template_vlandia_one_participant_set_v1", "tournament_template_vlandia_one_participant_set_v2", "tournament_template_vlandia_one_participant_set_v3", "tournament_template_sturgia_one_participant_set_v1", "tournament_template_sturgia_one_participant_set_v2" };

                string[] _weaponTemplatesIdTeamSizeTwo = new String[] { "tournament_template_aserai_two_participant_set_v1", "tournament_template_aserai_two_participant_set_v2", "tournament_template_aserai_two_participant_set_v3", "tournament_template_battania_two_participant_set_v1", "tournament_template_battania_two_participant_set_v2", "tournament_template_battania_two_participant_set_v3", "tournament_template_battania_two_participant_set_v4", "tournament_template_battania_two_participant_set_v5", "tournament_template_empire_two_participant_set_v1", "tournament_template_empire_two_participant_set_v2", "tournament_template_empire_two_participant_set_v3", "tournament_template_khuzait_two_participant_set_v1", "tournament_template_khuzait_two_participant_set_v2", "tournament_template_khuzait_two_participant_set_v3", "tournament_template_vlandia_two_participant_set_v1", "tournament_template_vlandia_two_participant_set_v2", "tournament_template_vlandia_two_participant_set_v3", "tournament_template_vlandia_two_participant_set_v4", "tournament_template_sturgia_two_participant_set_v1", "tournament_template_sturgia_two_participant_set_v2", "tournament_template_sturgia_two_participant_set_v3" };

                string[] _weaponTemplatesIdTeamSizeFour = new String[] { "tournament_template_aserai_four_participant_set_v1", "tournament_template_aserai_four_participant_set_v2", "tournament_template_aserai_four_participant_set_v3", "tournament_template_aserai_four_participant_set_v4", "tournament_template_battania_four_participant_set_v1", "tournament_template_battania_four_participant_set_v2", "tournament_template_battania_four_participant_set_v3", "tournament_template_empire_four_participant_set_v1", "tournament_template_empire_four_participant_set_v2", "tournament_template_empire_four_participant_set_v3", "tournament_template_khuzait_four_participant_set_v1", "tournament_template_khuzait_four_participant_set_v2", "tournament_template_khuzait_four_participant_set_v3", "tournament_template_vlandia_four_participant_set_v1", "tournament_template_vlandia_four_participant_set_v2", "tournament_template_vlandia_four_participant_set_v3", "tournament_template_vlandia_four_participant_set_v4", "tournament_template_sturgia_four_participant_set_v1", "tournament_template_sturgia_four_participant_set_v2", "tournament_template_sturgia_four_participant_set_v3" };

                RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeOne);
                RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeTwo);
                RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeFour);
            }
        }

        protected void DoDebugPopup()
        {
            var patches = "Patch Status\n";
            foreach (var p in Patches)
            {
                patches += p.ToString() + ": " + p.Applied.ToString() + "\n";
            }

            TextObject info = new TextObject(patches);
            InformationManager.DisplayMessage(new InformationMessage(patches));
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

        public static void ShowMessage(string msg, Color? color = null)
        {
            if (color == null)
                color = Color.White;

            InformationManager.DisplayMessage(new InformationMessage(msg, (Color)color));
        }

        internal const int OBJ_PRIZEPOOL = 4106000;
        internal const int OBJ_TOURNAMENT_TYPE_MELEE2 = 4106001;
        internal const int OBJ_TOURNAMENT_REWARD = 4106002;

        internal const int SAVEDEF_PRIZEPOOL = 4105000;
        internal const int SAVEDEF_TOURNAMENT_TYPE_MELEE2 = 4105001;
        internal const int SAVEDEF_TOURNAMENT_REWARD = 4105002;
    }
}