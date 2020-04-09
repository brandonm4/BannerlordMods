using BannerLib.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TournamentLib;

namespace GrandTheftHorse
{
    public class GrandTheftHorseSubmodule : BMSubModuleBase
    {
        internal static bool InBattle = false;
        protected override void OnSubModuleLoad()
        {
            // Create a new HotKeyManager for your mod.
            var hkm = BannerLib.Input.HotKeyManager.Create("GrandTheftHorse");
            // Add your HotKeyBase derived class to the manager.
            // You can add as many hotkeys as you'd like before building them up.
            // You can also use `hkm.Add(new TestKey(SomeExampleArgument))` if you'd like to have a non-default constructor.
            var rslt = hkm.Add<TestKey>();
            // It's not necessary to supply a predicate, it's just a convenience.
            // You can also manually set IsEnabled to more simply enable/disable a keys functionality.
            rslt.Predicate = () => InBattle;
            // Subscribe to each of the events on the hotkey at any time.
            rslt.OnReleasedEvent += () =>
                InformationManager.DisplayMessage(new InformationMessage("Test Key Released!", Colors.Magenta));
            // Call this to build up all the hotkeys you added.
            hkm.Build();
        }
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            ShowMessage("Grand Theft Horse Module Loaded");
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            // An example just to demonstrate functionality.
           // if (game.GameType is Campaign) InBattle = true;
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);
        }

        public class TestKey : HotKeyBase
        {
            public TestKey() : base(nameof(TestKey))
            {
                DisplayName = "My Test Key";
                Description = "This is a test key.";
                DefaultKey = InputKey.Comma;
                Category = BannerLib.Input.HotKeyManager.Categories[HotKeyCategory.CampaignMap];
            }

            protected override void OnReleased()
            {
                var m = Mission.Current;
               if (Agent.Main != null && Agent.Main.IsActive())
                {
                    var targetAgent = Mission.Current.GetAgentsInRange(Agent.Main.Position.AsVec2, 10f).Where(x => x.IsEnemyOf(Agent.Main) && x.HasMount).FirstOrDefault();
                    if (targetAgent != null)
                    {
                        
                    }
                }
            }
        }
    }
}
