using BMTournamentPrize.Models;
using BMTournamentPrizes.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BMTournamentPrizes.Models
{
    public class TournamentPrizeExpansion
    {
        public Dictionary<string, TournamentPrizeSettings> SettlementPrizes = new Dictionary<string, TournamentPrizeSettings>();
        private static TournamentPrizeExpansion _instance;

        public static TournamentPrizeExpansion Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TournamentPrizeExpansion();                  
                }
                return _instance;
            }
        }
        public static void SetTournamentSelectedPrize(TournamentGame tournamentGame, ItemObject prize)
        {
            typeof(TournamentGame).GetProperty("Prize").SetValue(tournamentGame, prize);
        }
        public static void SetTournamentSelectedPrize(string settlement_string_id, string prize_string_id)
        {
            TournamentPrizeSettings settings;
            if (Instance.SettlementPrizes.TryGetValue(settlement_string_id, out settings))
            {
                try
                {
                    var tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.Find(settlement_string_id).Town);
                    var prize = Game.Current.ObjectManager.GetObject<ItemObject>(prize_string_id);
                    typeof(TournamentGame).GetProperty("Prize").SetValue(tournamentGame, prize);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Tournament Prize System", "Error Setting Tournament Prize\nFunction:SetTrounamentSelectPrize\n\nError:" + ex.ToStringFull());
                    FileLog.Log("Error Setting Tournament Prize\nFunction:SetTrounamentSelectPrize\nError:" + ex.ToStringFull());
                }
            }
        }
        public static void UpdatePrizeSettings(string settlement_string_id, TournamentPrizeSettings settings)
        {
            if (Instance.SettlementPrizes.ContainsKey(settlement_string_id))
            {
                Instance.SettlementPrizes[settlement_string_id] = settings;
            }
            else
            {
                Instance.SettlementPrizes.Add(settlement_string_id, settings);
            }
        }
    }

    public class TournamentPrizeSettings
    {
        public List<ItemObject> Items { get; set; } = new List<ItemObject>();
        public string itemid { get; set; } = "";
        public int RemainingRerolls { get; set; } = BMTournamentPrizeConfiguration.Instance.MaxNumberOfRerollsPerTournament;
    }
}
