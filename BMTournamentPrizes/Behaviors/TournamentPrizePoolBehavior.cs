using BMTournamentPrizes.Extensions;
using BMTournamentPrizes.Models;
using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TournamentLib.Extensions;
using TournamentLib.Models;

namespace BMTournamentPrizes.Behaviors
{
    public class TournamentPrizePoolBehavior : CampaignBehaviorBase
    {
        public TournamentPrizePoolBehavior() { }

        public static TournamentPrizePool GetSettlementPrizePool(string settlementStringId)
        {
            return GetTournamentPrizePool(Campaign.Current.Settlements.Where(x => x.StringId == settlementStringId).Single());
        }
        public static TournamentPrizePool GetTournamentPrizePool(Settlement settlement)
        {
            Town component = settlement.Town;
            TournamentPrizePool obj = MBObjectManager.Instance.GetObject<TournamentPrizePool>((TournamentPrizePool x) => x.Town == component);
            if (obj == null)
            {
                obj = MBObjectManager.Instance.CreateObject<TournamentPrizePool>();
                obj.Town = component;
            }
            return obj;
        }
        public static void ClearTournamentPrizes(string settlement_string_id)
        {
            ClearTournamentPrizes(Campaign.Current.Settlements.Where(x => x.StringId == settlement_string_id).Single());
        }
        public static void ClearTournamentPrizes(Settlement settlement)
        {
            var currentPool = GetTournamentPrizePool(settlement);
            currentPool.Prizes = new ItemRoster();
            currentPool.SelectedPrizeStringId = "";
            currentPool.RemainingRerolls = TournamentConfiguration.Instance.PrizeConfiguration.MaxNumberOfRerollsPerTournament;
        }
        #region Events
        private void OnAfterNewGameCreated(CampaignGameStarter starter)
        {
            var text = new TextObject("Re-roll Prize"); //Was going to put the remaining count, but not updating correctly.
            starter.AddGameMenuOption("town_arena", "bm_reroll_tournamentprize", text.ToString(),
                new GameMenuOption.OnConditionDelegate(RerollCondition),
                new GameMenuOption.OnConsequenceDelegate(RerollConsequence),
                false, -1, true);

            starter.AddGameMenuOption("town_arena", "bm_select_tournamentprize", "Select Prize",
             new GameMenuOption.OnConditionDelegate(PrizeSelectCondition),
             new GameMenuOption.OnConsequenceDelegate(PrizeSelectConsequence), false, -1, true);

            starter.AddGameMenuOption("town_arena", "bm_select_tournamenttype", "Select Tournament Style",
             new GameMenuOption.OnConditionDelegate(TournamentTypeSelectCondition),
             new GameMenuOption.OnConsequenceDelegate(TournamentTypeSelectConsequence), false, -1, true);

        }
        #endregion

        #region Prizes

        public static ItemObject GenerateTournamentPrize(TournamentGame tournamentGame, TournamentPrizePool currentPool = null, bool keepTownPrize = true)
        {
            ItemObject prize;
            var numItemsToGet = TournamentConfiguration.Instance.PrizeConfiguration.NumberOfPrizeOptions;
            bool bRegenAllPrizes = false;
            if (currentPool == null)
            {
                bRegenAllPrizes = true;
                currentPool = GetTournamentPrizePool(tournamentGame.Town.Settlement);
            }

            //Get the town items if using that mode
            List<string> townitems = new List<string>();
            if (TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode == PrizeListMode.TownCustom
                || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode == PrizeListMode.TownVanilla
                || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode == PrizeListMode.TownOnly)
            {
                townitems = GetValidTownItems(tournamentGame, TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMin, TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMax, TournamentConfiguration.Instance.PrizeConfiguration.TownValidPrizeTypes);
            }

            //Now get the list items - either customized or vanilla system
            List<string> listItems = new List<string>();
            if (TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode == PrizeListMode.Custom
                || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode == PrizeListMode.TownCustom)
            {
                listItems = TournamentConfiguration.Instance.PrizeConfiguration.CustomTourneyItems;
            }
            else if (TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode == PrizeListMode.TownVanilla
                || TournamentConfiguration.Instance.PrizeConfiguration.PrizeListMode == PrizeListMode.Vanilla)
            {
                listItems = GetVanillaSetOfPrizes(tournamentGame.Town.Settlement, numItemsToGet);
            }

            //Now concat them together to get full list.
            var allitems = townitems.Concat(listItems).ToList();

            //Add any existing items if we are filling in missing ones from an already generated pool
            var pickeditems = new List<string>();
            if (keepTownPrize)
            {
                pickeditems.Add(tournamentGame.Prize.StringId);
                currentPool.SelectedPrizeStringId = tournamentGame.Prize.StringId;
            }
            try
            {
                if (!bRegenAllPrizes)
                {
                    foreach (ItemRosterElement existingPrize in currentPool.Prizes)
                    {
                        if (!pickeditems.Contains(existingPrize.EquipmentElement.Item.StringId))
                            pickeditems.Add(existingPrize.EquipmentElement.Item.StringId);
                        if (allitems.Contains(existingPrize.EquipmentElement.Item.StringId))
                            allitems.Remove(existingPrize.EquipmentElement.Item.StringId);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: GetTournamentPrize existingprizes\n" + ex.ToStringFull());
            }
            if (allitems.Count() < numItemsToGet)
            {
                numItemsToGet = allitems.Count();
            }

            while (pickeditems.Count < numItemsToGet && allitems.Count() > 0)
            {
                var randomId = allitems.GetRandomElement<string>();

                if (!pickeditems.Contains(randomId))
                {
                    pickeditems.Add(randomId);
                    allitems.Remove(randomId);
                }
            }
            currentPool.Prizes = new ItemRoster();
            foreach (var id in pickeditems)
            {
                ItemModifier imod = new ItemModifier();  //Maybe make a random itemmodifer generator thingy                
                currentPool.Prizes.Add(new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>(id), 1, null));
            }

            if (!keepTownPrize)
            {
                var selected = currentPool.Prizes.GetRandomElement<ItemRosterElement>();
                currentPool.SelectedPrizeStringId = selected.EquipmentElement.Item.StringId;
                SetTournamentSelectedPrize(tournamentGame, selected.EquipmentElement.Item);
            }
            return Game.Current.ObjectManager.GetObject<ItemObject>(currentPool.SelectedPrizeStringId);
        }
        internal static List<string> GetValidTownItems(TournamentGame tournamentGame, int minValue, int maxValue, List<ItemObject.ItemTypeEnum> validtypes)
        {
            var roster = tournamentGame.Town.Owner.ItemRoster;
            roster.RemoveZeroCounts();
            var list = roster.Where(x =>
            x.Amount > 0
            && validtypes.Contains(x.EquipmentElement.Item.ItemType)
           && x.EquipmentElement.Item.Value >= minValue
           && x.EquipmentElement.Item.Value <= maxValue
              ).Select(x => x.EquipmentElement.Item.StringId).ToList();

            if (list.Count == 0)
            {
                list = roster.Where(x =>
                    x.Amount > 0
                    && validtypes.Contains(x.EquipmentElement.Item.ItemType))
                   .Select(x => x.EquipmentElement.Item.StringId).ToList();
            }
            if (list.Count == 0)
            {
                list = TournamentConfiguration.Instance.PrizeConfiguration.CustomTourneyItems;
            }
            if (list.Count == 0)
            {
                MessageBox.Show("Tournament Prize System", "Warning: The current town has no prizes available with your current defined filter.  Defaulting to Vanilla items.");
                //list = PrizeConfiguration.StockTourneyItems;
                list = GetVanillaSetOfPrizes(tournamentGame.Town.Settlement, TournamentConfiguration.Instance.PrizeConfiguration.NumberOfPrizeOptions);
            }
            return list;
        }
        public static ItemObject GetTournamentPrizeVanilla(Settlement settlement)
        {
            float minValue = 1000f;
            float maxValue = 5000f;

            if (TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell)
            {
                minValue = TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMin;
                maxValue = TournamentConfiguration.Instance.PrizeConfiguration.TownPrizeMax;
            }

            string[] strArray = new String[] { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };

            ItemObject obj = Game.Current.ObjectManager.GetObject<ItemObject>(strArray.GetRandomElement<string>());
            ItemObject itemObject = MBRandom.ChooseWeighted<ItemObject>(ItemObject.All, (ItemObject item) =>
            {
                if ((float)item.Value > minValue * (item.IsMountable ? 0.5f : 1f))
                {
                    if (TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeTypeFilterToLists)
                    {
                        if ((float)item.Value < maxValue * (item.IsMountable ? 0.5f : 1f) && item.Culture == settlement.Town.Culture &&
                            TournamentConfiguration.Instance.PrizeConfiguration.TownValidPrizeTypes.Contains(item.ItemType)
                        )
                        {
                            return 1f;
                        }
                    }
                    else if ((float)item.Value < maxValue * (item.IsMountable ? 0.5f : 1f) && item.Culture == settlement.Town.Culture && (item.IsCraftedWeapon || item.IsMountable || item.ArmorComponent != null))
                    {
                        return 1f;
                    }

                }
                return 0f;
            }) ?? MBRandom.ChooseWeighted<ItemObject>(ItemObject.All, (ItemObject item) =>
            {
                if ((float)item.Value > minValue * (item.IsMountable ? 0.5f : 1f))
                {
                    if ((float)item.Value < maxValue * (item.IsMountable ? 0.5f : 1f) && (item.IsCraftedWeapon || item.IsMountable || item.ArmorComponent != null))
                    {
                        return 1f;
                    }
                }
                return 0f;
            });
            if (itemObject == null)
            {
                return obj;
            }
            return itemObject;
        }
        public static List<string> GetVanillaSetOfPrizes(Settlement settlement, int count)
        {
            List<string> prizes = new List<string>();
            int retryMax = 50;
            int currentTry = 0;
            while (prizes.Count < count && currentTry < retryMax)
            {
                var stringid = GetTournamentPrizeVanilla(settlement).StringId;
                if (!prizes.Contains(stringid))
                {
                    prizes.Add(stringid);
                }
                currentTry++;
            }
            return prizes;
        }
        public static void SetTournamentSelectedPrize(TournamentGame tournamentGame, ItemObject prize)
        {
            typeof(TournamentGame).GetProperty("Prize").SetValue(tournamentGame, prize);
        }
        #endregion

        #region Menu Entries

        private static bool RerollCondition(MenuCallbackArgs args)
        {
            if (!TournamentConfiguration.Instance.PrizeConfiguration.TournamentPrizeRerollEnabled)
            {
                return false;
            }
            bool flag;
            TextObject textObject;
            TournamentPrizePool settings = GetTournamentPrizePool(Settlement.CurrentSettlement);
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);

            if (settings.RemainingRerolls <= 0)
            {
                flag = true;
                flag1 = false;
                textObject = new TextObject("Re-roll Attempts Exceeded");
            }
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
        }
        public static void RerollConsequence(MenuCallbackArgs args)
        {
            try
            {
                TournamentPrizePool settings = GetTournamentPrizePool(Settlement.CurrentSettlement);
                TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);

                //Clear old prizes
                settings.SelectedPrizeStringId = "";
                settings.Prizes = new ItemRoster();
                settings.RemainingRerolls--;

                //Generate New Prize
                var prize = GenerateTournamentPrize(tournamentGame, settings, false);
                SetTournamentSelectedPrize(tournamentGame, prize);


                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: Re-roll: Refreshing Arena Menu:");
                    FileLog.Log(ex.ToStringFull());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tournament XPerience", "An error was detected re-rolling your prize pool.\nPlease zip up your save game (Documents folder)\nError log (harmony.log.txt on desktop)\nYour config file in BMTournamentXP\\ModuleData\nPut on google drive and message me link on Nexus.");
                FileLog.Log("ERROR: BMTournamentXP: Re-roll Prize Pool");
                FileLog.Log(ex.ToStringFull());
            }
        }

        public static bool PrizeSelectCondition(MenuCallbackArgs args)
        {
            if (!TournamentConfiguration.Instance.PrizeConfiguration.EnablePrizeSelection)
            {
                return false;
            }
            bool flag;
            TextObject textObject;
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
        }

        public static void PrizeSelectConsequence(MenuCallbackArgs args)
        {
            try
            {
                List<InquiryElement> prizeElements = new List<InquiryElement>();
                TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
                TournamentPrizePool currentPool = GetTournamentPrizePool(Settlement.CurrentSettlement);

                if (currentPool.Prizes.Count < TournamentConfiguration.Instance.PrizeConfiguration.NumberOfPrizeOptions)
                {
                    ItemObject prize = GenerateTournamentPrize(tournamentGame, currentPool, true);
                }


                //  InformationManager.Clear();
                foreach (ItemRosterElement ire in currentPool.Prizes)
                {
                    ItemObject p = ire.EquipmentElement.Item;
                    try
                    {
                        var ii = new ImageIdentifier(p.StringId, ImageIdentifierType.Item, p.Name.ToString());
                        prizeElements.Add(new InquiryElement(p.StringId, p.Name.ToString(), ii, true, p.ToToolTipTextObject().ToString()));
                        //  InformationManager.AddTooltipInformation(typeof(ItemObject), new object[] { p });
                    }
                    catch (Exception ex)
                    {
                        FileLog.Log("ERROR: Tournament Prize System\nFailed to add prize element to display" + p.StringId);
                        FileLog.Log(ex.ToStringFull());
                    }
                }
                if (prizeElements.Count > 0)
                {
                    InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                            "Tournament Prize Selection", "You can choose an item from the list below as your reward if you win the tournament!", prizeElements, true, true, "OK", "Cancel",
                            new Action<List<InquiryElement>>(OnSelectPrize), new Action<List<InquiryElement>>(OnDeSelectPrize)), true);
                    try
                    {
                        GameMenu.SwitchToMenu("town_arena");
                    }
                    catch (Exception ex)
                    {
                        FileLog.Log("ERROR: BMTournamentXP: Select Prize: Refresh Menu");
                        FileLog.Log(ex.ToStringFull());
                    }
                }
                else
                {
                    InformationManager.ShowInquiry(new InquiryData("Tournament Prize Selection", "You should not be seeing this.  Something went wrong generating the prize list. Your item restrictions may be set too narrow.", true, false, "OK", "", null, null));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tournament XPerience", "An error was detected selecting your prize pool.\nPlease zip up your save game (Documents folder)\nError log (harmony.log.txt on desktop)\nYour config file in BMTournamentXP\\ModuleData\nPut on google drive and message me link on Nexus.");
                FileLog.Log("ERROR: BMTournamentXP: Tournament Prize Selection");
                FileLog.Log(ex.ToStringFull());
            }
        }

        static void OnSelectPrize(List<InquiryElement> prizeSelections)
        {
            if (prizeSelections.Count > 0)
            {
                try
                {
                    TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
                    var prize = Game.Current.ObjectManager.GetObject<ItemObject>(prizeSelections.First().Identifier.ToString());
                    SetTournamentSelectedPrize(tournamentGame, prize);
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: OnSelectPrize: Error setting Town Prize");
                    FileLog.Log(ex.ToStringFull());
                }
                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: OnSelectPrize: Refresh Arena Menu");
                    FileLog.Log(ex.ToStringFull());
                }
            }
        }
        static void OnDeSelectPrize(List<InquiryElement> prizeSelections)
        {

        }

        public static bool TournamentTypeSelectCondition(MenuCallbackArgs args)
        {
            //if (!TournamentConfiguration.Instance.EnableTournamentTypeSelection)
            //{
            //    return false;
            //}
            bool flag;
            TextObject textObject;
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
        }
        public static void TournamentTypeSelectConsequence(MenuCallbackArgs args)
        {
            List<InquiryElement> tournamentTypeElements = new List<InquiryElement>();
            tournamentTypeElements.Add(new InquiryElement("melee", "Standard Melee Tournament", new ImageIdentifier("battania_noble_sword_2_t5", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("melee2", "Individual Only Melee Tournament", new ImageIdentifier("battania_noble_sword_2_t5", ImageIdentifierType.Item)));
#if DEBUG
            tournamentTypeElements.Add(new InquiryElement("archery", "Archery Tournament", new ImageIdentifier("training_longbow", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("joust", "Jousting Tournament", new ImageIdentifier("khuzait_lance_3_t5", ImageIdentifierType.Item)));
            tournamentTypeElements.Add(new InquiryElement("race", "Horse Racing Tournament", new ImageIdentifier("desert_war_horse", ImageIdentifierType.Item)));
#endif
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                    "Tournament Type Selection", "What kind of Tournament would you like to compete in today?", tournamentTypeElements, true, true, "OK", "Cancel",
                    new Action<List<InquiryElement>>(OnSelectTournamentStyle), new Action<List<InquiryElement>>(OnSelectDoNothing)), true);
            try
            {
                GameMenu.SwitchToMenu("town_arena");
            }
            catch (Exception ex)
            {
                FileLog.Log("ERROR: BMTournamentXP: Select TournyType: Refreshing Arena Menu:");
                FileLog.Log(ex.ToStringFull());
            }

        }

        static void OnSelectTournamentStyle(List<InquiryElement> selectedTypes)
        {
            if (selectedTypes.Count > 0)
            {
                var town = Settlement.CurrentSettlement.Town;
                TournamentManager tournamentManager = Campaign.Current.TournamentManager as TournamentManager;
                TournamentGame tournamentGame;
                TournamentGame currentGame = tournamentManager.GetTournamentGame(town);
            
                switch (selectedTypes.First().Identifier.ToString())
                {
                    case "melee":
                        tournamentGame = new FightTournamentGame(town);
                        break;
                    case "melee2":
                        tournamentGame = new Fight2TournamentGame(town);
                        break;
                    case "archery":
                        tournamentGame = new ArcheryTournamentGame(town);
                        break;
                    case "joust":
                        tournamentGame = new JoustingTournamentGame(town);
                        break;
                    case "race":
                        tournamentGame = new HorseRaceTournamentGame(town);
                        break;
                    default:
                        tournamentGame = new FightTournamentGame(town);
                        break;
                }

                if (tournamentGame.GetType() != currentGame.GetType())
                {
                    ((List<TournamentGame>)Traverse.Create(tournamentManager).Field("_activeTournaments").GetValue()).Remove(currentGame);
                    tournamentManager.AddTournament(tournamentGame);
                }

                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    FileLog.Log("ERROR: BMTournamentXP: Refreshing Arena Screen:");
                    FileLog.Log(ex.ToStringFull());
                }
            }
        }
        static void OnSelectDoNothing(List<InquiryElement> prizeSelections)
        {

        }


#endregion
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
