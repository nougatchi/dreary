using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dreary.Forms
{
    public partial class OneExecute : Form
    {
        public string output;
        public OneExecute()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            output = richTextBox1.Text;
            Close();
        }
    }
}
