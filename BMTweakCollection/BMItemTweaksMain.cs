using BMTweakCollection.Models;
using BMTweakCollection.Patches;
using BMTweakCollection.Utility;
using HarmonyLib;
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
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TournamentLib;

namespace BMTweakCollection
{
    public class BMTweakCollectionMain : BMSubModuleBase
    {

        public static BMRandomTweaksConfiguration Configuration { get; set; }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            string appSettings = String.Concat(BasePath.Name, "Modules/BMTweakCollection/ModuleData/BMTweakCollection.config.json");
            var configtxt = File.ReadAllText(appSettings);
            BMTweakCollectionMain.Configuration = JsonConvert.DeserializeObject<BMRandomTweaksConfiguration>(configtxt);

            try
            {
                var h = new Harmony("com.darkspyre.bannerlord.tweakcol");
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

           // LootCollectorPatch.DoPatching();


        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            ShowMessage("Brandon's Tweak Collection Module Loaded");
        }
        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            FixPerkPeakForm();

            FixPerkHealthyScout();



        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (campaignGameStarter != null)
            {
             
            }
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

  
    }
}
