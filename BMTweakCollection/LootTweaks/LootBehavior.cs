using BMTweakCollection.Helpers;
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
        private float PlayerMaxValue {
            get
            {
                return Hero.MainHero.Level * 500 + BaseMaxValue;
            }
        }
        private CharacterObject PartyLooter { get; set; }

        //Move these to config
        public bool HorseDropEnabled { get; set; } = true;
        public float BaseDropRate { get; set; } = 30;
        public float BaseMaxValue { get; set; } = 5000;
        
        public override void AfterStart()
        {
            LootedItems = new List<EquipmentElement>();

            ExplainedNumber explainedNumber = new ExplainedNumber();
            explainedNumber.Add(BaseDropRate);

            PartyLooter = BMHelpers.CharacterHelpers.GetCharacterWithHighestSkill(MobileParty.MainParty.Party, DefaultSkills.Roguery);
            SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Roguery, DefaultSkillEffects.RogueryLootBonus, PartyLooter, ref explainedNumber, true);
           
            PlayerDropRate = explainedNumber.ResultNumber;
        }
        public override void OnBattleEnded()
        {
            PartyLooter.HeroObject.AddSkillXp(DefaultSkills.Roguery, 10f);
        }
        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            if (Mission.Current.CombatType != Mission.MissionCombatType.Combat)
                return;
            
            {
                if ((affectedAgent.IsMainAgent == false
                    && !affectedAgent.IsMount))
                {
                    if (affectedAgent.IsHero)
                    {
                        var characterObject = (CharacterObject)affectedAgent.Character;
                        if (characterObject != null && characterObject.HeroObject != null && HeroHelper.IsCompanionInPlayerParty(characterObject.HeroObject))
                        {
                            return;
                        }
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        EquipmentIndex equipmentIndex = (EquipmentIndex)i;
                        var rdm = MBRandom.RandomFloatRanged(110f);
                        EquipmentElement equipmentFromSlot;
                        if (rdm < PlayerDropRate) //Might as well use the player loot rate.  If player looses they get no loot anyway.
                        {
                            if (affectedAgent.Character.Equipment.GetEquipmentFromSlot(equipmentIndex).Item != null)
                            {
                                equipmentFromSlot = affectedAgent.Character.Equipment.GetEquipmentFromSlot(equipmentIndex);
                                if (!HorseDropEnabled)
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
        }
        public void SetObserver(IBattleObserver observer)
        {
            this.BattleObserver = observer;
        }
    }
}
