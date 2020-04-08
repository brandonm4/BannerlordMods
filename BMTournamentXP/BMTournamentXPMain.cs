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
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TournamentLib.Models;

namespace BMTournamentXP
{
    public class BMTournamentXPMain : MBSubModuleBase
    {
        public static string Version { get { return "e1.2.9"; } }

        internal static void ShowMessage(string msg)
        {
            InformationManager.DisplayMessage(new InformationMessage(msg));
        }

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
                MessageBox.Show(string.Concat("Error patching:\n", str, " \n\n", message));
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            DisplayVersionInfo(false);

            //Campaign campaign = ((Campaign)game.GameType);
            //if (campaign != null)
            //{

            //    CorruptedCharFix(campaign);

            //}
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            ShowMessage("Tournament XPerience XP Module Loaded");
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);
           
            //if (TournamentConfiguration.Instance.XPConfiguration.IsTournamentXPEnabled)
            //{
            //    //EnableTournamentXP(mission);
            //}
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            //Need to find a better way
            if (TournamentConfiguration.Instance.XPConfiguration.IsTournamentXPEnabled || TournamentConfiguration.Instance.XPConfiguration.IsArenaXPEnabled)
            {
               // gameStarterObject.AddModel(new TournamentCombatXpModel());  /* Harmony Patch against DefaultCombatXpModel is failing for some reason */
            }

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

        private void DisplayVersionInfo(bool showpopup)
        {
            string t = "Yes";
            string a = "Yes";
            if (!TournamentConfiguration.Instance.XPConfiguration.IsTournamentXPEnabled)
            {
                t = "No";
            }
            if (!TournamentConfiguration.Instance.XPConfiguration.IsArenaXPEnabled)
            {
                a = "No";
            }


            string info = String.Concat("Tournament Patch v", BMTournamentXPMain.Version, " Loaded\n", "Tournament XP Enabled:\t", t, "\n",
                    "Tournament XP Amount:\t", (100 * TournamentConfiguration.Instance.XPConfiguration.TournamentXPAdjustment).ToString(), "%\n",
                    "Arena XP Enabled:\t", a, "\n",
                    "Arena XP Amount:\t", (100 * TournamentConfiguration.Instance.XPConfiguration.ArenaXPAdjustment).ToString(), "%\n"
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

        //        private void CorruptedCharFix(Campaign campaign)
        //        {
        //            {
        //                foreach (var c in campaign.Characters)
        //                {
        //                    if (c.IsHero && c.HeroObject != null)
        //                    {
        //                        if (c.HeroObject.IsWanderer && c.HeroObject.CompanionOf != Hero.MainHero.Clan)
        //                        {
        //                            //Set non-companions that are wanderers back to stock
        //                            //The problems chars have IsArcher, IsInfantry and IsMounted as Exception - not null, true or false.  Basically just trying to access to force an exception, then murdering the char.
        //                            var bHadIssue = false;
        //                            try
        //                            {
        //#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        //#pragma warning disable RCS1166 // Value type object is never equal to null.
        //                                if (c.IsArcher == null)
        //#pragma warning restore RCS1166 // Value type object is never equal to null.
        //#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        //                                {
        //                                 //   typeof(CharacterObject).GetProperty("IsArcher").SetValue(c, false);
        //                                }
        //                            }
        //                            catch
        //                            {
        //                          //      typeof(CharacterObject).GetProperty("IsArcher").SetValue(c, false);
        //                                bHadIssue = true;
        //                            }
        //                            try
        //                            {
        //#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        //#pragma warning disable RCS1166 // Value type object is never equal to null.
        //                                if (c.IsMounted == null)
        //#pragma warning restore RCS1166 // Value type object is never equal to null.
        //#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        //                                {
        //                                //    typeof(CharacterObject).GetProperty("IsMounted").SetValue(c, false);
        //                                }
        //                            }
        //                            catch
        //                            {
        //                             //   typeof(CharacterObject).GetProperty("IsMounted").SetValue(c, false);
        //                                bHadIssue = true;
        //                            }
        //                            try
        //                            {
        //#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        //#pragma warning disable RCS1166 // Value type object is never equal to null.
        //                                if (c.IsInfantry == null)
        //#pragma warning restore RCS1166 // Value type object is never equal to null.
        //#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        //                                {
        //                               //     typeof(CharacterObject).GetProperty("IsInfantry").SetValue(c, false);
        //                                }
        //                            }
        //                            catch
        //                            {
        //                             //   typeof(CharacterObject).GetProperty("IsInfantry").SetValue(c, false);
        //                                bHadIssue = true;
        //                            }

        //                            if (bHadIssue)
        //                            {
        //                                //   Traverse.Create(c.HeroObject).Method("SetInitialValuesFromCharacter").GetValue(new object[] { c });
        //                                //Murder the char
        //                                //c.HeroObject.IsDead = true;
        //                                c.HeroObject.AlwaysDie = true;

        //                                //    c.HeroObject.ChangeState(Hero.CharacterStates.Dead);
        //                                //KillCharacterAction.ApplyByRemove(c.HeroObject, true);
        //                                if (c.HeroObject.CurrentSettlement != null)
        //                                {
        //                                    Traverse.Create(c.HeroObject.CurrentSettlement).Method("RemoveHero").GetValue(new object[] { c.HeroObject });
        //                                }

        //                                ApplyInternal(c.HeroObject, null, KillCharacterAction.KillCharacterActionDetail.Lost, true);

        //                                if(c.HeroObject.PartyBelongedTo != null)
        //                                {

        //                                }
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        private static void ApplyInternal(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail actionDetail, bool showNotification)
        //        {     
        //            if (!victim.IsAlive)
        //            {
        //                return;
        //            }            
        //            //victim.EncyclopediaText = KillCharacterAction.CreateObituary(victim, actionDetail);
        //            victim.EncyclopediaText = (TextObject)Traverse.Create(typeof(KillCharacterAction)).Method("CreateObituary").GetValue(new object[] { victim, actionDetail });
        //            //KillCharacterAction.MakeDead(victim, true);
        //            Traverse.Create(typeof(KillCharacterAction)).Method("MakeDead").GetValue(new object[] { victim, true });
        //            victim.MakeWounded(actionDetail);
        //        }
    }
}
