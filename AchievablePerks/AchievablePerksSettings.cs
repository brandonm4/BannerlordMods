namespace AchievablePerks
{
    public class AchievablePerksSettings
    {
        private static AchievablePerksSettings _instance;

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