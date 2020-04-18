using ModLib;
using ModLib.GUI.ViewModels;

using System;
using System.Linq;

using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.TwoDimension;

using TournamentsXPanded.Models;

using XPanded.Common.Diagnostics;

namespace TournamentsXPanded.Settings
{
    public static class SettingsHelperModLib
    {
        public static TournamentXPSettingsModLib GetModLibSettings(bool forceDebug = false, bool forceMenu = false)
        {
            var modnames = Utilities.GetModulesNames().ToList();
            if (modnames.Contains("Bannerlord.MBOptionScreen"))
            {
                if (forceMenu)
                {
                    //Reenable ModLib settings menu option
                    Module.CurrentModule.AddInitialStateOption(new InitialStateOption("ModOptionsMenu", new TextObject("ModLib Options"), 9990, () =>
                    {
                        ScreenManager.PushScreen(new ModOptionsGauntletScreen());
                    }, false));
                }
            }

            try
            {
                FileDatabase.Initialise(SettingsHelper.ModuleFolderName);
                TournamentXPSettingsModLib settings = FileDatabase.Get<TournamentXPSettingsModLib>(TournamentXPSettingsModLib.InstanceID);
                if (settings == null) settings = new TournamentXPSettingsModLib();
                SettingsDatabase.RegisterSettings(settings);
                if (forceDebug)
                    settings.DebugMode = true;
                //        TournamentXPSettings.SetSettings(settings.GetSettings());
                return settings;
            }
            catch (Exception ex)
            {
                ErrorLog.Log("TournamentsXPanded failed to initialize settings data.\n\n" + ex.ToStringFull());
                return null;
            }
        }
    }

    internal class ModOptionsGauntletScreen : ScreenBase
    {
        private GauntletLayer gauntletLayer;
        private GauntletMovie movie;
        private ModSettingsScreenVM vm;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
            spriteData.SpriteCategories["ui_encyclopedia"].Load(resourceContext, uiresourceDepot);
            gauntletLayer = new GauntletLayer(1);
            gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
            gauntletLayer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(gauntletLayer);
            AddLayer(gauntletLayer);
            vm = new ModSettingsScreenVM();
            movie = gauntletLayer.LoadMovie("ModOptionsScreen", vm);
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            // || gauntletLayer.Input.IsGameKeyReleased(34)
            if (gauntletLayer.Input.IsHotKeyReleased("Exit"))
            {
                vm.ExecuteCancel();
            }
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            RemoveLayer(gauntletLayer);
            gauntletLayer.ReleaseMovie(movie);
            gauntletLayer = null;
            movie = null;
            vm.ExecuteSelect(null);
            vm.AssignParent(true);
            vm = null;
        }
    }
}