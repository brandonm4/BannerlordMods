using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using TaleWorlds.Core;

namespace BMTournamentPrize.Models
{
    public class BMTournamentPrizeConfiguration
    {
        
        public void LoadXML(string xmlpath = "")
        {
        
            if (!String.IsNullOrWhiteSpace(xmlpath))
            {
                //if (File.Exists(xmlpath))
                {
                    XmlDocument xmlDocument = new XmlDocument();

                    xmlDocument.Load(xmlpath);
                    XmlNode xmlNodes = xmlDocument.SelectSingleNode("BMTournamentXP");
                    foreach (XmlNode n in xmlNodes.ChildNodes)
                    {
                        switch (n.Name.Trim().ToLower())
                        {
                            case "showinfopopup":
                                //if (string.Equals(n.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                //{
                                //    _showpopup = true;
                                //}
                                //else
                                //{
                                //    _showpopup = false;
                                //}
                                break;
                            case "tournament":
                                foreach (XmlNode nc in n.ChildNodes)
                                {
                                    switch (nc.Name.Trim().ToLower())
                                    {
                                        case "additionalgold":
                                            int bg = 0;
                                            int.TryParse(nc.InnerText, out bg);
                                            TournamentBonusMoney = bg;
                                            break;
                                        case "enablereroll":
                                            if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                            {
                                                TournamentPrizeRerollEnabled = true;
                                            }
                                            else
                                            {
                                                TournamentPrizeRerollEnabled = false;
                                            }
                                            break;
                                        case "prizelistmode":
                                            PrizeListMode = nc.InnerText.Trim();
                                            break;
                                        case "townprizemin":
                                            TownPrizeMin = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "townprizemax":
                                            TownPrizeMax = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "includedtownprizetypes":
                                            foreach (XmlNode np in nc.ChildNodes)
                                            {
                                                try
                                                {
                                                    TownValidPrizeTypes.Add((ItemObject.ItemTypeEnum)Enum.Parse(typeof(ItemObject.ItemTypeEnum), np.InnerText.Trim(), true));
                                                }
                                                catch
                                                {
                                                    //   MessageBox.Show("Invalid entry in Tournament Town Prize Types: ");
                                                }
                                            }
                                            break;
                                        case "tournamentbonusmoneybasenamedcharlevel":
                                            TournamentBonusMoneyBaseNamedCharLevel = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "customprizefilename":
                                            CustomPrizeFileName = nc.InnerText.Trim();
                                            if (!CustomPrizeFileName.EndsWith(".json"))
                                            {
                                                CustomPrizeFileName += ".json";
                                            }
                                            break;
                                        case "oppenentdifficultyaffectsodds":
                                            if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                            {
                                                OppenentDifficultyAffectsOdds = true;
                                            }
                                            else
                                            {
                                                OppenentDifficultyAffectsOdds = false;
                                            }
                                            break;
                                        case "maximumbetodds":
                                            MaximumBetOdds = float.Parse(nc.InnerText.Trim());
                                            break;
                                        case "enableconfigreloadrealtime":
                                            if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                            {
                                                EnableConfigReloadRealTime = true;
                                            }
                                            else
                                            {
                                                EnableConfigReloadRealTime = false;
                                            }
                                            break;
                                        case "companionswinprizes":
                                            if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                            {
                                                CompanionsWinPrizes = true;
                                            }
                                            else
                                            {
                                                CompanionsWinPrizes = false;
                                            }
                                            break;

                                    }
                                }
                                break;



                        }
                    }
                    xmlDocument = null;
                    if (!EnablePrizeSelection)
                    {
                        NumberOfPrizeOptions = 1;
                    }
                }
            }
        }
        private static BMTournamentPrizeConfiguration _instance = null;
        public static BMTournamentPrizeConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BMTournamentPrizeConfiguration();
                    if (_instance == null)
                    {
                        FileLog.Log("ERROR: Error Initializing Tournament Prizes");
                    }
                }
                //else
                //{
                //    if (_instance.EnableConfigReloadRealTime)
                //    {
                //        _instance.LoadXML(_instance._xmlPath);
                //    }
                //}
                return _instance;
            }
        }


        public static string Version { get; set; } = "e1.1.2";     
        public bool TournamentPrizeRerollEnabled { get; set; } = true;
        public int TournamentBonusMoney { get; set; } = 500;
        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 100;
        public string PrizeListMode { get; set; } = "Custom";
        public int TownPrizeMin { get; set; } = 1000;
        public int TownPrizeMax { get; set; } = 10000;
        public List<ItemObject.ItemTypeEnum> TownValidPrizeTypes { get; set; } = new List<ItemObject.ItemTypeEnum>();
        public bool UseTownInventoryAsPrize { get; set; } = true;
        public List<string> TourneyItems = new List<string>() { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };

        public int NumberOfPrizeOptions { get; set; } = 3;
        public bool EnablePrizeSelection { get; set; } = true;
        public string CustomPrizeFileName { get; set; } = "";

        public bool OppenentDifficultyAffectsOdds { get; set; } = true;
        public float MaximumBetOdds { get; set; } = 2;

        public bool EnableConfigReloadRealTime { get; set; } = false;

        public List<string> StockTourneyItems { get; set; } = new List<string>() { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };

        public bool CompanionsWinPrizes { get; set; } = false;
    }
}
