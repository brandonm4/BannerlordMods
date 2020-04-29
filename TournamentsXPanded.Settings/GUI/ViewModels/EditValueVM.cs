﻿using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace TournamentsXPanded.Settings.GUI.ViewModels
{
    public class EditValueVM : ViewModel
    {
        private string _textInput = "";
        private string _titleText = "title";
        private string _descriptionText = "description";

        public SettingProperty SettingProperty { get; set; } = null;

        [DataSourceProperty]
        public string TextInput
        {
            get => _textInput;
            set
            {
                if (_textInput != value)
                {
                    _textInput = value;
                    OnPropertyChanged();

                }
            }
        }
        [DataSourceProperty]
        public string TitleText
        {
            get => _titleText;
            set
            {
                _titleText = value;
                OnPropertyChanged();
            }
        }
        [DataSourceProperty]
        public string DescriptionText
        {
            get => _descriptionText;
            set
            {
                _descriptionText = value;
                OnPropertyChanged();
            }
        }
        [DataSourceProperty]
        public SettingTypeTXP SettingTypeTXP => SettingProperty.SettingTypeTXP;
        [DataSourceProperty]
        public float MinValue => SettingProperty.SettingAttribute.EditableMinValue;
        [DataSourceProperty]
        public float MaxValue => SettingProperty.SettingAttribute.EditableMaxValue;

        public EditValueVM(SettingProperty settingProperty)
        {
            SettingProperty = settingProperty;

            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            TitleText = $"Edit \"{SettingProperty.Name}\"";
            string format = SettingProperty.SettingTypeTXP == SettingTypeTXP.Int ? "0" : "0.00";
            DescriptionText = $"Edit the value for \"{SettingProperty.Name}\".\nThe minimum value is {SettingProperty.SettingAttribute.EditableMinValue.ToString(format)} and the maximum value is {SettingProperty.SettingAttribute.EditableMaxValue.ToString(format)}.";
            TextInput = SettingProperty.ValueString;
            OnPropertyChanged(nameof(SettingTypeTXP));
        }

        public void ExecuteDone()
        {
            if (SettingProperty.SettingTypeTXP == SettingTypeTXP.Int)
            {
                int val;
                if (int.TryParse(TextInput, out val))
                {
                    SettingProperty.URS.Do(new SetIntSettingProperty(SettingProperty, val));
                    SettingProperty.URS.Do(new SetValueAction<int>(new Ref(SettingProperty.Property, SettingProperty.SettingsInstance), val));
                    SettingProperty.OnPropertyChanged("ValueString");
                }
            }
            else if (SettingProperty.SettingTypeTXP == SettingTypeTXP.Float)
            {
                float val;
                if (float.TryParse(TextInput, out val))
                {
                    SettingProperty.URS.Do(new SetFloatSettingProperty(SettingProperty, val));
                    SettingProperty.URS.Do(new SetValueAction<float>(new Ref(SettingProperty.Property, SettingProperty.SettingsInstance), val));
                    SettingProperty.OnPropertyChanged("ValueString");
                }
            }
            ScreenManager.PopScreen();
        }

        public void ExecuteCancel()
        {
            ScreenManager.PopScreen();
        }
    }
}
