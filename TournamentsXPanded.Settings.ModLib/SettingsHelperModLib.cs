using ModLib;

using System;
using System.IO;
using System.Reflection;
using TournamentsXPanded.Models;

using XPanded.Common.Diagnostics;

namespace TournamentsXPanded.Settings
{
    public static class SettingsHelperModLib
    {
        public static TournamentXPSettings GetModLibSettings(bool forceDebug = false)
        {
            try
            {
                var settings = GetSettingsFromModLib(forceDebug);
                return settings;
            }
            catch (Exception ex)
            {
                //Modlib failed to initialize - try loading it from MBOptionScreen instead

                var modlibsettingsdll = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetBasePath(), "Modules", "Bannerlord.MBOptionScreen", "bin", "Win64_Shipping_Client", "ModLib.dll");
                modlibsettingsdll = System.IO.Path.GetFullPath(modlibsettingsdll);
                if (File.Exists(modlibsettingsdll))
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(modlibsettingsdll);
                        var settings = GetSettingsFromModLib(forceDebug);
                        return settings;
                    }
                    catch(Exception ex2)
                    {
                        ErrorLog.Log("TournamentsXPanded failed to initialize settings data.\n\n" + ex2.ToStringFull());
                    }
                }
                ErrorLog.Log("TournamentsXPanded failed to initialize settings data.\n\n" + ex.ToStringFull());
            }
            return null;
        }

        internal static TournamentXPSettings GetSettingsFromModLib(bool forceDebug = false)
        {
            FileDatabase.Initialise(SettingsHelper.ModuleFolderName);
            TournamentXPSettingsModLib settings = FileDatabase.Get<TournamentXPSettingsModLib>(TournamentXPSettingsModLib.InstanceID);
            if (settings == null)
            {
                settings = new TournamentXPSettingsModLib();
            }
            SettingsDatabase.RegisterSettings(settings);
            if (forceDebug)
            {
                settings.DebugMode = true;
            }            
            return settings.GetSettings();
        }
    }

    //internal class ModOptionsGauntletScreen : ScreenBase
    //{
    //    private GauntletLayer gauntletLayer;
    //    private GauntletMovie movie;
    //    private ModSettingsScreenVM vm;

    //    protected override void OnInitialize()
    //    {
    //        base.OnInitialize();
    //        SpriteData spriteData = UIResourceManager.SpriteData;
    //        TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
    //        ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
    //        spriteData.SpriteCategories["ui_encyclopedia"].Load(resourceContext, uiresourceDepot);
    //        gauntletLayer = new GauntletLayer(1);
    //        gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
    //        gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
    //        gauntletLayer.IsFocusLayer = true;
    //        ScreenManager.TrySetFocus(gauntletLayer);
    //        AddLayer(gauntletLayer);
    //        vm = new ModSettingsScreenVM();
    //        movie = gauntletLayer.LoadMovie("ModOptionsScreen", vm);
    //    }

    //    protected override void OnFrameTick(float dt)
    //    {
    //        base.OnFrameTick(dt);
    //        // || gauntletLayer.Input.IsGameKeyReleased(34)
    //        if (gauntletLayer.Input.IsHotKeyReleased("Exit"))
    //        {
    //            vm.ExecuteCancel();
    //        }
    //    }

    //    protected override void OnFinalize()
    //    {
    //        base.OnFinalize();
    //        RemoveLayer(gauntletLayer);
    //        gauntletLayer.ReleaseMovie(movie);
    //        gauntletLayer = null;
    //        movie = null;
    //        vm.ExecuteSelect(null);
    //        vm.AssignParent(true);
    //        vm = null;
    //    }
    //}
}