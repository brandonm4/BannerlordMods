using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLib.Extensions
{
    public static class TournamentStringExtensions
    {
        public static bool ConvertToBool(this string s )
        {
            if (!String.IsNullOrWhiteSpace(s))
            {
                s = s.Trim().ToLower();
                switch(s)
                {
                    case "y":
                    case "yes":
                    case "1":
                    case "true":
                    case "t":
                        return true;
                }
            }
            return false;
        }
    }
}
