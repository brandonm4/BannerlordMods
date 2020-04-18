using System;

using TournamentsXPanded.Common;

namespace XPanded.Common.Extensions
{
    public static class TournamentStringExtensions
    {
        public static bool ConvertToBool(this string s)
        {
            if (!String.IsNullOrWhiteSpace(s))
            {
                s = s.Trim().ToLower();
                switch (s)
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

        public static PrizeListMode ToPrizeListMode(this string s)
        {
            if (!String.IsNullOrWhiteSpace(s))
            {
                s = s.Trim().ToLower();
                switch (s)
                {
                    case "vanilla":
                    case "stock":
                        return PrizeListMode.Vanilla;

                    case "custom":
                        return PrizeListMode.Custom;

                    case "townstock":
                    case "townvanilla":
                        return PrizeListMode.TownVanilla;

                    case "towncustom":
                        return PrizeListMode.TownCustom;

                    case "townonly":
                        return PrizeListMode.TownOnly;
                }
            }

            return PrizeListMode.Vanilla;
        }
    }
}