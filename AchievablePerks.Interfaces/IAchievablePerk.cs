using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievablePerks.Interfaces
{
   public interface IAchievablePerk
    {
        bool IsConditionMet { get; set; }
        AppliesTo PerkAppliesTo { get; set; }
        void OnGain();
        void OnDailyTick();
        void OnWeeklyTick();

    }

    [Flags]
    public enum AppliesTo
    {
        None = 0,
        MainHero = 1 << 0,
        Companions = 1 << 1,
        Wanderers = 1 << 2,
        Notable = 1 << 3,
        Commanders = 1 << 4,
        MinorFactionHero = 1 << 5,
        MinorFactionLeader = 1 << 6,
        MajorFactionLeader = 1 << 6,
    }
}
