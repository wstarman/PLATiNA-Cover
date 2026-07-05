using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLATiNA_Cover
{
    public partial class Covering : Form
    {
        private string _imagePath = "";
        private bool _keepAspectRatio = true;

        public string imagePath
        {
            get { return _imagePath; }
            set
            {
                if (imagePath != value)
                {
                    _imagePath = value;
                    pictureBox1.Image = File.Exists(value) ? Image.FromFile(value) : null;
                }
            }
        }
        public bool keepAspectRatio
        {
            get { return _keepAspectRatio; }
            set
            {
                _keepAspectRatio = value;
                pictureBox1.SizeMode = value ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.StretchImage;
            }
        }

        public Covering()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_LAYERED = 0x80000;
                const int WS_EX_TRANSPARENT = 0x20;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_APPWINDOW = 0x00040000;

                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT;
                cp.ExStyle |= WS_EX_TOOLWINDOW;
                cp.ExStyle &= ~WS_EX_APPWINDOW;
                return cp;
            }
        }
    }
}
