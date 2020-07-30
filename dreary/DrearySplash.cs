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

namespace dreary
{
    public partial class DrearySplash : Form
    {
        private Timer tmr;

        public DrearySplash()
        {
            InitializeComponent();
        }

        private void DrearySplash_Shown(object sender, EventArgs e)
        {
            tmr = new Timer();
            tmr.Interval = 3000;
            tmr.Start();
            tmr.Tick += tmr_Tick;

        }

        void tmr_Tick(object sender, EventArgs e)
        {
            tmr.Stop();
            Form1 mf = new Form1();
            if (File.Exists("game.dll"))
            {
                mf.dllMode = true;
                DrearyGameDll gameDll = new DrearyGameDll("game.dll");
                mf.gameDll = gameDll;
                mf.throwFatalInsteadOfMsg = false;
            }
            mf.Show();
            Hide();
            TopMost = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            label1.Text = "DREAMSCAPE 2";
        }
    }
}
