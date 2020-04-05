using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using TaleWorlds.Core;

namespace BMTournamentXP.Models
{
    public class BMTournamentXPConfiguration
    {

        public void LoadXML(string xmlpath = "")
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
                      
                        case "tournament":
                            foreach (XmlNode nc in n.ChildNodes)
                            {
                                switch (nc.Name.Trim().ToLower())
                                {
                                    case "enable":
                                        if (string.Equals(nc.InnerText, "true", StringComparison.OrdinalIgnoreCase))
                                        {
                                            IsTournamentXPEnabled = true;
                                        }
                                        else
                                        {
                                            IsTournamentXPEnabled = false;
                                        }
                                        break;
                                    case "xpadjustment":
                                        float tadj = 1;
                                        float.TryParse(nc.InnerText, out tadj);
                                     TournamentXPAdjustment = tadj;
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
                                          IsArenaXPEnabled = true;
                                        }
                                        else
                                        {
                                            IsArenaXPEnabled = false;
                                        }
                                        break;
                                    case "xpadjustment":
                                        float tadj = 1;
                                        float.TryParse(nc.InnerText, out tadj);
                                        ArenaXPAdjustment = tadj;
                                        break;
                                }
                            }
                            break;


                    }
                }
                xmlDocument = null;
            }
        }
        private static BMTournamentXPConfiguration _instance = null;
        public static BMTournamentXPConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BMTournamentXPConfiguration();
                    if (_instance == null)
                        throw new Exception("Unable to find settings in Loader");
                }

                return _instance;
            }
        }


        public static string Version { get; set; } = "e1.1.2";
        public bool IsTournamentXPEnabled { get; set; } = true;
        public float TournamentXPAdjustment { get; set; } = 1.0f;
        public bool TournamentPrizeRerollEnabled { get; set; } = true;
      
        public bool IsArenaXPEnabled { get; set; } = true;
        public float ArenaXPAdjustment { get; set; } = 1.0f;            
    }
}
