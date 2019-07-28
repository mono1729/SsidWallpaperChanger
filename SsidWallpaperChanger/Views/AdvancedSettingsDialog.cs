using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SsidWallpaperChanger.Utilities;
using System.Configuration;

namespace SsidWallpaperChanger.Views
{
    public partial class AdvancedSettingsDialog : Form
    {
        private float _dpiScale;


        public AdvancedSettingsDialog()
        {
            InitializeComponent();
            _dpiScale = Consts.DpiScale;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ReadSettings();
        }

        private void ReadSettings()
        {
            resetWallpaperCheckBox.Checked = 
                ConfigurationManager.AppSettings["resetWallpaperOnExit"].Equals(bool.TrueString);
        }

        private void WriteSettings()
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfg.AppSettings.Settings["resetWallpaperOnExit"].Value =
                resetWallpaperCheckBox.Checked.ToString();
            cfg.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            WriteSettings();
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
