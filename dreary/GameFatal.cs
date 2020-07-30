using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dreary
{
    public partial class GameFatal : Form
    {
        private Exception ex;
        public GameFatal(Exception e)
        {
            ex = e;
            InitializeComponent();
            try
            {
                richTextBox1.Text = $"{ex.Message}\n{ex.Source}\n{ex.TargetSite}\n{ex.StackTrace}\n{ex.HResult}\n{ex.HelpLink}\nReport this to @nougatchi#2097 over discord.";
                Console.WriteLine($"{ex.Message}\n{ex.Source}\n{ex.TargetSite}\n{ex.StackTrace}\n{ex.HResult}\n{ex.HelpLink}\nReport this to @nougatchi#2097 over discord.");
            } catch(Exception ex)
            {
                MessageBox.Show("Apparently, there was an error showing the error dialogue. Press OK to exit. Heres the error data:" + $"{ex.Message}\n{ex.Source}\n{ex.TargetSite}\n{ex.StackTrace}\n{ex.HResult}\n{ex.HelpLink}\nReport this to @nougatchi#2097 over discord.");
                Application.Exit();
            }
            try
            {
                AudioSystem.PlayAudio("GameSounds/rwd.mp3", 0.15f);
            } catch(Exception ex)
            {
                
            }
        }

        private void GameFatal_Load(object sender, EventArgs e)
        {
            
        }

        private void GameFatal_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
