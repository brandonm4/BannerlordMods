using SandBox;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace BrandonMod
{
    public class BrandonMod : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
           base.OnSubModuleLoad();
        }

        public override void OnCampaignStart(Game game, object initializerObject)
        {
//            Campaign gameType = game.GameType as Campaign;
 //           if (gameType != null)
            {

                //CampaignGameManager campaignGameManager = MBGameManager.Current as CampaignGameManager;
                //CampaignGameStarter campaignGameStarter = ((CampaignGameStarter)initializerObject);

                //var cvcb = campaignGameStarter.CampaignBehaviors.Where(x => x.GetType() == typeof(ClanVariablesCampaignBehavior)).FirstOrDefault();
                //campaignGameStarter.CampaignBehaviors.Remove(cvcb);
                //campaignGameStarter.AddBehavior(new BMClanVariablesCampaignBehavior());           
              //  gameType.SandBoxManager.SandBoxMissionManager = new BMSandBoxMissionManager();

            }
        }
       

      
        public override void OnGameLoaded(Game game, object initializerObject)
        {
            Campaign gameType = game.GameType as Campaign;
            if (gameType != null)
            {

                CampaignGameManager campaignGameManager = MBGameManager.Current as CampaignGameManager;
                CampaignGameStarter campaignGameStarter = ((CampaignGameStarter)initializerObject);

                //var cvcb = campaignGameStarter.CampaignBehaviors.Where(x => x.GetType() == typeof(ClanVariablesCampaignBehavior)).FirstOrDefault();
                //campaignGameStarter.CampaignBehaviors.Remove(cvcb);
                //campaignGameStarter.AddBehavior(new BMClanVariablesCampaignBehavior());

                //game.RemoveGameHandler<SandBoxManager>();
                //game.AddGameHandler<BMSandboxManager>();

                //SandBoxManager sandBoxManager = gameType.SandBoxManager;                
                //sandBoxManager.SandBoxMissionManager = new BMSandBoxMissionManager();
                //sandBoxManager.AgentBehaviorManager = new BMAgentBehaviorManager();
                //sandBoxManager.ModuleManager = new ModuleManager();
                //BMSandboxManager.OnGameStart(campaignGameStarter);

               // gameType.SandBoxManager.SandBoxMissionManager = new BMSandBoxMissionManager();

            }
        }


        //public override void OnMissionBehaviourInitialize(Mission mission)
        //{
        //    base.OnMissionBehaviourInitialize(mission);

        //    if (!mission.HasMissionBehaviour<BMExperienceOnHitLogic>() &&
        //            (mission.HasMissionBehaviour<TournamentArcheryMissionController>()
        //            || mission.HasMissionBehaviour<TournamentFightMissionController>()
        //            || mission.HasMissionBehaviour<TournamentJoustingMissionController>()
        //            || mission.HasMissionBehaviour<ArenaPracticeFightMissionController>()))
        //    {
        //        //mission.AddMissionBehaviour(new BMExperienceOnHitLogic());                
        //    }

        //    if (mission.HasMissionBehaviour<TournamentFightMissionController>())
        //    {
        //        //TournamentFightMissionController x = mission.GetMissionBehaviour<TournamentFightMissionController>();
        //        //BMTournamentFightMissionController a = x as BMTournamentFightMissionController;
        //        //mission.AddMissionBehaviour(a);
        //    }

            

        //}

        //public override bool DoLoading(Game game)
        //{

        //    return base.DoLoading(game);
        //}

        
    }

    public class BMClanVariablesCampaignBehavior : ClanVariablesCampaignBehavior
    {
        internal BMClanFinanceModel ClanFinanceModel { get; set; }

        public BMClanVariablesCampaignBehavior() : base()
        {
            ClanFinanceModel = new BMClanFinanceModel();
        }
              
        private void BMDailyTickHero(Hero hero)
        {
            if (hero.IsActive && (hero.IsNotable || hero.IsGangLeader))
            {
                GiveGoldAction.ApplyBetweenCharacters(null, hero, this.ClanFinanceModel.CalculateNotableDailyGoldChange(hero, true), true);
            }
        }
        private void BMDailyTickClan(Clan clan)
        {
            StatExplainer statExplainer;
            if (clan != CampaignData.NeutralFaction)
            {
                if (clan.Kingdom != null)
                {
                    foreach (Settlement settlement in clan.Settlements)
                    {
                        if (!settlement.IsTown)
                        {
                            continue;
                        }
                        if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoadTolls))
                        {
                            GiveGoldAction.ApplyBetweenCharacters(null, settlement.OwnerClan.Leader, (int)((float)settlement.Town.TradeTaxAccumulated * 0.03f), true);
                        }
                        if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.StateMonopolies))
                        {
                            GiveGoldAction.ApplyBetweenCharacters(null, settlement.OwnerClan.Leader, (int)((float)((IEnumerable<Workshop>)settlement.Town.Workshops).Sum<Workshop>((Workshop t) => t.ProfitMade) * 0.05f), true);
                        }
                        if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.DebasementOfTheCurrency))
                        {
                            GiveGoldAction.ApplyBetweenCharacters(null, settlement.OwnerClan.Leader, 100, true);
                        }
                        if (!clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CrownDuty))
                        {
                            continue;
                        }
                        GiveGoldAction.ApplyBetweenCharacters(null, settlement.OwnerClan.Leader, (int)((float)settlement.Town.TradeTaxAccumulated * 0.05f), true);
                    }
                }
                if (clan == Clan.PlayerClan)
                {
                    statExplainer = new StatExplainer();
                }
                else
                {
                    statExplainer = null;
                }
                StatExplainer statExplainer1 = statExplainer;
                GiveGoldAction.ApplyBetweenCharacters(null, clan.Leader, MathF.Floor(this.ClanFinanceModel.CalculateClanGoldChange(clan, statExplainer1, true)), true);
                Clan influence = clan;
                influence.Influence = influence.Influence + Campaign.Current.Models.ClanPoliticsModel.CalculateInfluenceChange(clan, null);
                if (clan.IsMinorFaction && clan.CommanderHeroes != null)
                {
                    clan.Leader.ChangeHeroGold(clan.CommanderHeroes.Count * 100);
                }
                if (statExplainer1 != null)
                {
                    float number = 0f;
                    float single = 0f;
                    foreach (StatExplainer.ExplanationLine line in statExplainer1.Lines)
                    {
                        if (line.Number <= 0f)
                        {
                            single += line.Number;
                        }
                        else
                        {
                            number += line.Number;
                        }
                    }
                    TextObject textObject = new TextObject("{=dPD5zood}Daily Gold Change: {CHANGE}{GOLD_ICON}", null);
                    float single1 = number + single;
                    textObject.SetTextVariable("CHANGE", single1);
                    textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"Icons\\Coin@2x\">");
                    InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), (single1 >= 0f ? "event:/ui/notification/coins_positive" : "event:/ui/notification/coins_negative")));
                }
            }
            this.UpdateGovernorsOfClan(clan);
        }

        private void UpdateGovernorsOfClan(Clan clan)
        {
            float single;
            List<Tuple<Town, float>> tuples = new List<Tuple<Town, float>>();
            foreach (Town fortification in clan.Fortifications)
            {
                float count = 0f;
                count = count + (float)((fortification.IsTown ? 3 : 1));
                count += (float)Math.Sqrt((double)(fortification.Prosperity / 1000f));
                count += (float)fortification.Settlement.BoundVillages.Count;
                count = count * (clan.Culture == fortification.Settlement.Culture ? 1f : 0.5f);
                if (clan.Leader.MapFaction.IsKingdomFaction)
                {
                    Vec2 position2D = fortification.Settlement.Position2D;
                    single = position2D.Distance(clan.Leader.MapFaction.FactionMidPoint);
                }
                else
                {
                    single = 100f;
                }
                float single1 = single;
                count = count * (1f - (float)Math.Sqrt((double)(single1 / Campaign.MapDiagonal)));
                tuples.Add(new Tuple<Town, float>(fortification, count));
            }
            int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
            List<Hero> heros = new List<Hero>();
            for (int i = 0; i < clan.Fortifications.Count; i++)
            {
                Tuple<Town, float> tuple = null;
                float item2 = 0f;
                foreach (Tuple<Town, float> tuple1 in tuples)
                {
                    if (tuple1.Item2 <= item2)
                    {
                        continue;
                    }
                    item2 = tuple1.Item2;
                    tuple = tuple1;
                }
                if (item2 > 0.01f)
                {
                    tuples.Remove(tuple);
                    float single2 = 0f;
                    Hero hero = null;
                    foreach (Hero hero1 in clan.Heroes)
                    {
                        if (hero1.PartyBelongedTo != null || !hero1.IsAlive || hero1.CharacterObject.Occupation != Occupation.Lord || hero1.Age <= (float)heroComesOfAge || hero1.IsOccupiedByAnEvent() || heros.Contains(hero1))
                        {
                            continue;
                        }
                        float heroGoverningStrengthForClan = (tuple.Item1.Governor == hero1 ? 1f : 0.75f) * Campaign.Current.Models.DiplomacyModel.GetHeroGoverningStrengthForClan(hero1);
                        if (heroGoverningStrengthForClan <= single2)
                        {
                            continue;
                        }
                        single2 = heroGoverningStrengthForClan;
                        hero = hero1;
                    }
                    if (hero != null)
                    {
                        if (tuple.Item1.Governor != hero)
                        {
                            if (hero.GovernorOf != null)
                            {
                                ChangeGovernorAction.ApplyByGiveUpCurrent(hero);
                            }
                            ChangeGovernorAction.Apply(tuple.Item1, hero);
                        }
                        heros.Add(hero);
                    }
                }
            }
        }
    }
}
