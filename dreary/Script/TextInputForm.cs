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
    public partial class TextInputForm : Form
    {
        public string output;
        public TextInputForm(string title, string button)
        {
            InitializeComponent();
            button1.Text = button;
            Text = title;
        }

        private void TextInputForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            output = textBox1.Text;
            Close();
        }
    }
}
