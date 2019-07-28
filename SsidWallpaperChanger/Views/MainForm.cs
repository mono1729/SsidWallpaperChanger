using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SsidWallpaperChanger.Services;
using SsidWallpaperChanger.Models;
using SsidWallpaperChanger.ViewModels;
using SsidWallpaperChanger.Utilities;
using System.Configuration;
using System.Threading;

namespace SsidWallpaperChanger.Views
{
    public partial class MainForm : Form
    {
        private WallpaperCollection _wpCollection;
        private BindingList<WallpaperViewModel> _wallpaperVms;
        private WlanService _wlan;
        private EventService _event;
        private float _dpiScale;
        private bool _resetWallpaperOnExit => 
            ConfigurationManager.AppSettings["resetWallpaperOnExit"].Equals(bool.TrueString);

        public MainForm()
        {
            InitializeComponent();
            _dpiScale = Consts.DpiScale;
            InitializeServices();
            dataGridView1.AutoGenerateColumns = false;
            if (int.TryParse(ConfigurationManager.AppSettings["watchInterval"], out int interval))
            {
                timer1.Interval = interval;
            }
        }

        private void InitializeServices()
        {
            if (_event != null)
            {
                _event.Dispose();
            }
            _wlan = WlanService.Instance;
            _wpCollection = WallpaperCollection.ReadXml();
            _event = new EventService(_wpCollection);
            _event.RegisterEvent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Do not forget CloseReason confirmation.
            // If you don't, it will cause a malfunction on the time of shutdown.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                button2.PerformClick();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadData();
            ApplyDpiScale();
            ApplyCultureOnDataGrid();
        }

        private void LoadData()
        {
            _wallpaperVms = new BindingList<WallpaperViewModel>();

            foreach (var model in _wpCollection)
            {
                _wallpaperVms.Add(new WallpaperViewModel(model));
            }
            dataGridView1.DataSource = _wallpaperVms;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Hide();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            string ssid;
            using (var f = new AddEntryDialog())
            {
                f.ShowDialog();
                ssid = f.SelectedSsid;
            }
            
            if (!string.IsNullOrEmpty(ssid))
            {
                if (_wpCollection.ContainsSsid(ssid))
                {
                    MessageBox.Show(Properties.Resources.Already_registered_, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                var wp = new Wallpaper();
                wp.Ssid = ssid;
                wp.WallColor = Color.Black;
                wp.ResizeMode = Models.ResizeMode.Original;
                _wpCollection.Add(wp);
                _wallpaperVms.Add(new WallpaperViewModel(wp));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            InitializeServices();
            LoadData();
        }


        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var backcolorCol = ((DataGridView)sender).Columns[nameof(Wallpaper.WallColor)].Index;
            if (!(e.ColumnIndex == backcolorCol))
            {
                return;
            }
            Color back = ColorTranslator.FromHtml((string)e.Value);
            Color fore = GetLuma(back) < 0.5 ? Color.White : Color.Black;
            e.CellStyle.BackColor = back;
            e.CellStyle.ForeColor = fore;
        }

        private double GetLuma(Color color)
        {
            var r = color.R * 0.2126 / 255;
            var g = color.G * 0.7152 / 255;
            var b = color.B * 0.0722 / 255;
            return r + g + b;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var g = (DataGridView)sender;
            var imagePathIdx = g.Columns["ImagePathBrowse"].Index;
            var removeRowIdx = g.Columns["RemoveRow"].Index;
            if (e.ColumnIndex == imagePathIdx)
            {
                var result = openFileDialog1.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }
                g.Rows[e.RowIndex].Cells["ImagePath"].Value = openFileDialog1.FileName;
            }
            if (e.ColumnIndex == removeRowIdx)
            {
                if (DialogResult.No == 
                    (MessageBox.Show(
                        text: Properties.Resources.Proceed_to_remove_this_entry_,
                        caption: Properties.Resources.Confirmation,
                    buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Question)))
                {
                    return;
                }

                var wp = _wallpaperVms[e.RowIndex].GetModel();
                g.Rows.RemoveAt(e.RowIndex);
                _wpCollection.Remove(wp);
            }
        }

        // code from https://stackoverflow.com/a/26530645
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);
            var datagridview = sender as DataGridView;

            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var backColorCol = ((DataGridView)sender).Columns[nameof(Wallpaper.WallColor)].Index;
            if (e.ColumnIndex != backColorCol)
            {
                return;
            }
            var result = colorDialog1.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value = ColorTranslator.ToHtml(colorDialog1.Color);
            ((DataGridView)sender).ClearSelection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _wpCollection.WriteXml();
            this.Hide();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            _event?.WatchWlanNetworkChanging();
        }
        

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            this.timer1.Enabled = !this.Visible;
        }

        private void ApplyDpiScale()
        {
            // This method handles only DataGridView.
            // Other controls are handled by Form.AutoScaleMode property.
            foreach(DataGridViewColumn col in dataGridView1.Columns)
            {
                col.Width = (int)(col.Width * _dpiScale);
            }

            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = (int)(row.Height * _dpiScale);
            }

            dataGridView1.RowTemplate.Height = (int)(dataGridView1.RowTemplate.Height * _dpiScale);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var ad = new AboutDialog())
            {
                ad.ShowDialog();
            }
        }

        private void ApplyCultureOnDataGrid()
        {
            // Buttons or ComboBoxes on Cells are incompatible with i18n.
            if (Thread.CurrentThread.CurrentUICulture.ToString() == "ja-JP")
            {
                // ResizeMode ComboBox
                var dict = new Dictionary<string,string>();

                // TODO: Move localized messages to resource file.
                dict.Add("Original", "オリジナル");
                dict.Add("Zoom", "拡大");
                dict.Add("BorderlessZoom", "フチなし拡大");
                var resizeModeCol = (DataGridViewComboBoxColumn)dataGridView1.Columns["ResizeMode"];
                resizeModeCol.ValueMember = "Key";
                resizeModeCol.DisplayMember = "Value";
                resizeModeCol.DataSource = new BindingSource(dict, null);

                // Remove Button
                var removeCol = (DataGridViewButtonColumn)dataGridView1.Columns["RemoveRow"];
                removeCol.Text = "削除";
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            using (var d = new AdvancedSettingsDialog())
            {
                d.ShowDialog();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_resetWallpaperOnExit)
            {
                _event.ApplyDefaultWallpaper();
            }
        }
    }
}
