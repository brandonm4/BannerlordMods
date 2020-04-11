using HarmonyLib;

using SandBox;
using SandBox.TournamentMissions.Missions;

using System;
using System.IO;
using System.Windows.Forms;

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

using TournamentLib;
using TournamentLib.Models;

namespace BMTournamentXP
{
    public class BMTournamentXPMain : BMSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            //Load config file
            string appSettings = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/BMTournament.config.xml");

            if (File.Exists(appSettings))
            {
                //Configuration = new BMTournamentXPConfiguration(appSettings);
                TournamentConfiguration.Instance.LoadXML(appSettings);
            }

            try
            {
                var h = new Harmony("com.darkspyre.bannerlord.xp");
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
                MessageBox.Show(string.Concat("Tournament XP Error patching:\n", str, " \n\n", message));
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);          
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            ShowMessage("Tournament XPerience XP Module Loaded");
        }
            
        private void EnableTournamentXP(Mission mission)
        {
            if (!mission.HasMissionBehaviour<BMExperienceOnHitLogic>() &&
              (mission.HasMissionBehaviour<TournamentArcheryMissionController>()
              || mission.HasMissionBehaviour<TournamentJoustingMissionController>()
              || mission.HasMissionBehaviour<TownHorseRaceMissionController>()))
            {
                mission.AddMissionBehaviour(new BMExperienceOnHitLogic(TournamentConfiguration.Instance.XPConfiguration.TournamentXPAdjustment));
            }
        }

        //private void EnableArenaXP(Mission mission)
        //{
        //    if (!mission.HasMissionBehaviour<BMExperienceOnHitLogic>() &&
        //      mission.HasMissionBehaviour<ArenaPracticeFightMissionController>())
        //    {
        //        mission.AddMissionBehaviour(new BMExperienceOnHitLogic(TournamentConfiguration.Instance.XPConfiguration.ArenaXPAdjustment));
        //    }
        //}

        //private void DisplayVersionInfo(bool showpopup)
        //{
        //    string t = "Yes";
        //    string a = "Yes";
        //    if (!TournamentConfiguration.Instance.XPConfiguration.IsTournamentXPEnabled)
        //    {
        //        t = "No";
        //    }
        //    if (!TournamentConfiguration.Instance.XPConfiguration.IsArenaXPEnabled)
        //    {
        //        a = "No";
        //    }

        //    string info = String.Concat("Tournament Patch v", BMTournamentXPMain.Version, " Loaded\n", "Tournament XP Enabled:\t", t, "\n",
        //            "Tournament XP Amount:\t", (100 * TournamentConfiguration.Instance.XPConfiguration.TournamentXPAdjustment).ToString(), "%\n",
        //            "Arena XP Enabled:\t", a, "\n",
        //            "Arena XP Amount:\t", (100 * TournamentConfiguration.Instance.XPConfiguration.ArenaXPAdjustment).ToString(), "%\n"
        //            );

        //    if (showpopup)
        //    {
        //        InformationManager.ShowInquiry(new InquiryData("Tournament XP",
        //            info,
        //            true, false, "Ok", "No", null, null, ""), false);
        //    }
        //    else
        //    {
        //        InformationManager.DisplayMessage(new InformationMessage(info));
        //    }
        //}        
    }
}