using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;

namespace TournamentsXPanded.Utility
{
    internal static class CptTourneyWrapper
    {
        static Assembly CptTourneyAssembly;
        static CptTourneyWrapper()
        {
            if (Utilities.GetModulesNames().Contains("CptTourney"))
            {
                CptTourneyAssembly  = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name == "CptTourney").FirstOrDefault();                
            }
        }

        public static bool IsKnighthoodCulture(Settlement settlement = null, CultureObject culture = null)
        {
            if (Utilities.GetModulesNames().Contains("CptTourney") && CptTourneyAssembly != null)
            {                              
                var cptTourney = CptTourneyAssembly.GetType("CptTourney.CptTourneySubmodule");
                if ((bool)cptTourney.GetMethod("isKnighthoodCulture", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Invoke(null, new object[] { settlement, culture }))
                {
                    return true;
                }              
            }
            return false;
        }
         
    }
}
