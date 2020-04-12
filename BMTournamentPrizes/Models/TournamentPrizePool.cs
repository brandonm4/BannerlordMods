using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using TournamentLib.Models;

namespace TournamentsXPanded.Models
{

    /* If you are viewing this for ideas on how to do things, don't do this 
    /* adding a savable class to the MBObjectManager makes the game unloadable ifyou remove the mod from the game 
    /* you're better off managing your data in SyncData in your behavior 
    /* However, if you are also inherting and adding classes that were SaveableClasses - like I do for the new TournamentTypes, those must also be SaveableClasses, 
     * so in this particular case, I felt it was justifiable.
     * I haven't been able to succesfully deregister my classes to make for a /clean/ save yet.  
     */
    [SaveableClass(TournamentsXPandedSubModule.OBJ_PRIZEPOOL)]
    public class TournamentPrizePool : MBObjectBase
    {
        [SaveableProperty(300)]
        public ItemRoster Prizes { get; set; }
        [SaveableProperty(100)]
        public string SelectedPrizeStringId { get; set; }
        [SaveableProperty(200)]
        public int RemainingRerolls { get; set; }
        [SaveableProperty(20)]
        public Town Town
        {
            get;
            set;
        }

        public ItemRosterElement SelectPrizeItemRosterElement
        {
            get
            {
                return Prizes.Where(x => x.EquipmentElement.Item.StringId == SelectedPrizeStringId).FirstOrDefault();
            }
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
        public TournamentPrizePoolSaveableTypeDefiner() : base(TournamentsXPandedSubModule.SAVEDEF_PRIZEPOOL)
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
