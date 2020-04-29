using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TournamentsXPanded.Settings.Attributes;
using System.Reflection;
using TournamentsXPanded.Models;
using TournamentXPanded.Configurator.Localization;

namespace TournamentXPanded.Configurator.Controls
{
    public partial class XPSettingControlBase : UserControl
    {
        internal PropertyInfo _pInfo;
        internal TournamentsXPanded.Models.TournamentXPSettings _settings;
        private string languageId;

        public XPSettingControlBase(string id)
        {
            InitializeComponent();
            languageId = id;
        }
        public void SetProperty(SettingPropertyAttribute attr, PropertyInfo pInfo, TournamentXPSettings settings)
        {
            _pInfo = pInfo;
            _settings = settings;


            string text = attr.DisplayName;
            if (text.Trim().StartsWith("{"))
            {
                var id = LocalizedTextManager.GetStringId(text);
              text=   LocalizedTextManager.GetTranslatedText(languageId, id);
            }
            lblName.Text = text;
            if (!string.IsNullOrWhiteSpace(attr.HintText))
            {
                string text2 = attr.HintText;
                if (text2.Trim().StartsWith("{"))
                {
                    var id = LocalizedTextManager.GetStringId(text2);
                    text2 = LocalizedTextManager.GetTranslatedText(languageId, id);
                }
                ToolTip tt = new ToolTip();
                tt.IsBalloon = true;
                tt.SetToolTip(lblName, text2);
            }

            checkBox1.Visible = false;
            textBox1.Visible = false;
            textBox1.TextAlign = HorizontalAlignment.Right;
            trackBar1.Visible = false;
            trackBar2.Visible = false;

            switch (pInfo.PropertyType.Name)
            {
                case "Boolean":
                    checkBox1.Visible = true;
                    checkBox1.Checked = (bool)pInfo.GetValue(settings);
                    break;
                case "Int32":
                case "Int64":
                case "Integer":
                    textBox1.Visible = true;
                    textBox1.Text = ((int)pInfo.GetValue(settings)).ToString();
                    trackBar1.Visible = true;
                    trackBar1.Minimum = (int)attr.MinValue;
                    trackBar1.Maximum = (int)attr.MaxValue;
                    trackBar1.Value = ((int)pInfo.GetValue(settings));
                    break;
                case "Single":
                    textBox1.Visible = true;
                    textBox1.Text = ((float)pInfo.GetValue(settings)).ToString();
                    trackBar2.Visible = true;
                    trackBar2.Minimum = attr.MinValue;
                    trackBar2.Maximum = attr.MaxValue;
                    trackBar2.Value = ((float)pInfo.GetValue(settings));
                    break;
                default:
                    break;
            }
        }

        public void UpdateSetting()
        {
            switch (_pInfo.PropertyType.Name)
            {
                case "Boolean":
                    _pInfo.SetValue(_settings, checkBox1.Checked);
                    break;
                case "Int32":
                case "Int64":
                case "Integer":
                    textBox1.Visible = true;
                    int i;
                    if (int.TryParse(textBox1.Text, out i))
                        _pInfo.SetValue(_settings, i);
                    break;
                case "Single":
                    textBox1.Visible = true;
                    float f;
                    if (float.TryParse(textBox1.Text, out f))
                        _pInfo.SetValue(_settings, f);
                    break;
                default:
                    break;
            }
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
        }
        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar2.Value.ToString();
        }
    }
}
