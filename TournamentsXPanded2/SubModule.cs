﻿using Newtonsoft.Json;
using SandBox;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common;
using TournamentsXPanded.Models;
using TournamentsXPanded.Settings;
using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TournamentsXPanded
{
    public class TournamentsXPandedSubModule : XPandedSubModuleBase
    {
        private static bool disabled = false;
        private static ApplicationVersion versionNative;
        private System.ComponentModel.BackgroundWorker menuChecker1;
        private string _id;
        private bool inMenu;

        protected override void OnSubModuleLoad()
        {
            var dirpath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName);
            try
            {
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create config directory.  Please manually create this directory: " + dirpath);
            }
            base.OnSubModuleLoad();

            bool mismatch = false;
            try
            {
                versionNative = ModuleInfo.GetModules().Where(x => x.Name == "Native").Select(x => new { x.Name, x.Version }).FirstOrDefault().Version;
#if VERSION130
                if (versionNative.Major == 1 && versionNative.Minor < 3)
                {
                    mismatch = true;
                }
#endif
#if VERSION120
                if (versionNative.Major == 1 && versionNative.Minor != 2)
                {
                    mismatch = true;
                }
#endif
                if (mismatch)
                {
                    MessageBox.Show("TournamentsXPanded Version Mismatch detected.\nInstall the correct one for your version of the game.\nGame Version: " + versionNative.Major + "." + versionNative.Minor + "." + versionNative.Revision);
                    disabled = true;
                }
                else
                {
                    _id = Guid.NewGuid().ToString();
                    menuChecker1 = new System.ComponentModel.BackgroundWorker();
                    var configPath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName);

                    if (SettingsHelper.LoadSettings(configPath))
                    {
                        if (TournamentXPSettings.Instance.TournamentEquipmentFilter)
                        {
                            CreateEquipmentRules();
                        }
                        //Add localizations
                        LocalizedTextManager.LoadLocalizationXmls();
                    }
                    else
                    {
                        disabled = true;
                        MessageBox.Show("TournamentXP had a critical failure during initialization.  Check your error logs.\nDisabling TournamentXP.");
                    }
                }
            }
            catch
            {
                disabled = true;
            }
        }
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            string version = typeof(TournamentsXPandedSubModule).Assembly.GetName().Version.ToString();
            if (!disabled)
            {
                if (!TournamentXPSettings.Instance.DebugMode)
                {
                    ShowMessage("Tournaments XPanded v" + version + " Loaded", Colors.Cyan);
                }
                else
                {
                    ShowMessage("Tournaments XPanded v" + version + " Loaded {DEBUG MODE}", Colors.Red);
                }

                inMenu = true;

                menuChecker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.menuChecker1_DoWork);
                menuChecker1.RunWorkerAsync();
            }
            else
            {
                ShowMessage("Tournaments XPanded v" + version + " Disabled", Colors.Red);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!disabled)
            {
                try
                {
                    inMenu = false;
                }
                catch
                { }

                if (game.GameType is Campaign)
                {
                    ApplyPatches(game, typeof(TournamentsXPandedSubModule), TournamentXPSettings.Instance.DebugMode);

                    CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;

                    if (campaignGameStarter != null)
                    {
                        campaignGameStarter.AddBehavior(new TournamentsXPandedBehavior());
                    }
                }
            }


        }
        public override void OnGameInitializationFinished(Game game)
        {
            if (!disabled)
            {
                if (game.GameType is Campaign)
                {
                    //Setup Custom Items.
                    string customfile = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "CustomPrizeItems.json");
                    if (File.Exists(customfile))
                    {
                        var configtxt = File.ReadAllText(customfile);
                        var customItems = JsonConvert.DeserializeObject<List<string>>(configtxt);
                        //InitCustomItems(customItems);
                        InitItemsList(customItems, out TournamentsXPandedBehavior._customItems);
                    }

                    //Setup Legacy Items
                    List<string> strArray = new List<string>() { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
                    InitItemsList(strArray, out TournamentsXPandedBehavior._legacyItems);

                }
            }
        }
        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            if (!disabled && !mission.HasMissionBehaviour<TournamentXPandedMatchBehavior>() &&
                 (mission.HasMissionBehaviour<TournamentArcheryMissionController>()
                 || mission.HasMissionBehaviour<TournamentJoustingMissionController>()
                 || mission.HasMissionBehaviour<TownHorseRaceMissionController>()
                 || mission.HasMissionBehaviour<TournamentFightMissionController>()
                 ))
            {
                if (TournamentXPSettings.Instance.BonusRenownFirstKill > 0
                    || TournamentXPSettings.Instance.BonusRenownLeastDamage > 0
                    || TournamentXPSettings.Instance.BonusRenownMostDamage > 0
                    || TournamentXPSettings.Instance.BonusRenownMostKills > 0)
                {
                    foreach (var t in TournamentsXPandedBehavior.Tournaments)
                    {
                        t.Active = false;
                    }
                    var tournamentInfo = TournamentsXPandedBehavior.GetTournamentInfo(Settlement.CurrentSettlement.Town);
                    tournamentInfo.Rewards = new TournamentReward();
                    tournamentInfo.Active = true;

                    var mb = mission.GetMissionBehaviour<TournamentBehavior>();
                    mission.AddMissionBehaviour(new TournamentXPandedMatchBehavior(mb));
                    mission.AddListener(new TournamentXPandedMatchListener(mb));
                }
            }
        }
        protected override void OnSubModuleUnloaded()
        {
            try
            {
                inMenu = false;
                menuChecker1.CancelAsync();
            }
            catch
            { }
        }
        private void CreateEquipmentRules()
        {
            try
            {
                var rulespath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "equipmentrules.json");
                if (File.Exists(rulespath))
                {
                    var rulestext = File.ReadAllText(rulespath);
                    TournamentsXPandedBehavior.EquipmentFilters = JsonConvert.DeserializeObject<List<ItemEquipmentFilter>>(rulestext);
                }
                else
                {
                    TournamentsXPandedBehavior.EquipmentFilters.Add(new ItemEquipmentFilter
                    {
                        TargetIgnoreMounted = true,
                        TargetCultureId = "khuzait",
                        TargetItemTypeString = "Polearm",
                        ReplacementAddHorse = true,
                        RuleOrder = 1,
                    });
                    TournamentsXPandedBehavior.EquipmentFilters.Add(new ItemEquipmentFilter
                    {
                        TargetIgnoreMounted = true,
                        TargetItemTypeString = "Polearm",
                        ReplacementItemTypeString = "OneHandedWeapon",
                        //ReplacementStringId = "sturgia_sword_1_t2",
                        RuleOrder = 5,
                    });

                    File.WriteAllText(rulespath,
                    JsonConvert.SerializeObject(TournamentsXPandedBehavior.EquipmentFilters, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    }));
                }

                foreach (var d in TournamentsXPandedBehavior.EquipmentFilters)
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
        private void menuChecker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker StayTheFuckAway = sender as BackgroundWorker;
            //int arg = (int)e.Argument;
            CheckMenu();
            if (StayTheFuckAway.CancellationPending)
            {
                e.Cancel = true;
            }
        }
        private void CheckMenu()
        {
            while (inMenu)
            {
                try
                {
                    var menu = Module.CurrentModule.GetInitialStateOptions().Where(x => x.Id == _id).FirstOrDefault();
                    if (menu == null)
                    {
                        //Stay the fuck away from my menu, k thx bye
                        Module.CurrentModule.AddInitialStateOption(new InitialStateOption(_id,
        new TextObject("TournamentXP Options", null),
        9990,
        () =>
        {
            //  ScreenManager.PushScreen(new ModOptionsGauntletScreen());
            var confpath = System.IO.Path.GetFullPath(System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetBasePath(), "Modules", ModuleFolderName, "bin", "Win64_Shipping_Client", "TournamentXPanded.Configurator.exe"));
            

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = confpath;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = "\"" + System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "tournamentxpsettings.json") + "\"";

            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
                SettingsHelper.LoadSettings(System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName));
                ShowWindow((int)Process.GetCurrentProcess().MainWindowHandle, SW_RESTORE);
            }
        },
        false));
                    }
                }
                catch { }
                Thread.Sleep(200);
            }
        }

        #region Windows
        private const int SW_HIDE = 0;
        private const int SW_RESTORE = 9;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);
        #endregion
    }
}