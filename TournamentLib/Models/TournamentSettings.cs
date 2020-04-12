using ModLib;
using ModLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TournamentLib.Models
{
    public class TournamentXPSettings : SettingsBase
    {
        private const string instanceID = "TournamentXPSettings";
        private static Settings _instance = null;
        public override string ModName => "BMTournamentXP";
        public override string ModuleFolderName => ModLibSubModule.ModuleFolderName;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FileDatabase.Get<ModLib.Settings>(instanceID);
                    if (_instance == null)
                    {
                        _instance = new Settings();
                        SettingsDatabase.SaveSettings(_instance);
                    }
                }
                return _instance;
            }
        }
        [XmlElement]
        public override string ID { get; set; } = instanceID;

        #region Miscellaneous
        [XmlElement]
        [SettingProperty("Disable Quest Troops Affecting Morale", "When enabled, quest troops such as \"Borrowed Troop\" in your party are ignored when party morale is calculated.")]
        public bool QuestCharactersIgnorePartySize { get; set; } = false;
        [XmlElement]
        [SettingProperty("Show Number of Days of Food", "Changes the number showing how much food you have to instead show how many days' worth of food you have. (Bottom right of campaign map UI).")]
        public bool ShowFoodDaysRemaining { get; set; } = false;

    }
}
