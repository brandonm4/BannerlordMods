using HarmonyLib;


using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Models;


using ModLib;
using AutoMapper;

namespace TournamentsXPanded
{
    public partial class TournamentsXPandedSubModule : MBSubModuleBase
    {
        private List<TournamentEquipmentRestrictor> restrictors = new List<TournamentEquipmentRestrictor>();
        public static string ModuleFolderName { get; } = "TournamentsXPanded";

        protected override void OnSubModuleLoad()
        {



            //Setup Logging
            if (File.Exists(System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs")))
            {
                File.Delete(System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs"));
            }
            string logpath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs", "logfile.txt");
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(logpath)))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logpath));
            }
            ErrorLog.LogPath = logpath;



            var modnames = Utilities.GetModulesNames().ToList();
            bool modLibLoaded = false;
            if (modnames.Contains("ModLib"))
            {
                try
                {
                    FileDatabase.Initialise(ModuleFolderName);
                    TournamentXPSettingsModLib settings = FileDatabase.Get<TournamentXPSettingsModLib>(TournamentXPSettingsModLib.InstanceID);
                    if (settings == null) settings = new TournamentXPSettingsModLib();
                    SettingsDatabase.RegisterSettings(settings);
                    modLibLoaded = true;
                    TournamentXPSettings.SetSettings(settings.GetSettings());
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("TournamentsXPanded failed to initialize settings data.\n\n" + ex.ToStringFull());
                    modLibLoaded = false;
                }
            }

            if (!modLibLoaded)
            {
                TournamentXPSettings settings = new TournamentXPSettings();
                string configPath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "tournamentxpsettings.json");
                if (File.Exists(configPath))
                {
                    var settingsjson = File.ReadAllText(configPath);
                    settings = JsonConvert.DeserializeObject<TournamentXPSettings>(settingsjson);
                }
                else
                {
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                    serializerSettings.Formatting = Formatting.Indented;

                    var settingsjson = JsonConvert.SerializeObject(settings, serializerSettings);
                    File.WriteAllText(configPath, settingsjson);
                }
                TournamentXPSettings.SetSettings(settings);
            }

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
        }


        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {

            ShowMessage("Tournaments XPanded Loaded");
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
            string customfile = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "CustomPrizeItems.json");
            if (File.Exists(customfile))
            {
                var configtxt = File.ReadAllText(customfile);
                var customItems = JsonConvert.DeserializeObject<List<string>>(configtxt);
                InitCustomItems(customItems);
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


        private void InitCustomItems(List<string> customItems)
        {
            List<ItemObject> tourneyItems = new List<ItemObject>();
            List<string> problemids = new List<string>();
            TournamentPrizePoolBehavior.CustomTourneyItems = new List<ItemObject>();
            foreach (var id in customItems)
            {
                ItemObject item;
                try
                {
                    item = Game.Current.ObjectManager.GetObject<ItemObject>(id);
                }
                catch
                {
                    item = null;
                }
                if (item == null || item.ItemType == ItemObject.ItemTypeEnum.Invalid)
                {
                    problemids.Add(id);
                    ErrorLog.Log(String.Concat("WARNING: Tournament Prize System\n", "Invalid Item Id detected in prize list.  Please remove from the list.  Ignoring problem item and continuing.\n\n", id));
                }
                else
                {
                    TournamentPrizePoolBehavior.CustomTourneyItems.Add(item);
                }
            }
            if (problemids.Count > 0)
            {
                string info = "Errors in Custom Prize List.\nReview list and correct or remove these entries:\n";
                foreach (var p in problemids)
                {
                    info = String.Concat(info, p, "\n");
                }

                MessageManager.DisplayDebugMessage(info);
                //InformationManager.ShowInquiry(new InquiryData("Tournament Prize Errors",
                //    info,
                //    true, false, "Ok", "No", null, null, ""), false);

            }
        }

        /* Mod Settings Interfaces */
        #region ModSettings
        public static Dictionary<string, string> GetModSettingValue()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            PropertyInfo[] properties = typeof(TournamentXPSettings).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                settings.Add(property.Name, property.GetValue(TournamentXPSettings.Instance).ToString());
            }
            return settings;
        }

        public static void SaveModSettingValue(Dictionary<string, string> newSettings)
        {
            // write to save settings to anywhere you want
            //this method will be called when the player clicks on Done button in the settings screen
            PropertyInfo[] properties = typeof(TournamentXPSettings).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (newSettings.ContainsKey(property.Name))
                {
                    property.SetValue(TournamentXPSettings.Instance, newSettings[property.Name]);
                }
            }
        }


        #endregion

        internal const int OBJ_PRIZEPOOL = 4106000;
        internal const int OBJ_TOURNAMENT_TYPE_MELEE2 = 4106001;
        internal const int OBJ_TOURNAMENT_REWARD = 4106002;

        internal const int SAVEDEF_PRIZEPOOL = 4105000;
        internal const int SAVEDEF_TOURNAMENT_TYPE_MELEE2 = 4105001;
        internal const int SAVEDEF_TOURNAMENT_REWARD = 4105002;
    }
}