using System;

using TournamentsXPanded.Models;

using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXP.Addon.ModLib
{
    public static class SettingsHelperModLib
    {
        public static TournamentXPSettings GetModLibSettings()
        {
            try
            {
                var settings = TournamentsXPSettingsModLib.Instance.GetSettings();
                return settings;
            }
            catch (Exception ex)
            {
                ErrorLog.Log("TournamentsXPanded failed to initialize settings data.\n\n" + ex.ToStringFull());
            }
            return null;
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