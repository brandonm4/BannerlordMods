using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaleWorlds.Core;
using TournamentLib.Extensions;


namespace TournamentLib.Models
{
    public class TournamentConfiguration
    {
        public static string LastXMLPath { get; set; } = "";

        public void LoadXML(string xmlpath = "")
        {
            LastXMLPath = xmlpath;    
        
            if (!String.IsNullOrWhiteSpace(xmlpath))
            {
                //if (File.Exists(xmlpath))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(xmlpath);
                    XPConfiguration = new XPConfiguration();
                    PrizeConfiguration = new PrizeConfiguration();

                    XmlNode xmlNodes = xmlDocument.SelectSingleNode("BMTournamentXP");
                    foreach (XmlNode n in xmlNodes.ChildNodes)
                    {
                        switch (n.Name.Trim().ToLower())
                        {                         
                            case "tournament":
                                foreach (XmlNode nc in n.ChildNodes)
                                {
                                    switch (nc.Name.Trim().ToLower())
                                    {                                      
                                        case "enablereroll":
                                            PrizeConfiguration.TournamentPrizeRerollEnabled = nc.InnerText.ConvertToBool();                                         
                                            break;
                                        case "prizelistmode":
                                            PrizeConfiguration.PrizeListMode = nc.InnerText.Trim();
                                            break;
                                        case "townprizemin":
                                            PrizeConfiguration.TownPrizeMin = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "townprizemax":
                                            PrizeConfiguration.TownPrizeMax = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "includedtownprizetypes":
                                            foreach (XmlNode np in nc.ChildNodes)
                                            {
                                                try
                                                {
                                                    PrizeConfiguration.TownValidPrizeTypes.Add((ItemObject.ItemTypeEnum)Enum.Parse(typeof(ItemObject.ItemTypeEnum), np.InnerText.Trim(), true));
                                                }
                                                catch
                                                {
                                                    //   MessageBox.Show("Invalid entry in Tournament Town Prize Types: ");
                                                }
                                            }
                                            break;
                                        case "tournamentbonusmoneybasenamedcharlevel":
                                            PrizeConfiguration.TournamentBonusMoneyBaseNamedCharLevel = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "customprizefilename":
                                            PrizeConfiguration.CustomPrizeFileName = nc.InnerText.Trim();
                                            if (!PrizeConfiguration.CustomPrizeFileName.EndsWith(".json"))
                                            {
                                                PrizeConfiguration.CustomPrizeFileName += ".json";
                                            }
                                            break;
                                        case "oppenentdifficultyaffectsodds":
                                            PrizeConfiguration.OppenentDifficultyAffectsOdds = nc.InnerText.ConvertToBool();                                            
                                            break;
                                        case "maximumbetodds":
                                            PrizeConfiguration.MaximumBetOdds = float.Parse(nc.InnerText.Trim());
                                            break;
                                        case "enableconfigreloadrealtime":
                                            PrizeConfiguration.EnableConfigReloadRealTime = nc.InnerText.ConvertToBool();                                            
                                            break;
                                        case "companionswinprizes":
                                            PrizeConfiguration.CompanionsWinPrizes = nc.InnerText.ConvertToBool();                                            
                                            break;
                                        case "numberofprizeoptions":
                                            PrizeConfiguration.NumberOfPrizeOptions = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "enableprizeselection":
                                            PrizeConfiguration.EnablePrizeSelection = nc.InnerText.ConvertToBool();                                            
                                            break;
                                        case "maxnumberofrerollspertournament":
                                            PrizeConfiguration.MaxNumberOfRerollsPerTournament = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "townprizeminmaxaffectsvanillaandcustomlistsaswell":
                                            PrizeConfiguration.TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell = nc.InnerText.ConvertToBool();                                            
                                            break;
                                        case "bonustournamentmatchgold":
                                            PrizeConfiguration.BonusTournamentMatchGold = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "bonustournamentwingold":
                                            PrizeConfiguration.BonusTournamentWinGold = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "bonustournamentwinrenown":
                                            PrizeConfiguration.BonusTournamentWinRenown = int.Parse(nc.InnerText.Trim());
                                            break;
                                        case "bonustournamentwininfluence":
                                            PrizeConfiguration.BonusTournamentWinInfluence = float.Parse(nc.InnerText.Trim());
                                            break;
                                        case "enablerenownpertrooptier":
                                            PrizeConfiguration.EnableRenownPerTroopTier = nc.InnerText.ConvertToBool();                                            
                                            break;
                                        case "renownpertrooptier":
                                            PrizeConfiguration.RenownPerTroopTier = new List<float>();
                                            var tiervalues = nc.InnerText.Split(',');
                                            foreach(var t in tiervalues)
                                            {                                                
                                                float rptt = 0;
                                                float.TryParse(t, out rptt);
                                                PrizeConfiguration.RenownPerTroopTier.Add(rptt);
                                            }
                                            break;
                                        case "renownperheroproperty":
                                            PrizeConfiguration.RenownPerHeroProperty = new List<float>();
                                            var tiervalues2= nc.InnerText.Split(',');
                                            foreach (var t in tiervalues2)
                                            {                                                
                                                float rptt = 0;
                                                float.TryParse(t, out rptt);
                                                PrizeConfiguration.RenownPerHeroProperty.Add(rptt);
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    xmlDocument = null;
                    if (!PrizeConfiguration.EnablePrizeSelection)
                    {
                        PrizeConfiguration.NumberOfPrizeOptions = 1;
                    }
                }
            }
        }
        private static TournamentConfiguration _instance = null;
        public static TournamentConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TournamentConfiguration();                
                }                
                return _instance;
            }
        }

        public PrizeConfiguration PrizeConfiguration { get; set; } = new PrizeConfiguration();
        public XPConfiguration XPConfiguration { get; set; } = new XPConfiguration();
    }

    public class PrizeConfiguration
    {
        public bool EnableConfigReloadRealTime { get; set; } = false;
        public static string Version { get; set; } = "e1.2.3";

        public bool TournamentPrizeRerollEnabled { get; set; } = true;
        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;

        public string PrizeListMode { get; set; } = "TownCustom";
        public int TownPrizeMin { get; set; } = 1000;
        public int TownPrizeMax { get; set; } = 10000;
        public bool TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell { get; set; } = false;
        public List<ItemObject.ItemTypeEnum> TownValidPrizeTypes { get; set; } = new List<ItemObject.ItemTypeEnum>();
        public List<string> TourneyItems = new List<string>() { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
        public string CustomPrizeFileName { get; set; } = "";

        public int NumberOfPrizeOptions { get; set; } = 3;
        public bool EnablePrizeSelection { get; set; } = true;

        public bool CompanionsWinPrizes { get; set; } = false;

        public bool OppenentDifficultyAffectsOdds { get; set; } = true;
        public float MaximumBetOdds { get; set; } = 2;
        public int BonusTournamentMatchGold { get; set; } = 500;
        public int BonusTournamentWinGold { get; set; } = 500;

        public int BonusTournamentWinRenown { get; set; } = 3;
        public float BonusTournamentWinInfluence { get; set; } = 1f;

        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;
        public bool EnableRenownPerTroopTier { get; set; } = true;
        public List<float> RenownPerTroopTier { get; set; } = new List<float>() { 0, 1, 1, 1, 2, 2, 2, 3 };

        /* Base, IsNoble, IsNotable, IsCommander, IsMinorFactionLeader, IsMajorFactionLeader */
        public List<float> RenownPerHeroProperty { get; set; } = new List<float>() { 1, 3, 1, 1, 2, 5, 10 };

        public static List<string> StockTourneyItems { get; } = new List<string>() { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
    }
    public class XPConfiguration
    {
        public bool IsTournamentXPEnabled { get; set; } = true;
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        public bool TournamentPrizeRerollEnabled { get; set; } = true;
        public bool IsArenaXPEnabled { get; set; } = true;
        public float ArenaXPAdjustment { get; set; } = 1.0f;
    }

    public enum RenownHeroTier
    {
        HeroBase,
        IsNoble,
        IsNotable,
        IsCommander,
        IsMinorFactionHero,
        IsMinorFactionLeader,
        IsMajorFactionLeader,
    };
}
