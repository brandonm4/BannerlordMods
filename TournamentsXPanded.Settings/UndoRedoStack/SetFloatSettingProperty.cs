using TournamentsXPanded.Settings.GUI.ViewModels;
using TournamentsXPanded.Settings.Interfaces;
using System;
using TournamentsXPanded.Settings;

namespace TournamentsXPanded.Settings
{
    public class SetFloatSettingProperty : IAction
    {
        public Ref Context { get; } = null;

        public object Value { get; }

        private SettingProperty SettingProperty { get; }
        private float originalValue;

        public SetFloatSettingProperty(SettingProperty settingProperty, float value)
        {
            if (settingProperty == null) throw new ArgumentNullException(nameof(settingProperty));

            Value = value;
            SettingProperty = settingProperty;
            originalValue = SettingProperty.IntValue;
        }

        public void DoAction()
        {
            SettingProperty.FloatValue = (float)Value;
        }

        public void UndoAction()
        {
            SettingProperty.FloatValue = originalValue;
        }
    }
}
