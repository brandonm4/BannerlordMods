using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using TournamentLib.Models;

namespace BMTournamentPrizes.Models
{
    //[SaveableClass(4106000)]
    public class TournamentPrizePool : MBObjectBase
    {
      // [SaveableProperty(300)]
        public ItemRoster Prizes { get; set; } 
    // [SaveableProperty(100)]
        public string SelectedPrizeStringId { get; set; } 
     // [SaveableProperty(200)]
        public int RemainingRerolls { get; set; } 
     // [SaveableProperty(20)]
        public Town Town
        {
            get;
            set;
        }
        public TournamentPrizePool()
        {
            Prizes = new ItemRoster();
            SelectedPrizeStringId = "";
            RemainingRerolls = TournamentConfiguration.Instance.PrizeConfiguration.MaxNumberOfRerollsPerTournament;            
        }
        public TournamentPrizePool(int rerolls = -1)
        {
            Prizes = new ItemRoster();
            SelectedPrizeStringId = "";
            RemainingRerolls = TournamentConfiguration.Instance.PrizeConfiguration.MaxNumberOfRerollsPerTournament;
            if (rerolls >= 0)
            {
                RemainingRerolls = rerolls;
            }            
        }

    }


    public class TournamentPrizePoolSaveableTypeDefiner : SaveableTypeDefiner
    {
        public TournamentPrizePoolSaveableTypeDefiner() : base(4105000)
        {
        }

        protected override void DefineClassTypes()
        {
            base.AddClassDefinition(typeof(TournamentPrizePool), 1);
        }

        protected override void DefineContainerDefinitions()
        {
            base.ConstructContainerDefinition(typeof(List<TournamentPrizePool>));
            base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, TournamentPrizePool>));
            base.ConstructContainerDefinition(typeof(Dictionary<string, TournamentPrizePool>));
        }

        protected override void DefineGenericClassDefinitions()
        {
            base.ConstructGenericClassDefinition(typeof(MBObjectManager.ObjectTypeRecord<TournamentPrizePool>));
        }
    }
}
