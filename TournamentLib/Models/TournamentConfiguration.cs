using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TournamentLib.Extensions;


namespace TournamentLib.Models
{
    public class TournamentConfiguration
    {

        public const string TournamentXPVersion = "1.3.1";

        public static string LastXMLPath { get; set; } = "";
        public bool HasLoaded = false;

        public void LoadXML(string xmlpath = "", bool force = false)
        {
            LastXMLPath = xmlpath;
            if (HasLoaded && !force)
            {
                return;
            }

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
                                        case "enable":
                                            XPConfiguration.IsTournamentXPEnabled = nc.InnerText.ConvertToBool();
                                            break;
                                        case "xpadjustment":
                                            float tadj = 1;
                                            float.TryParse(nc.InnerText, out tadj);
                                            XPConfiguration.TournamentXPAdjustment = tadj;
                                            break;
                                        case "enablereroll":
                                            PrizeConfiguration.TournamentPrizeRerollEnabled = nc.InnerText.ConvertToBool();
                                            break;
                                        case "prizelistmode":
                                            PrizeConfiguration.PrizeListMode = nc.InnerText.ToPrizeListMode();
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
                                            string tourneyitemsfile = String.Concat(BasePath.Name, "Modules/BMTournamentXP/ModuleData/", TournamentConfiguration.Instance.PrizeConfiguration.CustomPrizeFileName);
                                            if (File.Exists(tourneyitemsfile))
                                            {
                                                var configtxt = File.ReadAllText(tourneyitemsfile);
                                                TournamentConfiguration.Instance.PrizeConfiguration.CustomTourneyItems = JsonConvert.DeserializeObject<List<string>>(configtxt);
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
                                            foreach (var t in tiervalues)
                                            {
                                                float rptt = 0;
                                                float.TryParse(t, out rptt);
                                                PrizeConfiguration.RenownPerTroopTier.Add(rptt);
                                            }
                                            break;
                                        case "renownperheroproperty":
                                            PrizeConfiguration.RenownPerHeroProperty = new List<float>();
                                            var tiervalues2 = nc.InnerText.Split(',');
                                            foreach (var t in tiervalues2)
                                            {
                                                float rptt = 0;
                                                float.TryParse(t, out rptt);
                                                PrizeConfiguration.RenownPerHeroProperty.Add(rptt);
                                            }
                                            break;
                                        case "tournamentequipmentfilter":
                                            TournamentTweaks.TournamentEquipmentFilter = nc.InnerText.ConvertToBool();
                                            break;
                                        case "enableprizetypefiltertolists":
                                            PrizeConfiguration.EnablePrizeTypeFilterToLists = nc.InnerText.ConvertToBool();
                                            break;
                                        case "bonustournamentmatchgoldimmediate":
                                            PrizeConfiguration.BonusTournamentMatchGoldImmediate = nc.InnerText.ConvertToBool();
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
                                            XPConfiguration.IsArenaXPEnabled = nc.InnerText.ConvertToBool();
                                            break;
                                        case "xpadjustment":
                                            float tadj = 1;
                                            float.TryParse(nc.InnerText, out tadj);
                                            XPConfiguration.ArenaXPAdjustment = tadj;
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
            HasLoaded = true;
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


        public bool EnableTournamentTypeSelection
        {
            get
            {
#if DEBUG
                    return true;
#endif
                return false;
            }
        }

        public PrizeConfiguration PrizeConfiguration { get; set; } = new PrizeConfiguration();
        public XPConfiguration XPConfiguration { get; set; } = new XPConfiguration();
        public TournamentTweaks TournamentTweaks { get; set; } = new TournamentTweaks();


    }

    public class Config
    {

    }
    public class PrizeConfiguration : Config
    {
        public bool EnableConfigReloadRealTime { get; set; } = false;
        public static string Version { get; set; } = "e1.2.3";

        public bool TournamentPrizeRerollEnabled { get; set; } = true;
        public int MaxNumberOfRerollsPerTournament { get; set; } = 3;

        public PrizeListMode PrizeListMode { get; set; } = PrizeListMode.TownCustom;
        public int TownPrizeMin { get; set; } = 1000;
        public int TownPrizeMax { get; set; } = 5000;
        public bool TownPrizeMinMaxAffectsVanillaAndCustomListsAsWell { get; set; } = false;
        public bool EnablePrizeTypeFilterToLists { get; set; } = false;
        public List<ItemObject.ItemTypeEnum> TownValidPrizeTypes { get; set; } = new List<ItemObject.ItemTypeEnum>();
        public List<string> CustomTourneyItems { get; set; }
        public string CustomPrizeFileName { get; set; } = "";

        public int NumberOfPrizeOptions { get; set; } = 3;
        public bool EnablePrizeSelection { get; set; } = true;

        public bool CompanionsWinPrizes { get; set; } = false;

        public bool OppenentDifficultyAffectsOdds { get; set; } = true;
        public float MaximumBetOdds { get; set; } = 4;
        public int BonusTournamentMatchGold { get; set; } = 0;
        public int BonusTournamentWinGold { get; set; } = 0;
        public bool BonusTournamentMatchGoldImmediate { get; set; } = false;

        public int BonusTournamentWinRenown { get; set; } = 0;
        public float BonusTournamentWinInfluence { get; set; } = 0;

        public int TournamentBonusMoneyBaseNamedCharLevel { get; set; } = 0;
        public bool EnableRenownPerTroopTier { get; set; } = false;
        public List<float> RenownPerTroopTier { get; set; } = new List<float>() { 0, 1, 1, 1, 2, 2, 2, 3 };

        /* Base, IsNoble, IsNotable, IsCommander, IsMinorFactionLeader, IsMajorFactionLeader */
        public List<float> RenownPerHeroProperty { get; set; } = new List<float>() { 1, 3, 1, 1, 2, 5, 10 };
    }
    public class XPConfiguration : Config
    {
        public bool IsTournamentXPEnabled { get; set; } = true;
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        public bool TournamentPrizeRerollEnabled { get; set; } = true;
        public bool IsArenaXPEnabled { get; set; } = true;
        public float ArenaXPAdjustment { get; set; } = 1.0f;
    }
    public class TournamentTweaks : Config
    {
        public bool TournamentEquipmentFilter { get; set; }
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

    public enum PrizeListMode
    {
        Vanilla,
        Custom,
        TownVanilla,
        TownCustom,
        TownOnly,
    }
}
