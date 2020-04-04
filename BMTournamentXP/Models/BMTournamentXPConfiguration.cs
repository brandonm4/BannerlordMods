using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TaleWorlds.Core;

namespace BMTournamentXP.Models
{
    public class BMTournamentXPConfiguration
    {

        public BMTournamentXPConfiguration(string xmlpath = "")
        {
            if (xmlpath != "")
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
                                    case "enable":
                                        if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                        {
                                            BMTournamentXPMain.Configuration.IsTournamentXPEnabled = true;
                                        }
                                        else
                                        {
                                            BMTournamentXPMain.Configuration.IsTournamentXPEnabled = false;
                                        }
                                        break;
                                    case "xpadjustment":
                                        float tadj = 1;
                                        float.TryParse(nc.InnerText, out tadj);
                                        BMTournamentXPMain.Configuration.TournamentXPAdjustment = tadj;
                                        break;
                                    case "additionalgold":
                                        int bg = 0;
                                        int.TryParse(nc.InnerText, out bg);
                                        BMTournamentXPMain.Configuration.TournamentBonusMoney = bg;
                                        break;
                                    case "enablereroll":
                                        if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                        {
                                            BMTournamentXPMain.Configuration.TournamentPrizeRerollEnabled = true;
                                        }
                                        else
                                        {
                                            BMTournamentXPMain.Configuration.TournamentPrizeRerollEnabled = false;
                                        }
                                        break;
                                    case "prizelistmode":
                                        PrizeListMode = nc.InnerText.Trim();
                                        break;
                                    case "townprizemin":
                                        BMTournamentXPMain.Configuration.TownPrizeMin = int.Parse(nc.InnerText.Trim());
                                        break;
                                    case "townprizemax":
                                        BMTournamentXPMain.Configuration.TownPrizeMin = int.Parse(nc.InnerText.Trim());
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
                                                MessageBox.Show("Invalid entry in Tournament Town Prize Types: ");
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                        case "arena":
                            foreach (XmlNode nc in n.ChildNodes)
                            {
                                switch (nc.Name.Trim().ToLower())
                                {
                                    case "enable":
                                        if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                        {
                                            BMTournamentXPMain.Configuration.IsArenaXPEnabled = true;
                                        }
                                        else
                                        {
                                            BMTournamentXPMain.Configuration.IsArenaXPEnabled = false;
                                        }
                                        break;
                                    case "xpadjustment":
                                        float tadj = 1;
                                        float.TryParse(nc.InnerText, out tadj);
                                        BMTournamentXPMain.Configuration.ArenaXPAdjustment = tadj;
                                        break;
                                }
                            }
                            break;


                    }
                }
                xmlDocument = null;
            }
        }


        public static string Version { get; set; } = "e1.1.1";
        public bool IsTournamentXPEnabled { get; set; } = true;
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        public bool TournamentPrizeRerollEnabled { get; set; } = true;
        public int TournamentBonusMoney { get; set; } = 500;
        public bool IsArenaXPEnabled { get; set; } = true;
        public float ArenaXPAdjustment { get; set; } = 1.0f;

        public string PrizeListMode { get; set; } = "Custom";
        public int TownPrizeMin { get; set; } = 1000;
        public int TownPrizeMax { get; set; } = 10000;
        public List<ItemObject.ItemTypeEnum> TownValidPrizeTypes { get; set; } = new List<ItemObject.ItemTypeEnum>();

        public bool UseTownInventoryAsPrize { get; set; } = true;
        public List<string> TourneyItems = new List<string>() { "winds_fury_sword_t3", "bone_crusher_mace_t3", "tyrhung_sword_t3", "pernach_mace_t3", "early_retirement_2hsword_t3", "black_heart_2haxe_t3", "knights_fall_mace_t3", "the_scalpel_sword_t3", "judgement_mace_t3", "dawnbreaker_sword_t3", "ambassador_sword_t3", "heavy_nasalhelm_over_imperial_mail", "closed_desert_helmet", "sturgian_helmet_closed", "full_helm_over_laced_coif", "desert_mail_coif", "heavy_nasalhelm_over_imperial_mail", "plumed_nomad_helmet", "eastern_studded_shoulders", "ridged_northernhelm", "armored_bearskin", "noble_horse_southern", "noble_horse_imperial", "noble_horse_western", "noble_horse_eastern", "noble_horse_battania", "noble_horse_northern", "special_camel" };
    }
}
