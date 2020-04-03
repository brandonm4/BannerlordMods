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



            Campaign gameType = game.GameType as Campaign;
            if (gameType != null)
            {
                //if (BMTweakCollectionMain.Configuration.ItemMultiplayerToSinglePlayerEnabled)
                //{
                //    foreach (ItemObject i in gameType.Items)
                //    {
                //        if (!i.NotMerchandise)
                //        {
                //            typeof(ItemObject).GetProperty("MultiplayerItem").SetValue(i, false);
                //        }
                //    }
                //}
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
    }
}
