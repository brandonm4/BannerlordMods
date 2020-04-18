using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AchievablePerks
{
    public partial class AchievablePerksSubModule : MBSubModuleBase
    {
        public static string ModuleFolderName { get; } = "AchievablePerks";

        public static void ShowMessage(string msg, Color? color = null)
        {
            if (color == null)
                color = Color.White;

            InformationManager.DisplayMessage(new InformationMessage(msg, (Color)color));
        }
    }
}