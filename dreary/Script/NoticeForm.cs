using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dreary.Script
{
    public partial class NoticeForm : Form
    {
        public NoticeForm(string title, string notice)
        {
            InitializeComponent();
            Text = title;
            richTextBox1.Text = notice;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void NoticeForm_Load(object sender, EventArgs e)
        {

        }
    }
}
