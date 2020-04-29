using TXP.ModLib.GUI.ViewModels;
using TXP.ModLib.Interfaces;
using System;

namespace TXP.ModLib
{
    public class SetIntSettingProperty : IAction
    {
        public Ref Context { get; } = null;

        public object Value { get; }

        private SettingProperty SettingProperty { get; }
        private int originalValue;

        public SetIntSettingProperty(SettingProperty settingProperty, int value)
        {
            if (settingProperty == null) throw new ArgumentNullException(nameof(settingProperty));
            Value = value;
            SettingProperty = settingProperty;
            originalValue = SettingProperty.IntValue;
        }

        public void DoAction()
        {
            SettingProperty.IntValue = (int)Value;
        }

        public void UndoAction()
        {
            SettingProperty.IntValue = originalValue;
        }
    }
}
