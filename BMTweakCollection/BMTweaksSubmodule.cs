using BMTweakCollection.Behaviors;
using BMTweakCollection.LootTweaks;
using BMTweakCollection.Models;
using BMTweakCollection.Patches;
using ModLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using TournamentsXPanded;
using TournamentsXPanded.Common;
using XPanded.Common.Diagnostics;

namespace BMTweakCollection
{
    public partial class BMTweakCollectionSubModule : XPandedSubModuleBase
    {
        public static bool disabled = false;
        public static new string ModuleFolderName { get; } = "BMTweakCollection";

        protected override void OnSubModuleLoad()
        {        
            try
            {
                FileDatabase.Initialise(ModuleFolderName);
                BMRandomTweaksConfiguration settings = FileDatabase.Get<BMRandomTweaksConfiguration>(BMRandomTweaksConfiguration.InstanceID);
                if (settings == null)
                {
                    settings = new BMRandomTweaksConfiguration();
                }

                SettingsDatabase.RegisterSettings(settings);
            }
            catch (Exception ex)
            {
                ErrorLog.Log("TournamentsXPanded failed to initialize settings data.\n\n" + ex.ToStringFull());
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            ShowMessage("Brandon's Tweak Collection Module Loaded", Colors.Green);
        }

        public override void OnGameInitializationFinished(Game game)
        {
            if (game.GameType is Campaign)
            {
                ApplyPatches(game, typeof(BMTweakCollectionSubModule));
            }
        }       
        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            if (mission.CombatType == Mission.MissionCombatType.Combat)
                mission.AddMissionBehaviour(new LootBehavior());
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!disabled)
            {
                if (game.GameType is Campaign)
                {
                    CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
                    
                    if (campaignGameStarter != null)
                    {
                        campaignGameStarter.AddBehavior(new FixedCompanionSkillsBehavior());
                    }
                }
            }
        }
    }
}