using BMTournamentPrizes.Behaviors;
using BMTournamentPrizes.Models;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TournamentLib;
using TournamentLib.Models;

namespace BMTournamentPrizes
{
    public class BMTournamentPrizesMain : BMSubModuleBase
    {      
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
          

        }
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            ShowMessage("Tournament XPerience Prize Module Loaded");

        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {            
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
                //gameStarterObject.AddModel(new TournamentPrizeExpansion());              
                MBObjectManager.Instance.RegisterType<TournamentPrizePool>("TournamentPrizePool", "TournamentPrizePools", true);                
                if (campaignGameStarter != null)
                {
                    campaignGameStarter.AddBehavior(new TournamentPrizePoolBehavior());
                }

                try
                {
                    var h = new Harmony("com.darkspyre.bannerlord.tournamentprizes");
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
                    MessageBox.Show(string.Concat("Tournament XP Prizes Error patching:\n", str, " \n\n", message));
                }
            }
        }
        public override void OnGameInitializationFinished(Game game)
        {
            //Campaign gameType = game.GameType as Campaign;
            //if (gameType != null)
            //{
            //    TournamentPoolInitialization(gameType);
            //}
        }       
    }
}
