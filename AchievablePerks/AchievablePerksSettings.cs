using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievablePerks
{
    public class AchievablePerksSettings
    {
        static AchievablePerksSettings _instance;
        public static AchievablePerksSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AchievablePerksSettings();
                }
                return _instance;
            }
        }

        public bool DebugMode { get; set; } = false;
    }
}
