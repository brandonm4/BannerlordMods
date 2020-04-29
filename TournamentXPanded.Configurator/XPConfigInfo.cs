using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TournamentsXPanded.Settings.Attributes;

namespace TournamentXPanded.Configurator
{
    internal sealed class ConfigInfo
    {
        internal SettingPropertyAttribute attribute { get; set; }
        internal PropertyInfo propertyInfo { get; set; }
        internal SettingPropertyGroupAttribute pgroup { get; set; }
    }
}
