using BMTournamentXP.Models;
using HarmonyLib;
using Newtonsoft.Json;
using SandBox;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static BMTournamentXPConfiguration Configuration { get; set; } = new BMTournamentXPConfiguration();

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            //Load config file
            string appSettings = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/BMTournamentXP.config.xml");
            if (File.Exists(appSettings))
            {
                Configuration = new BMTournamentXPConfiguration(appSettings);
            }

            //Load tournament items
            string tourneyitemsfile = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/BMTournamentPrizeList.json");           
            if (BMTournamentXPMain.Configuration.PrizeListMode.Trim().IndexOf("custom", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (File.Exists(tourneyitemsfile))
                {
                    var configtxt = File.ReadAllText(tourneyitemsfile);
                    Configuration.TourneyItems = JsonConvert.DeserializeObject<List<string>>(configtxt);
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

            DisplayVersionInfo(false);

            Campaign campaign = ((Campaign)game.GameType);
            if (campaign != null)
            {

                //   IList<CharacterObject> characters = campaign.Characters as IList<CharacterObject>;
                //if (characters != null)

            }
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);

            if (BMTournamentXPMain.Configuration.IsArenaXPEnabled)
            {
                EnableArenaXP(mission);
            }
            if (BMTournamentXPMain.Configuration.IsTournamentXPEnabled)
            {
                EnableTournamentXP(mission);
            }
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
                mission.AddMissionBehaviour(new BMExperienceOnHitLogic(BMTournamentXPMain.Configuration.TournamentXPAdjustment));

            }
        }
        private void EnableArenaXP(Mission mission)
        {
            if (!mission.HasMissionBehaviour<BMExperienceOnHitLogic>() &&
              mission.HasMissionBehaviour<ArenaPracticeFightMissionController>())
            {
                mission.AddMissionBehaviour(new BMExperienceOnHitLogic(BMTournamentXPMain.Configuration.ArenaXPAdjustment));
            }
        }

        private void DisplayVersionInfo(bool showpopup)
        {
            string t = "Yes";
            string a = "Yes";
            if (!BMTournamentXPMain.Configuration.IsTournamentXPEnabled)
            {
                t = "No";
            }
            if (!BMTournamentXPMain.Configuration.IsArenaXPEnabled)
            {
                a = "No";
            }
            string info = String.Concat("Tournament Patch v", BMTournamentXPConfiguration.Version, " Loaded\n", "Tournament XP Enabled:\t", t, "\n",
                    "Tournament XP Amount:\t", (100 * BMTournamentXPMain.Configuration.TournamentXPAdjustment).ToString(), "%\n",
                    "Arena XP Enabled:\t", a, "\n",
                    "Arena XP Amount:\t", (100 * BMTournamentXPMain.Configuration.ArenaXPAdjustment).ToString(), "%\n"
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

        private void CorruptedCharFix(Campaign campaign)
        {
            {
                foreach (var c in campaign.Characters)
                {
                    if (c.IsHero && c.HeroObject != null)
                    {
                        if (c.HeroObject.IsWanderer && c.HeroObject.CompanionOf != Hero.MainHero.Clan)
                        {
                            //Set non-companions that are wanderers back to stock
                            //The problems chars have IsArcher, IsInfantry and IsMounted as Exception - not null, true or false.  Basically just trying to access to force an exception, then murdering the char.
                            var bHadIssue = false;
                            try
                            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning disable RCS1166 // Value type object is never equal to null.
                                if (c.IsArcher == null)
#pragma warning restore RCS1166 // Value type object is never equal to null.
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                                {
                                 //   typeof(CharacterObject).GetProperty("IsArcher").SetValue(c, false);
                                }
                            }
                            catch
                            {
                          //      typeof(CharacterObject).GetProperty("IsArcher").SetValue(c, false);
                                bHadIssue = true;
                            }
                            try
                            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning disable RCS1166 // Value type object is never equal to null.
                                if (c.IsMounted == null)
#pragma warning restore RCS1166 // Value type object is never equal to null.
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                                {
                                //    typeof(CharacterObject).GetProperty("IsMounted").SetValue(c, false);
                                }
                            }
                            catch
                            {
                             //   typeof(CharacterObject).GetProperty("IsMounted").SetValue(c, false);
                                bHadIssue = true;
                            }
                            try
                            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning disable RCS1166 // Value type object is never equal to null.
                                if (c.IsInfantry == null)
#pragma warning restore RCS1166 // Value type object is never equal to null.
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                                {
                               //     typeof(CharacterObject).GetProperty("IsInfantry").SetValue(c, false);
                                }
                            }
                            catch
                            {
                             //   typeof(CharacterObject).GetProperty("IsInfantry").SetValue(c, false);
                                bHadIssue = true;
                            }

                            if (bHadIssue)
                            {
                                //   Traverse.Create(c.HeroObject).Method("SetInitialValuesFromCharacter").GetValue(new object[] { c });
                                //Murder the char
                                //c.HeroObject.IsDead = true;
                            }

                        }
                    }
                }
            }
        }
    }
}
