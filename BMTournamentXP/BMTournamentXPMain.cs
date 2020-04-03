using HarmonyLib;
using SandBox;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BMTournamentXP
{
    public class BMTournamentXPMain : MBSubModuleBase
    {
        private bool _enableTournamentXP = true;
        private bool _enableArenaXP = true;
        //private int _maximumTournamentBet = 300;
        private float _tournamentxpmod = 1;
        private float _arenaxpmod = 1;
        private bool _showpopup = false;
        private string _version = "e1.1.0";
        
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            XmlDocument xmlDocument = new XmlDocument();
            string appSettings = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/BMTournamentXP.config.xml");
            xmlDocument.Load(appSettings);
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("BMTournamentXP");
            foreach (XmlNode n in xmlNodes.ChildNodes)
            {
                switch (n.Name.Trim().ToLower())
                {
                    case "showinfopopup":                        
                        if (string.Equals(n.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            _showpopup = true;
                        }
                        else
                        {
                            _showpopup = false;
                        }
                        break;
                    case "tournament":
                        foreach (XmlNode nc in n.ChildNodes)
                        {
                            switch (nc.Name.Trim().ToLower())
                            {
                                case "enable":
                                    if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                    {
                                        _enableTournamentXP = true;
                                    }
                                    else
                                    {
                                        _enableTournamentXP = false;
                                    }
                                    break;
                                case "xpadjustment":
                                    float tadj = 1;
                                    float.TryParse(nc.InnerText, out tadj);
                                    _tournamentxpmod = tadj;
                                    break;
                                case "additionalgold":
                                    int bg = 0;
                                    int.TryParse(nc.InnerText, out bg);                                    
                                    //TournamentBehaviourPatch.bonusmoney = bg;
                                    break;
                                case "enablereroll":
                                    if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                    {
                                        TournamentCampaignBehaviorPatch1.IsEnabled = true;
                                    }
                                    else
                                    {
                                        TournamentCampaignBehaviorPatch1.IsEnabled = false;
                                    }
                                    break;
                            }
                        }
                        break;
                    case "arena":
                        foreach (XmlNode nc in n.ChildNodes)
                        {
                            switch (nc.Name.Trim().ToLower())
                            {
                                case "enable":
                                    if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                    {
                                        _enableArenaXP = true;
                                    }
                                    else
                                    {
                                        _enableArenaXP = false;
                                    }
                                    break;
                                case "xpadjustment":
                                    float tadj = 1;
                                    float.TryParse(nc.InnerText, out tadj);
                                    _arenaxpmod = tadj;
                                    break;                            
                            }
                        }
                        break;
                                                           
                    //case "maximumtournamentbet":
                    //    int bet = 300;
                    //    int.TryParse(n.InnerText, out bet);
                    //    _maximumTournamentBet = bet;
                    //    break;                                       
                }
            }

            try
            {
                var h = new Harmony("com.darkspyre.bannerlord.tournament");
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
                MessageBox.Show(string.Concat("Error patching:\n", str, " \n\n", message));
            }


        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            DisplayVersionInfo(_showpopup);
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);

            if (_enableArenaXP)
            {
                EnableArenaXP(mission);
            }
            if (_enableTournamentXP)
            {
                EnableTournamentXP(mission);
            }

            //if (!mission.HasMissionBehaviour<TournamentBehavior>())
            //{
            //    TournamentBehavior x = mission.GetMissionBehaviour<TournamentBehavior>();
            //    mission.RemoveMissionBehaviour(x);

            //    ITournamentGameBehavior gameBehavior = null;

            //    if(mission.HasMissionBehaviour<TournamentFightMissionController>())
            //    {
            //        gameBehavior = mission.GetMissionBehaviour<TournamentFightMissionController>();
            //    }
            //    if (mission.HasMissionBehaviour<TournamentJoustingMissionController>())
            //    {
            //        gameBehavior = mission.GetMissionBehaviour<TournamentJoustingMissionController>();
            //    }
            //    if (mission.HasMissionBehaviour<TournamentArcheryMissionController>())
            //    {
            //        gameBehavior = mission.GetMissionBehaviour<TournamentArcheryMissionController>();
            //    }
            //    if (mission.HasMissionBehaviour<TownHorseRaceMissionController>())
            //    {
            //        gameBehavior = mission.GetMissionBehaviour<TownHorseRaceMissionController>();
            //    }

            //    BMTournamentBehavior tb = new BMTournamentBehavior(x.TournamentGame, x.Settlement, gameBehavior, x.IsPlayerParticipating);
            //    tb.MaximumBet = _maximumTournamentBet;
            //    mission.AddMissionBehaviour(tb);
            //}

        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            //InformationManager.DisplayMessage(new InformationMessage(string.Concat("Tournament XP Enabled ", _enableTournamentXP.ToString(), ".")));
            //InformationManager.DisplayMessage(new InformationMessage(string.Concat("Arena XP Enabled ", _enableArenaXP.ToString(), ".")));
        }
        public override void OnGameLoaded(Game game, object initializerObject)
        {
           
        }
        public override void OnCampaignStart(Game game, object starterObject)
        {
            base.OnCampaignStart(game, starterObject);
        }

        private void EnableTournamentXP(Mission mission)
        {
            if (!mission.HasMissionBehaviour<BMExperienceOnHitLogic>() &&
              (mission.HasMissionBehaviour<TournamentFightMissionController>()
              || mission.HasMissionBehaviour<TournamentArcheryMissionController>()
              || mission.HasMissionBehaviour<TournamentJoustingMissionController>()
              || mission.HasMissionBehaviour<TownHorseRaceMissionController>()))
            {
                mission.AddMissionBehaviour(new BMExperienceOnHitLogic(_tournamentxpmod));

            }
        }
        private void EnableArenaXP(Mission mission)
        {
            if (!mission.HasMissionBehaviour<BMExperienceOnHitLogic>() &&
              mission.HasMissionBehaviour<ArenaPracticeFightMissionController>())
            {
                mission.AddMissionBehaviour(new BMExperienceOnHitLogic(_arenaxpmod));
            }
        }

        private void DisplayVersionInfo(bool showpopup)
        {
            string t = "Yes";
            string a = "Yes";
            if (!_enableTournamentXP)
            {
                t = "No";
            }
            if (!_enableArenaXP)
            {
                a = "No";
            }
            string info = String.Concat("Tournament Patch v", _version, " Loaded\n", "Tournament XP Enabled:\t", t, "\n",
                    "Tournament XP Amount:\t", (100 * _tournamentxpmod).ToString(), "%\n",
                    "Arena XP Enabled:\t", a, "\n",
                    "Arena XP Amount:\t", (100 * _arenaxpmod).ToString(), "%\n"
                    );

            if (showpopup)
            {
                InformationManager.ShowInquiry(new InquiryData("Tournament XP",
                    info,
                    true, false, "Ok", "No", null, null, ""), false);
            }
            else
            {
                InformationManager.DisplayMessage(new InformationMessage(info));
            }
        }
    }
}
