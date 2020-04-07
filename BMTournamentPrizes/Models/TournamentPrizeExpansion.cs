﻿using BMTournamentPrize.Models;
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
                catch (Exception ex)
                {
                    MessageBox.Show("Tournament Prize System", "Error Setting Tournament Prize\nFunction:SetTrounamentSelectPrize\n\nError:" + ex.ToStringFull());
                    FileLog.Log("Error Setting Tournament Prize\nFunction:SetTrounamentSelectPrize\nError:" + ex.ToStringFull());
                }
            }
        }

        public static void ClearTournamentPrizes(string settlement_string_id)
        {
            if (Instance.SettlementPrizes.ContainsKey(settlement_string_id))
            {
                Instance.SettlementPrizes.Remove(settlement_string_id);
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

        public static ItemObject GenerateTournamentPrize(TournamentGame tournamentGame, List<ItemObject> existingPrizes = null)
        {
            ItemObject prize;
            TournamentPrizeSettings prizeSettings;// = new TournamentPrizeSettings();
            TournamentPrizeExpansion.Instance.SettlementPrizes.TryGetValue(tournamentGame.Town.Settlement.StringId, out prizeSettings);
            if (prizeSettings == null)
            {
                prizeSettings = new TournamentPrizeSettings();
            }

            List<string> townitems = new List<string>();
            if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim().IndexOf("town", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                townitems = GetValidTownItems(tournamentGame.Town.Owner.ItemRoster, BMTournamentPrizeConfiguration.Instance.TownPrizeMin, BMTournamentPrizeConfiguration.Instance.TownPrizeMax, BMTournamentPrizeConfiguration.Instance.TownValidPrizeTypes);
            }

            var allitems = new List<string>();

            if (BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim().IndexOf("custom", StringComparison.OrdinalIgnoreCase) >= 0
                || BMTournamentPrizeConfiguration.Instance.PrizeListMode.Trim().IndexOf("stock", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                allitems = townitems.Concat(BMTournamentPrizeConfiguration.Instance.TourneyItems).ToList();
            }
            else
            {
                allitems = townitems;
            }
            var numItemsToGet = BMTournamentPrizeConfiguration.Instance.NumberOfPrizeOptions;

            var pickeditems = new List<string>();
            if (existingPrizes != null)
            {
                foreach (var existingPrize in existingPrizes)
                {
                    pickeditems.Add(existingPrize.StringId);
                    if (allitems.Contains(existingPrize.StringId))
                    {
                        allitems.Remove(existingPrize.StringId);
                    }
                }
            }

            if (allitems.Count() < numItemsToGet)
            {
                numItemsToGet = allitems.Count();
            }

            while (pickeditems.Count < numItemsToGet && allitems.Count > 0)
            {
                var randomId = allitems.GetRandomElement<string>();

                if (!pickeditems.Contains(randomId))
                {
                    pickeditems.Add(randomId);
                    allitems.Remove(randomId);
                }
            }
            prizeSettings.Items = new List<ItemObject>();
            foreach (var id in pickeditems)
            {
                prizeSettings.Items.Add(Game.Current.ObjectManager.GetObject<ItemObject>(id));
            }
            prizeSettings.itemid = prizeSettings.Items[MBRandom.RandomInt(prizeSettings.Items.Count - 1)].StringId;
            prize = prizeSettings.Items.Where(x => x.StringId == prizeSettings.itemid).Single();
            TournamentPrizeExpansion.UpdatePrizeSettings(tournamentGame.Town.Settlement.StringId, prizeSettings);
            return prize;

        }

        private static List<string> GetValidTownItems(ItemRoster roster, int minValue, int maxValue, List<ItemObject.ItemTypeEnum> validtypes)
        {
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
                list = BMTournamentPrizeConfiguration.Instance.TourneyItems;
            }
            if (list.Count == 0)
            {
                MessageBox.Show("Tournament Prize System", "Warning: The current town has no prizes available with your current defined filter.  Defaulting to Vanilla items.");
                list = BMTournamentPrizeConfiguration.StockTourneyItems;
            }

            return list;
        }
    }

    public class TournamentPrizeSettings
    {
        public List<ItemObject> Items { get; set; } = new List<ItemObject>();
        public string itemid { get; set; } = "";
        public int RemainingRerolls { get; set; } = BMTournamentPrizeConfiguration.Instance.MaxNumberOfRerollsPerTournament;
    }
}