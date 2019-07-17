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
using Microsoft.Win32;
using SsidWallpaperChanger.Utilities;

namespace SsidWallpaperChanger.Views
{
    public partial class AdvancedSettingsDialog : Form
    {
        private float _dpiScale;
        private bool _enableCheckStatusChanged = false;


        public AdvancedSettingsDialog()
        {
            InitializeComponent();
            _dpiScale = Consts.DpiScale;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            SetShieldIconOnCheckBox();
            checkBox1.Checked = GetStatusOfAllowBlockingAppsAtShutdown();
            _enableCheckStatusChanged = true;
        }

        private void SetShieldIconOnCheckBox()
        {
            var iconSize = (int)(16 * _dpiScale);
            var smallIcon = new Bitmap(iconSize, iconSize);
            var gra = Graphics.FromImage(smallIcon);
            gra.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gra.DrawImage(SystemIcons.Shield.ToBitmap(), 0, 0, iconSize, iconSize);
            checkBox1.Image = smallIcon;
        }

        private bool GetStatusOfAllowBlockingAppsAtShutdown()
        {
            int value;
            try
            {
                value = (int)Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\System",
                    "AllowBlockingAppsAtShutdown",
                    0
                );
            }
            catch (Exception)
            {
                return false;
            }
            return value == 1;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            if (!_enableCheckStatusChanged)
                return;
            if (!ChangeStatusOfAllowBlockingAppsAtShutdown(checkBox.Checked))
            {
                _enableCheckStatusChanged = false;
                checkBox.Checked = !checkBox.Checked;
                _enableCheckStatusChanged = true;
            }
        }

        private bool ChangeStatusOfAllowBlockingAppsAtShutdown(bool status)
        {
            var statusString = status ? "1" : "0";
            var regDef =
            #region Definition of REG file.
$@"Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System]
""AllowBlockingAppsAtShutdown""=dword:0000000{statusString}";

            #endregion

            using (var f = new StreamWriter(Consts.TempRegFilePath))
            {
                f.Write(regDef);
            }

            bool statusChanged = false;

            try
            {
                var proc = Process.Start("regedit.exe", "/S " + Consts.TempRegFilePath);
                proc.WaitForExit();
                if (proc.ExitCode == 0)
                {
                    statusChanged = true;
                }
            }
            catch (Win32Exception)
            {
            }
            finally
            {
                File.Delete(Consts.TempRegFilePath);
            }

            return statusChanged;
        }
    }
}
