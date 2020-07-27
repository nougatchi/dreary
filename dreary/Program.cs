using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace dreary
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Debug.Listeners.Add(new ConsoleTraceListener());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var gameForm = new Form1();
            if (File.Exists("game.dll"))
            {
                gameForm.dllMode = true;
                DrearyGameDll gameDll = new DrearyGameDll("game.dll");
                gameForm.gameDll = gameDll;
            }
            Application.Run(gameForm);
        }
    }
}
