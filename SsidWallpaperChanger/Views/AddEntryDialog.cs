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
using SsidWallpaperChanger.Utilities;

namespace SsidWallpaperChanger.Views
{
    public partial class AddEntryDialog : Form
    {
        private List<string> _ssids;
        private List<string> _connectedSsids;

        public AddEntryDialog()
        {
            InitializeComponent();
        }

        public string SelectedSsid { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // add default entry.
            _ssids = new List<string>();
            _ssids.Add(Consts.DefaultSsid);
            _ssids.AddRange(WlanService.Instance.Ssids);
            _connectedSsids = new List<string>(WlanService.Instance.ConnectedSsids);
            listBox1.DataSource = _ssids;
        }

        private void listBox1_Format(object sender, ListControlConvertEventArgs e)
        {
            if (_connectedSsids.Contains((string)e.Value))
            {
                e.Value = (string)e.Value + " [CONNECTED]";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var idx = listBox1.SelectedIndex;
            SelectedSsid = (string)listBox1.Items[idx];
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.PerformClick();
        }
    }
}
