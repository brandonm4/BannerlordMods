namespace TournamentsXPanded.Models
{
    public class TournamentReward
    {
        public int BonusGold { get; set; }
        public float BonusRenown { get; set; }
        public float BonusInfluence { get; set; }
        public int LastPayoutIndex { get; set; } = -1;
        public bool PrizeGiven { get; set; } = false;

        public TournamentReward()
        {
            BonusRenown = TournamentXPSettings.Instance.BonusTournamentWinRenown;
            BonusInfluence = TournamentXPSettings.Instance.BonusTournamentWinInfluence + 1f;
            BonusGold = 0;
        }
    }
}