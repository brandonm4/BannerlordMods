using Newtonsoft.Json;
using SandBox;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common;
using TournamentsXPanded.Models;

using TournamentsXPanded.Settings;

using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded
{
    public partial class TournamentsXPandedSubModule : XPandedSubModuleBase
    {
        private List<TournamentEquipmentRestrictor> restrictors = new List<TournamentEquipmentRestrictor>();
        private bool disabled = false;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            var version = ModuleInfo.GetModules().Where(x => x.Name == "Native").Select(x => new { x.Name, x.Version }).FirstOrDefault().Version;
            bool mismatch = false;
#if VERSION120
            if (version.Major == 1 && version.Minor < 2)
            {
                mismatch = true;
            }
#endif
#if VERSION111
            if (version.Major == 1 && version.Minor > 1)
            {
                mismatch = true;
            }
            if (version.Major > 1)
            {
            mismatch = true;
            }
#endif
            if (mismatch)
            {
                MessageBox.Show("TournamentsXPanded Version Mismatch detected.\nInstall the correct one for your version of the game.\nGame Version: " + version.Major + "." + version.Minor + "." + version.Revision);
            }
            SettingsHelper.LoadSettings();

            //Setup Item Filters if needed
            if (TournamentXPSettings.Instance.TournamentEquipmentFilter)
            {
                CreateEquipmentRules();
            }

            //Add localizations
            LocalizedTextManager.LoadLocalizationXmls();

#if DEBUG
            TournamentXPSettings.Instance.DebugMode = true;
#endif
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            string version = typeof(TournamentsXPandedSubModule).Assembly.GetName().Version.ToString();

            if (!TournamentXPSettings.Instance.DebugMode)
            {
                ShowMessage("Tournaments XPanded v" + version + " Loaded", Colors.Cyan);
            }
            else
            {
                ShowMessage("Tournaments XPanded v" + version + " Loaded {DEBUG MODE}", Colors.Red);
            }
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

        public override void OnGameInitializationFinished(Game game)
        {
            if (game.GameType is Campaign)
            {
                ApplyPatches(game);
                if (TournamentXPSettings.Instance.TournamentEquipmentFilter)
                {
                    string[] _weaponTemplatesIdTeamSizeOne = new string[] { "tournament_template_aserai_one_participant_set_v1", "tournament_template_battania_one_participant_set_v1", "tournament_template_battania_one_participant_set_v2", "tournament_template_empire_one_participant_set_v1", "tournament_template_khuzait_one_participant_set_v1", "tournament_template_khuzait_one_participant_set_v2", "tournament_template_vlandia_one_participant_set_v1", "tournament_template_vlandia_one_participant_set_v2", "tournament_template_vlandia_one_participant_set_v3", "tournament_template_sturgia_one_participant_set_v1", "tournament_template_sturgia_one_participant_set_v2" };

                    string[] _weaponTemplatesIdTeamSizeTwo = new string[] { "tournament_template_aserai_two_participant_set_v1", "tournament_template_aserai_two_participant_set_v2", "tournament_template_aserai_two_participant_set_v3", "tournament_template_battania_two_participant_set_v1", "tournament_template_battania_two_participant_set_v2", "tournament_template_battania_two_participant_set_v3", "tournament_template_battania_two_participant_set_v4", "tournament_template_battania_two_participant_set_v5", "tournament_template_empire_two_participant_set_v1", "tournament_template_empire_two_participant_set_v2", "tournament_template_empire_two_participant_set_v3", "tournament_template_khuzait_two_participant_set_v1", "tournament_template_khuzait_two_participant_set_v2", "tournament_template_khuzait_two_participant_set_v3", "tournament_template_vlandia_two_participant_set_v1", "tournament_template_vlandia_two_participant_set_v2", "tournament_template_vlandia_two_participant_set_v3", "tournament_template_vlandia_two_participant_set_v4", "tournament_template_sturgia_two_participant_set_v1", "tournament_template_sturgia_two_participant_set_v2", "tournament_template_sturgia_two_participant_set_v3" };

                    string[] _weaponTemplatesIdTeamSizeFour = new string[] { "tournament_template_aserai_four_participant_set_v1", "tournament_template_aserai_four_participant_set_v2", "tournament_template_aserai_four_participant_set_v3", "tournament_template_aserai_four_participant_set_v4", "tournament_template_battania_four_participant_set_v1", "tournament_template_battania_four_participant_set_v2", "tournament_template_battania_four_participant_set_v3", "tournament_template_empire_four_participant_set_v1", "tournament_template_empire_four_participant_set_v2", "tournament_template_empire_four_participant_set_v3", "tournament_template_khuzait_four_participant_set_v1", "tournament_template_khuzait_four_participant_set_v2", "tournament_template_khuzait_four_participant_set_v3", "tournament_template_vlandia_four_participant_set_v1", "tournament_template_vlandia_four_participant_set_v2", "tournament_template_vlandia_four_participant_set_v3", "tournament_template_vlandia_four_participant_set_v4", "tournament_template_sturgia_four_participant_set_v1", "tournament_template_sturgia_four_participant_set_v2", "tournament_template_sturgia_four_participant_set_v3" };

                    RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeOne);
                    RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeTwo);
                    RemoveTournamentSpearFootSets(_weaponTemplatesIdTeamSizeFour);
                }

                //Setup Custom Items.
                string customfile = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "CustomPrizeItems.json");
                if (File.Exists(customfile))
                {
                    var configtxt = File.ReadAllText(customfile);
                    var customItems = JsonConvert.DeserializeObject<List<string>>(configtxt);
                    //InitCustomItems(customItems);
                    InitItemsList(customItems, out TournamentPrizePoolBehavior._customItems);
                }

                //Setup Legacy Items
                List<string> strArray = new List<string>() { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
                InitItemsList(strArray, out TournamentPrizePoolBehavior._legacyItems);

                if (TournamentXPSettings.Instance.DebugMode)
                {
                    try
                    {
                        CreateDiagnostics();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("ERROR CREATING DIAGNOSTICS\n" + ex.ToStringFull());
                    }
                }

                //Try to repair
                foreach (var settlement in Settlement.All)
                {
                    if (settlement.HasTournament)
                    {
                        var tournament = Campaign.Current.TournamentManager.GetTournamentGame(settlement.Town) as Fight2TournamentGame;
                        try
                        {
                            if (tournament != null)
                            {
                                tournament.SetFightMode(Fight2TournamentGame.FightMode.Mixed);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Log("Error repairing tournament: " + settlement.Town.StringId);
                            ErrorLog.Log(ex.ToStringFull());
                        }
                    }
                }
            }
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            if (!mission.HasMissionBehaviour<TournamentXPandedTournamentBehavior>() &&
             (mission.HasMissionBehaviour<TournamentArcheryMissionController>()
             || mission.HasMissionBehaviour<TournamentJoustingMissionController>()
             || mission.HasMissionBehaviour<TownHorseRaceMissionController>()
             || mission.HasMissionBehaviour<TournamentFightMissionController>()
             ))
            {
                mission.AddMissionBehaviour(new TournamentXPandedTournamentBehavior());
            }
        }

        #region Local Methods
        protected void CreateDiagnostics()
        {
            var diag = "Tournaments XPanded Settings infomation\n";
            string configPath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "tournamentxpdiagnostics.log");
            if (!Directory.Exists(Path.GetDirectoryName(configPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configPath));
            }
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            try
            {
                var settingsjson = JsonConvert.SerializeObject(TournamentXPSettings.Instance, serializerSettings);
                diag += settingsjson;
                diag += "\n\n\nCustom Item List";
                diag += JsonConvert.SerializeObject(TournamentPrizePoolBehavior.CustomTournamentItems.Select(x => x.StringId), serializerSettings);
                diag += "\n\n\nAll stored prize pools\n";
            }
            catch (Exception ex)
            {
                diag += "ERROR SERIALIZING SETTINGS\n";
                diag += ex.ToStringFull() + "\n";
            }
            try
            {
                List<TournamentPrizePool> allPools = new List<TournamentPrizePool>();
                MBObjectManager.Instance.GetAllInstancesOfObjectType<TournamentPrizePool>(ref allPools);

                var cleanList = allPools.Select(x => new { x.Id, x.SelectedPrizeStringId, x.RemainingRerolls, TownStringId = x.Town.StringId, PrizeCount = x.Prizes.Count });
                diag += JsonConvert.SerializeObject(cleanList, serializerSettings);

                //   List<string> towns = new List<string>();
            }
            catch (Exception ex)
            {
                diag += "ERROR SERIALIZING PRIZEPOOLS\n";
                diag += ex.ToStringFull() + "\n";
            }
            File.WriteAllText(configPath, diag);
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
            foreach (var r in restrictors.OrderBy(x => x.RuleOrder))
            {
                foreach (var t in templates)
                {
                    var template = MBObjectManager.Instance.GetObject<CharacterObject>(t);
                    if (!string.IsNullOrWhiteSpace(r.TargetCultureId))
                    {
                        if (template.Culture.StringId != r.TargetCultureId)
                        {
                            continue;
                        }
                    }
                    try
                    {
                        foreach (Equipment battleEquipment in template.BattleEquipments)
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
                        MessageBox.Show("TournamentsXPanded\nError Applying Rule Equipement Filter\n" + ex.ToStringFull());
                        ErrorLog.Log("Error Applying Rule Equipement Filter\n" + ex.ToStringFull());
                    }
                }
            }

        }
        private void CreateEquipmentRules()
        {
            try
            {
                var rulespath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "equipmentrules.json");
                if (File.Exists(rulespath))
                {
                    var rulestext = File.ReadAllText(rulespath);
                    restrictors = JsonConvert.DeserializeObject<List<TournamentEquipmentRestrictor>>(rulestext);
                }
                else
                {
                    restrictors.Add(new TournamentEquipmentRestrictor
                    {
                        TargetIgnoreMounted = true,
                        TargetCultureId = "khuzait",
                        TargetItemTypeString = "Polearm",
                        ReplacementAddHorse = true,
                        RuleOrder = 1,
                    });
                    //Eventually plan to let people define their own
                    restrictors.Add(new TournamentEquipmentRestrictor
                    {
                        TargetIgnoreMounted = true,
                        TargetItemTypeString = "Polearm",
                        ReplacementItemTypeString = "OneHandedWeapon",
                        //ReplacementStringId = "sturgia_sword_1_t2",
                        RuleOrder = 5,
                    });

                    File.WriteAllText(rulespath,
                    JsonConvert.SerializeObject(restrictors, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    }));
                }

                foreach (var d in restrictors)
                {
                    if (!string.IsNullOrWhiteSpace(d.TargetItemTypeString))
                        d.TargetItemType = (ItemObject.ItemTypeEnum)Enum.Parse(typeof(ItemObject.ItemTypeEnum), d.TargetItemTypeString);
                    if (!string.IsNullOrWhiteSpace(d.ReplacementItemTypeString))
                        d.ReplacementItemType = (ItemObject.ItemTypeEnum)Enum.Parse(typeof(ItemObject.ItemTypeEnum), d.ReplacementItemTypeString);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("TournamentsXPanded\nError Processing Rule Equipement Filter\n" + ex.ToStringFull());
                ErrorLog.Log("Error Processing Rule Equipement Filter\n" + ex.ToStringFull());
            }
        }
        private void InitItemsList(List<string> itemStringIds, out List<ItemObject> itemList)
        {
            itemList = new List<ItemObject>();
            List<string> problemids = new List<string>();

            foreach (var id in itemStringIds)
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
                    ErrorLog.Log(string.Concat("WARNING: Tournament Prize System\n", "Invalid Item Id detected in prize list.  Please remove from the list.  Ignoring problem item and continuing.\n\n", id));
                }
                else
                {
                    // TournamentPrizePoolBehavior.CustomTournamentItems.Add(item);
                    itemList.Add(item);
                }
            }
            if (problemids.Count > 0)
            {
                string info = "Errors in Custom Prize List.\nReview list and correct or remove these entries:\n";
                foreach (var p in problemids)
                {
                    info = string.Concat(info, p, "\n");
                }

                MessageManager.DisplayDebugMessage(info);
                //InformationManager.ShowInquiry(new InquiryData("Tournament Prize Errors",
                //    info,
                //    true, false, "Ok", "No", null, null, ""), false);
            }
        }
        #endregion
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

        #endregion ModSettings

        internal const int OBJ_PRIZEPOOL = 4106000;
        internal const int OBJ_TOURNAMENT_TYPE_MELEE2 = 4106001;
        internal const int OBJ_TOURNAMENT_REWARD = 4106002;

        internal const int SAVEDEF_PRIZEPOOL = 4105000;
        internal const int SAVEDEF_TOURNAMENT_TYPE_MELEE2 = 4105001;
        internal const int SAVEDEF_TOURNAMENT_REWARD = 4105002;
    }
}