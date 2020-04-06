using BMTweakCollection.Models;
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

namespace BMTweakCollection
{
    public class BMTweakCollectionMain : MBSubModuleBase
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
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            FixPerkPeakForm();
            //FixPerkDisciplinarian();
            FixPerkHealthyScout();

            if (Configuration.RemoveAllEquippedHorses)
            {
                Campaign gameType = game.GameType as Campaign;
                if (gameType != null)
                {
                    foreach (var c in gameType.Characters)
                    {
                        if (c.HasMount())
                        {
                            //c.Equipment.GetEquipmentFromSlot(EquipmentIndex.Horse);
                        }
                    }
                }
            }

            //InformationManager.ShowInquiry(new InquiryData("Tweak Collection Enabled",
            //     "Tweak Collection Enabled",
            //     true, false, "Ok", "No", null, null, ""), false);
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (campaignGameStarter != null)
            {
                //if (BMTweakCollectionMain.Configuration.MaxHideoutTroopsEnabled)
                //{
                //    this.ReplaceTroopLimitModel(campaignGameStarter);
                //}
                //if (BMTweakCollectionMain.Configuration.CustomSmithingModelEnabled)
                //{
                //    this.ReplaceSmithingModel(campaignGameStarter);
                //}
            }
        }

        private bool ReplaceTroopLimitModel(CampaignGameStarter starter)
        {
            bool flag;
            IList<GameModel> models = starter.Models as IList<GameModel>;
            if (models != null)
            {
                int num = 0;
                while (num < models.Count)
                {
                    if (!(models[num] is DefaultTroopCountLimitModel))
                    {
                        num++;
                    }
                    else
                    {
                        models[num] = new CustomTroopLimitModel();
                        flag = true;
                        return flag;
                    }
                }
                flag = false;
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        private bool ReplaceSmithingModel(CampaignGameStarter starter)
        {
            bool flag;
            IList<GameModel> models = starter.Models as IList<GameModel>;
            if (models != null)
            {
                int num = 0;
                while (num < models.Count)
                {
                    if (!(models[num] is DefaultSmithingModel))
                    {
                        num++;
                    }
                    else
                    {
                        models[num] = new CustomSmithingModel();
                        flag = true;
                        return flag;
                    }
                }
                flag = false;
            }
            else
            {
                flag = false;
            }
            return flag;
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
