using HarmonyLib;
using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TournamentsXPanded.Models;
using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Behaviors
{

    class SaveData
    {
        public List<TournamentXPandedModel> Tournaments { get; set; }
    }
   
    public partial class TournamentsXPandedBehavior : CampaignBehaviorBase
    {
        internal const string saveId = "57AA1526EC9A43768612F4EF71D0F901";
        internal static List<TournamentXPandedModel> Tournaments = new List<TournamentXPandedModel>();
        internal static List<ItemEquipmentFilter> EquipmentFilters = new List<ItemEquipmentFilter>();

        internal static TournamentXPandedModel GetTournamentInfo(Town town)
        {
            var t = Tournaments.Where(x => x.SettlementStringId == town.Settlement.StringId).FirstOrDefault();
            if (t == null)
            {
                t = new TournamentXPandedModel() { SettlementStringId = town.Settlement.StringId };
                if (town.Settlement.HasTournament)
                {
                    var g = Campaign.Current.TournamentManager.GetTournamentGame(town);
                    t.PrizePool.Add(new PrizeItem() { ItemStringId = g.Prize.StringId, ItemModifierStringId = "" });
                    t.SelectedPrizeStringId = g.Prize.StringId;
                }
                Tournaments.Add(t);
            }
            return t;
        }
        

        #region Events
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
        }
        private void OnAfterNewGameCreated(CampaignGameStarter starter)
        {
            if (TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament > 0)
            {
                var text = new TextObject("{=tourn001}Re-roll Prize"); //Was going to put the remaining count, but not updating correctly.
                starter.AddGameMenuOption("town_arena", "bm_reroll_tournamentprize", text.ToString(),
                    new GameMenuOption.OnConditionDelegate(RerollCondition),
                    new GameMenuOption.OnConsequenceDelegate(RerollConsequence),
                    false, -1, true);
            }
            if (TournamentXPSettings.Instance.EnablePrizeSelection)
            {
                starter.AddGameMenuOption("town_arena", "bm_select_tournamentprize", new TextObject("{=tourn002}Select Prize").ToString(),
                 new GameMenuOption.OnConditionDelegate(PrizeSelectCondition),
                 new GameMenuOption.OnConsequenceDelegate(PrizeSelectConsequence), false, 1, true);
            }

            //if (TournamentXPSettings.Instance.EnableTournamentTypeSelection)
            //{
            //    starter.AddGameMenuOption("town_arena", "bm_select_tournamenttype", new TextObject("{=tourn003}Select Tournament Style").ToString(),
            //     new GameMenuOption.OnConditionDelegate(TournamentTypeSelectCondition),
            //     new GameMenuOption.OnConsequenceDelegate(TournamentTypeSelectConsequence), false, 2, true);
            //}
        }

        public override void SyncData(IDataStore dataStore)
        {
            SaveData data;
            string saveDataAsJson = null;
            if (dataStore.IsSaving)
            {
                data = new SaveData() { Tournaments = TournamentsXPandedBehavior.Tournaments };
                saveDataAsJson = JsonConvert.SerializeObject(data);                
            }

            dataStore.SyncData(saveId, ref saveDataAsJson);

            if (dataStore.IsLoading)
            {
                data = JsonConvert.DeserializeObject<SaveData>(saveDataAsJson);
                TournamentsXPandedBehavior.Tournaments = data.Tournaments;
            }
            
        }
        #endregion

        #region Reroll
        private static bool RerollCondition(MenuCallbackArgs args)
        {
            if (TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament == 0)
            {
                return false;
            }
            bool flag;
            TextObject textObject;
            var tournamentInfo = GetTournamentInfo(Settlement.CurrentSettlement.Town);
            bool flag1 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag, out textObject);

            if (tournamentInfo.ReRollsUsed >= TournamentXPSettings.Instance.MaxNumberOfRerollsPerTournament)
            {
                flag = true;
                flag1 = false;
                textObject = new TextObject("{=tourn004}Re-roll Attempts Exceeded");
            }
            args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return MenuHelper.SetOptionProperties(args, flag1, flag, textObject);
        }
        public static void RerollConsequence(MenuCallbackArgs args)
        {
            try
            {
                var tournamentInfo = GetTournamentInfo(Settlement.CurrentSettlement.Town);
                TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);

                //Clear old prizes
                tournamentInfo.SelectedPrizeStringId = "";
                tournamentInfo.PrizePool = new List<PrizeItem>();
                tournamentInfo.ReRollsUsed++;

                //Generate New Prize
                var prize = GenerateTournamentPrize(tournamentGame, tournamentInfo, false, true);
                SetTournamentSelectedPrize(tournamentGame, prize);

                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("ERROR: BMTournamentXP: Re-roll: Refreshing Arena Menu:");
                    ErrorLog.Log(ex.ToStringFull());
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log("Re-roll Prize Pool");
                ErrorLog.Log(ex.ToStringFull());
            }
        }
        #endregion

        #region PrizeSelect
        public static bool PrizeSelectCondition(MenuCallbackArgs args)
        {
            if (!TournamentXPSettings.Instance.EnablePrizeSelection)
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
                var tournamentInfo = GetTournamentInfo(Settlement.CurrentSettlement.Town);

                if (tournamentInfo.PrizePool.Count < TournamentXPSettings.Instance.NumberOfPrizeOptions)
                {
                    ItemObject prize = GenerateTournamentPrize(tournamentGame, tournamentInfo, true, false);
                }

                //  InformationManager.Clear();
                foreach (PrizeItem prizeItem in tournamentInfo.PrizePool)
                {
                    var p = prizeItem.ToItemRosterElement().EquipmentElement;
                    try
                    {
                        var ii = new ImageIdentifier(p.Item.StringId, ImageIdentifierType.Item, p.GetModifiedItemName().ToString());
                        // prizeElements.Add(new InquiryElement(p.Item.StringId, ii, true, p.Item.ToToolTipTextObject().ToString()));
                        prizeElements.Add(new InquiryElement(
                            p.Item.StringId,
                            p.GetModifiedItemName().ToString(),
                            ii,
                            true,
                            p.ToToolTipTextObject().ToString()
                            ));
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("ERROR: Tournament Prize System\nFailed to add prize element to display" + p.Item.StringId);
                        ErrorLog.Log(ex.ToStringFull());
                    }
                }
                if (prizeElements.Count > 0)
                {
                    string info = "{=tourn012}You can choose an item from the list below as your reward if you win the tournament!";
                    TextObject descr = new TextObject(info);

                    if (TournamentXPSettings.Instance.DebugMode)
                    {
                        info = "Town:{TOWN}\nMin:{MIN}\nMax:{MAX}\nProsperity:{PROSPERITY}\nTypes:{TYPES}";
                        descr = new TextObject(info);
                        descr.SetTextVariable("TOWN", tournamentInfo.Settlement.Name);
                        descr.SetTextVariable("MIN", GetMinPrizeValue().ToString());
                        descr.SetTextVariable("MAX", GetMaxPrizeValue().ToString());
                        descr.SetTextVariable("PROSPERITY", tournamentInfo.Settlement.Town.GetProsperityLevel().ToString());
                        var types = "";
                        foreach (var t in TournamentXPSettings.Instance.GetActivePrizeTypes())
                        {
                            types += t.ToString() + ", ";
                        }
                        types = types.Substring(0, types.Length - 2);
                        descr.SetTextVariable("TYPES", types);
                    }

                    InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                           new TextObject("{=tourn005}Tournament Prize Selection").ToString(), descr.ToString(), prizeElements, true, true, new TextObject("{=tourn006}OK").ToString(), new TextObject("{=tourn007}Cancel").ToString(),
                            new Action<List<InquiryElement>>(OnSelectPrize), new Action<List<InquiryElement>>(OnDeSelectPrize)), true);
                    try
                    {
                        GameMenu.SwitchToMenu("town_arena");
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("ERROR: BMTournamentXP: Select Prize: Refresh Menu");
                        ErrorLog.Log(ex.ToStringFull());
                    }
                }
                else
                {
                    InformationManager.ShowInquiry(new InquiryData("Tournament Prize Selection", "You should not be seeing this.  Something went wrong generating the prize list. Your item restrictions may be set too narrow.", true, false, "OK", "", null, null));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log("ERROR: BMTournamentXP: Tournament Prize Selection");
                ErrorLog.Log(ex.ToStringFull());
            }
        }
        private static void OnSelectPrize(List<InquiryElement> prizeSelections)
        {
            if (prizeSelections.Count > 0)
            {
                try
                {
                    TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
                    var tournamentInfo = GetTournamentInfo(Settlement.CurrentSettlement.Town);
                    tournamentInfo.SelectedPrizeStringId = prizeSelections.First().Identifier.ToString();
                    var prize = tournamentInfo.SelectedPrizeItem.ToItemRosterElement().EquipmentElement.Item;
                    SetTournamentSelectedPrize(tournamentGame, prize);
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("ERROR: BMTournamentXP: OnSelectPrize: Error setting Town Prize");
                    ErrorLog.Log(ex.ToStringFull());
                }
                try
                {
                    GameMenu.SwitchToMenu("town_arena");
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("ERROR: BMTournamentXP: OnSelectPrize: Refresh Arena Menu");
                    ErrorLog.Log(ex.ToStringFull());
                }
            }
        }
        private static void OnDeSelectPrize(List<InquiryElement> prizeSelections)
        {
        }
        #endregion
    }
}
