
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TournamentLib.Extensions;
using TournamentLib.Models;

namespace BMTournamentPrizes.Models
{
    public class TournamentPrizeExpansion
    {
        private Dictionary<string, TournamentPrizeSettings> _settlementPrizes = new Dictionary<string, TournamentPrizeSettings>();
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
        public void SetTournamentSelectedPrize(string settlement_string_id, string prize_string_id)
        {
            TournamentPrizeSettings settings;
            if (_settlementPrizes.TryGetValue(settlement_string_id, out settings))
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
        public TournamentPrizeSettings GetPrizesSettingsForSettlement(string settlement_string_id)
        {
            TournamentPrizeSettings settings;
            if (_settlementPrizes.ContainsKey(settlement_string_id))
            {
                settings = _settlementPrizes[settlement_string_id];
            }
            else
            {
                settings = new TournamentPrizeSettings();
            }
            return settings;
        }
        public void ClearTournamentPrizes(string settlement_string_id)
        {
            if (_settlementPrizes.ContainsKey(settlement_string_id))
            {
                _settlementPrizes.Remove(settlement_string_id);
            }
        }
        public void ClearAllTournamentPrizes()
        {
            _settlementPrizes = new Dictionary<string, TournamentPrizeSettings>();
        }
        public void UpdatePrizeSettings(string settlement_string_id, TournamentPrizeSettings settings)
        {
            if (_settlementPrizes.ContainsKey(settlement_string_id))
            {
                _settlementPrizes[settlement_string_id] = settings;
            }
            else
            {
                _settlementPrizes.Add(settlement_string_id, settings);
            }
        }

        public static ItemObject GenerateTournamentPrize(TournamentGame tournamentGame, List<ItemObject> existingPrizes = null)
        {
            ItemObject prize;
            var numItemsToGet = TournamentConfiguration.Instance.PrizeConfiguration.NumberOfPrizeOptions;
            TournamentPrizeSettings prizeSettings = TournamentPrizeExpansion.Instance.GetPrizesSettingsForSettlement(tournamentGame.Town.Settlement.StringId);

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

            while (pickeditems.Count < numItemsToGet && allitems.Count() > 0)
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
            TournamentPrizeExpansion.Instance.UpdatePrizeSettings(tournamentGame.Town.Settlement.StringId, prizeSettings);
            return prize;
        }

        private static List<string> GetValidTownItems(TournamentGame tournamentGame, int minValue, int maxValue, List<ItemObject.ItemTypeEnum> validtypes)
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
            string[] strArray = new String[] { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
            ItemObject obj = Game.Current.ObjectManager.GetObject<ItemObject>(strArray.GetRandomElement<string>());
            ItemObject itemObject = MBRandom.ChooseWeighted<ItemObject>(ItemObject.All, (ItemObject item) => {
                if ((float)item.Value > 1000f * (item.IsMountable ? 0.5f : 1f))
                {
                    if ((float)item.Value < 5000f * (item.IsMountable ? 0.5f : 1f) && item.Culture == settlement.Town.Culture && (item.IsCraftedWeapon || item.IsMountable || item.ArmorComponent != null))
                    {
                        return 1f;
                    }
                }
                return 0f;
            }) ?? MBRandom.ChooseWeighted<ItemObject>(ItemObject.All, (ItemObject item) => {
                if ((float)item.Value > 1000f * (item.IsMountable ? 0.5f : 1f))
                {
                    if ((float)item.Value < 5000f * (item.IsMountable ? 0.5f : 1f) && (item.IsCraftedWeapon || item.IsMountable || item.ArmorComponent != null))
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
    }

    public class TournamentPrizeSettings
    {
        public List<ItemObject> Items { get; set; } = new List<ItemObject>();
        public string itemid { get; set; } = "";
        public int RemainingRerolls { get; set; } = TournamentConfiguration.Instance.PrizeConfiguration.MaxNumberOfRerollsPerTournament;
    }
}
