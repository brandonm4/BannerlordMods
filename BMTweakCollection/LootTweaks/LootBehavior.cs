using BMTweakCollection.Helpers;
using BMTweakCollection.Models;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TournamentsXPanded;

namespace BMTweakCollection.LootTweaks
{
    public class LootBehavior : MissionLogic, IBattleSignaller, IMissionBehavior
    {

        public static List<EquipmentElement> LootedItems { get; set; } = new List<EquipmentElement>();
        public IBattleObserver BattleObserver
        {
            get;
            private set;
        }
        private float PlayerDropRate { get; set; }
        private float PlayerMaxValue
        {
            get
            {
                return Hero.MainHero.Level * 500 + BMRandomTweaksConfiguration.Instance.BaseMaxValue;
            }
        }
        private CharacterObject PartyLooter { get; set; }

        //Move these to config
     

        public override void AfterStart()
        {
            LootedItems = new List<EquipmentElement>();

            ExplainedNumber explainedNumber = new ExplainedNumber();
            explainedNumber.Add(BMRandomTweaksConfiguration.Instance.BaseDropRate);

            PartyLooter = BMHelpers.CharacterHelpers.GetCharacterWithHighestSkill(MobileParty.MainParty.Party, DefaultSkills.Roguery);
            SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Roguery, DefaultSkillEffects.RogueryLootBonus, PartyLooter, ref explainedNumber, true);

            PlayerDropRate = explainedNumber.ResultNumber;
        }
        public override void OnBattleEnded()
        {
            PartyLooter.HeroObject.AddSkillXp(DefaultSkills.Roguery, BMRandomTweaksConfiguration.Instance.LootSkillXpGain);
        }
        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            if (Mission.Current.CombatType != Mission.MissionCombatType.Combat)
                return;
            try
            {

                if ((affectedAgent.IsMainAgent == false && !affectedAgent.IsMount))
                {
                    //Don't get your companion equipment
                    if (affectedAgent.IsHero && affectedAgent.Team == Agent.Main.Team)
                    {
                        return;
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        EquipmentIndex equipmentIndex = (EquipmentIndex)i;
                        var rdm = MBRandom.RandomFloatRanged(BMRandomTweaksConfiguration.Instance.LootRandomMaxRoll);
                        EquipmentElement equipmentFromSlot;
                        if (rdm < PlayerDropRate) //Might as well use the player loot rate.  If player looses they get no loot anyway.
                        {
                            if (affectedAgent.Character.Equipment.GetEquipmentFromSlot(equipmentIndex).Item != null)
                            {
                                equipmentFromSlot = affectedAgent.Character.Equipment.GetEquipmentFromSlot(equipmentIndex);
                                if (!BMRandomTweaksConfiguration.Instance.HorseDropEnabled)
                                {
                                    if (equipmentFromSlot.Item.ItemType == ItemObject.ItemTypeEnum.HorseHarness
                                        || equipmentFromSlot.Item.ItemType == ItemObject.ItemTypeEnum.Horse)
                                    {
                                        continue;
                                    }
                                }
                                if (equipmentFromSlot.Item.Value > PlayerMaxValue)
                                {
                                    typeof(ItemObject).GetProperty("Value").SetValue(equipmentFromSlot.Item, PlayerMaxValue);
                                }
                                LootedItems.Add(equipmentFromSlot);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log("Error OnAgentRemoval");
                ErrorLog.Log(ex.ToStringFull());
            }
        }
        public void SetObserver(IBattleObserver observer)
        {
            this.BattleObserver = observer;
        }
    }
}
