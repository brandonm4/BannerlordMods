using BMTweakCollection.LootTweaks;
using BMTweakCollection.Models;
using BMTweakCollection.Patches;
using BMTweakCollection.Utility;
using HarmonyLib;
using ModLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TournamentsXPanded;

namespace BMTweakCollection
{
    public partial class BMTweakCollectionSubModule : MBSubModuleBase
    {


        public static string ModuleFolderName { get; } = "BMTweakCollection";

        protected override void OnSubModuleLoad()
        {
            if (File.Exists(System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs")))
            {
                File.Delete(System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs"));
            }
            string logpath = System.IO.Path.Combine(TaleWorlds.Engine.Utilities.GetConfigsPath(), ModuleFolderName, "Logs", "logfile.txt");
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(logpath)))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logpath));
            }
            ErrorLog.LogPath = logpath;




            try
            {
                FileDatabase.Initialise(ModuleFolderName);
                BMRandomTweaksConfiguration settings = FileDatabase.Get<BMRandomTweaksConfiguration>(BMRandomTweaksConfiguration.InstanceID);
                if (settings == null) settings = new BMRandomTweaksConfiguration();
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
            base.OnGameInitializationFinished(game);
            //FixPerkPeakForm();
            //FixPerkHealthyScout();

            //Configuration.MainPartySkillMods = new Dictionary<SkillObject, float>();
            //Configuration.MainPartySkillMods.Add(DefaultSkills.Charm, 3.0f);
            //Configuration.MainPartySkillMods.Add(DefaultSkills.Leadership, 3.0f);
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (campaignGameStarter != null)
            {
                // ItemTweaks.MakeCivilianSaddles(12);
            }
            try
            {
                //LootCollectorPatch.DoPatching();
                //var _harmony = new Harmony("com.darkspyre.bannerlord.tweakcol");
                //  _harmony.PatchAll();

                LootCollectorPatch.DoPatching();
                BMTweakCollectionSubModule.Harmony.PatchAll();
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
        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            mission.AddMissionBehaviour(new LootBehavior());
        }


        private void FixPerkPeakForm()
        {
            var _perk = PerkObject.FindFirst(x => x.Name.GetID() == "fBgGbxaw");
            if (_perk.PrimaryBonus == 0f)
            {
                var textObjStrings = TextObject.ConvertToStringList(
                   new List<TextObject> {
                  _perk.Name,
                  _perk.Description
                   }
                );

                // most of the properties of skills have private setters, yet Initialize is public
                _perk.Initialize(
                  textObjStrings[0],
                  textObjStrings[1],
                  _perk.Skill,
                  (int)_perk.RequiredSkillValue,
                  _perk.AlternativePerk,
                  _perk.PrimaryRole, 10f,
                  _perk.SecondaryRole, _perk.SecondaryBonus,
                  _perk.IncrementType
                );
            }
        }

        private void FixPerkDisciplinarian()
        {
            PerkObject _perk
        = PerkObject.FindFirst(x => x.Name.GetID() == "ER3ieXOb");
            if (_perk.PrimaryRole == SkillEffect.PerkRole.Personal)
            {
                var textObjStrings = TextObject.ConvertToStringList(
            new List<TextObject> {
          _perk.Name,
          _perk.Description
            }
          );

                // most of the properties of skills have private setters, yet Initialize is public
                _perk.Initialize(
                  textObjStrings[0],
                  textObjStrings[1],
                  _perk.Skill,
                  (int)_perk.RequiredSkillValue,
                  _perk.AlternativePerk,
                  SkillEffect.PerkRole.PartyLeader, 0f,
                  _perk.PrimaryRole, _perk.PrimaryBonus,
                  _perk.IncrementType
                );
            }
        }

        private void FixPerkHealthyScout()
        {
            PerkObject _perk
      = PerkObject.FindFirst(x => x.Name.GetID() == "dDKOoD3e");

            if (_perk.PrimaryRole == SkillEffect.PerkRole.PartyMember
        && _perk.PrimaryBonus == 0.15f)
            {
                var textObjStrings = TextObject.ConvertToStringList(
           new List<TextObject> {
          _perk.Name,
          _perk.Description
           }
         );

                // most of the properties of skills have private setters, yet Initialize is public
                _perk.Initialize(
                  textObjStrings[0],
                  textObjStrings[1],
                  _perk.Skill,
                  (int)_perk.RequiredSkillValue,
                  _perk.AlternativePerk,
                  SkillEffect.PerkRole.Personal, 8f,
                  _perk.SecondaryRole, _perk.SecondaryBonus,
                  _perk.IncrementType
                );
            }
        }
        public static void ShowMessage(string msg, Color? color = null)
        {
            if (color == null)
                color = Color.White;

            InformationManager.DisplayMessage(new InformationMessage(msg, (Color)color));
        }

    }
}
