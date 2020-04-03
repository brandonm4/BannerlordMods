using SandBox;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            XmlDocument xmlDocument = new XmlDocument();
            string appSettings = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/BMTournamentXP.config.xml");
            xmlDocument.Load(appSettings);
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("BMTournamentXP");
            foreach(XmlNode n in xmlNodes.ChildNodes)
            {
                switch (n.Name.ToLower())
                {
                    case "enabletournamentxp":
                        if (string.Equals(n.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            _enableTournamentXP = true;
                        }
                        else
                        {
                            _enableTournamentXP = false;
                        }
                        break;
                    case "enablearenaxp":
                        if (string.Equals(n.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            _enableArenaXP = true;
                        }
                        else
                        {
                            _enableArenaXP = false;
                        }
                        break;

                }

            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            InformationManager.DisplayMessage(new InformationMessage(string.Concat("Tournament XP Enabled ", _enableTournamentXP.ToString(), ".")));
            InformationManager.DisplayMessage(new InformationMessage(string.Concat("Arena XP Enabled ", _enableArenaXP.ToString(), ".")));

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

        }

        private void EnableTournamentXP(Mission mission)
        {
            if (!mission.HasMissionBehaviour<BMExperienceOnHitLogic>() &&
              (mission.HasMissionBehaviour<TournamentFightMissionController>()
              || mission.HasMissionBehaviour<TournamentArcheryMissionController>()
              || mission.HasMissionBehaviour<TournamentJoustingMissionController>()
              || mission.HasMissionBehaviour<TownHorseRaceMissionController>()))
            {
                mission.AddMissionBehaviour(new BMExperienceOnHitLogic());
            }
        }
        private void EnableArenaXP(Mission mission)
        {
            if (!mission.HasMissionBehaviour<BMExperienceOnHitLogic>() &&
              mission.HasMissionBehaviour<ArenaPracticeFightMissionController>())
            {
                mission.AddMissionBehaviour(new BMExperienceOnHitLogic());
            }
        }
    }
}
