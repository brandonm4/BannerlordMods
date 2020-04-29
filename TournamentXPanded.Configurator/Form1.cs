using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TournamentsXPanded.Models;
using TournamentsXPanded.Settings.Attributes;
using TournamentXPanded.Configurator.Controls;
using TournamentXPanded.Configurator.Localization;

namespace TournamentXPanded.Configurator
{


    public partial class Form1 : Form
    {
        private TournamentXPSettings _settings;
        List<ConfigInfo> propList;
        List<XPSettingControlBase> settingprops;

        public Form1()
        {
            InitializeComponent();
        }

        public void SetupLanguageDropdown()
        {
            ddlLanguage.Items.Clear();
            foreach(var l in LocalizedTextManager.LanguageIds)
            {
                ddlLanguage.Items.Add(l);
                ddlLanguage.SelectedIndex = 0;
            }
        }


        public void LoadConfig(string path)
        {
            txtConfigFilePath.Text = path;
            txtConfigFilePath.ReadOnly = true;
            var settings = TournamentsXPanded.Settings.SettingsHelper.GetFromFile(path);
            LoadConfig(settings);
        }
        public void LoadConfig(TournamentXPSettings settings)
        {
            _settings = settings;
            pnlSettings.Controls.Clear();
            settingprops = new List<XPSettingControlBase>();
            //var groups = new List<SettingPropertyGroup>();
            propList = (from p in typeof(TournamentXPSettings).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        let propAttr = p.GetCustomAttribute<SettingPropertyAttribute>(true)
                        let groupAttr = p.GetCustomAttribute<SettingPropertyGroupAttribute>(true)
                        where propAttr != null
                        let groupAttrToAdd = groupAttr ?? null
                        select new ConfigInfo { attribute = propAttr, propertyInfo = p, pgroup = groupAttrToAdd }).ToList();


            List<string> descriptions = new List<string>();
            List<string> hints = new List<string>();
            List<string> groups = new List<string>();


            List<FlowLayoutPanel> groupPanels = new List<FlowLayoutPanel>();
            groupPanels.Add(new FlowLayoutPanel() { Name = "Default" });

            var groupcount = 0;
            var desccount = 0;
            var hintcount = 0;

            foreach (var pr in propList)
            {
                FlowLayoutPanel pnl = groupPanels.Where(x => x.Name == "Default").First();
                if (pr.pgroup != null)
                {
                    pnl = groupPanels.Where(x => x.Name == pr.pgroup.GroupName).FirstOrDefault();
                    if (pnl == null)
                    {
                        pnl = new FlowLayoutPanel() { Name = pr.pgroup.GroupName };
                        pnl.AutoScroll = false;
                        pnl.AutoSize = true;
                        pnl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnl.BorderStyle = BorderStyle.FixedSingle;

                        string text = pr.pgroup.GroupName;
                        if (text.Trim().StartsWith("{"))
                        {
                            var id = LocalizedTextManager.GetStringId(text);
                            text = LocalizedTextManager.GetTranslatedText(ddlLanguage.SelectedItem.ToString(), id);
                        }

                        pnl.Controls.Add(new Label() { AutoSize = true, MinimumSize = new Size(700, 50), Text = text, Font = new System.Drawing.Font("Arial Narrow", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))) });
                        groupPanels.Add(pnl);
                        groupcount++;
                        groups.Add(pr.pgroup.GroupName);
                    }
                }

                var uc = new XPSettingControlBase(ddlLanguage.SelectedItem.ToString());
                uc.Anchor = AnchorStyles.Top;
                uc.SetProperty(pr.attribute, pr.propertyInfo, _settings);
                pnl.Controls.Add(uc);
                settingprops.Add(uc);
#if DEBUG
                descriptions.Add(pr.attribute.DisplayName);
                if (!string.IsNullOrWhiteSpace(pr.attribute.HintText))
                    hints.Add(pr.attribute.HintText);
#endif 
            }

            foreach (var pnl in groupPanels)
            {
                if (pnl.Controls.Count > 0)
                    pnlSettings.Controls.Add(pnl);
            }

            pnlSettings.Refresh();
#if DEBUG
            using (var sw = new System.IO.StreamWriter("E:\\texts.json"))
            {
                var count = 0;
                foreach (var s in groups)
                {
                    count++;
                    string id = ("000" + count.ToString());
                    id = id.Substring(id.Length - 4);
                    sw.WriteLine("<string id=\"txpg" + id + "\" text=\"" + s + "\" />");
                }
                foreach (var s in descriptions)
                {
                    count++;
                    string id = ("000" + count.ToString());
                    id = id.Substring(id.Length - 4);
                    sw.WriteLine("<string id=\"txpd" + id + "\" text=\"" + s + "\" />");
                }
                foreach (var s in hints)
                {
                    count++;
                    string id = ("000" + count.ToString());
                    id = id.Substring(id.Length - 4);
                    sw.WriteLine("<string id=\"txph" + id + "\" text=\"" + s + "\" />");
                }
            }
#endif
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            foreach (var p in settingprops)
            {
                p.UpdateSetting();
            }
            TournamentsXPanded.Settings.SettingsHelper.WriteSettings(txtConfigFilePath.Text, _settings);
            MessageBox.Show("Settings Saved. Close this window using the X in the top right to exit.");
        }

        private void DdlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_settings != null)
            LoadConfig(_settings);
        }
    }


}
