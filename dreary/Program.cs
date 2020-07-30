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
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new ConsoleTraceListener());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form gameForm = new DrearySplash();
            bool nofagsmode = false;
            if (args.Length != 0)
            {
                foreach(string i in args)
                {
                    switch(i)
                    {
                        case "-nosplash":
                            Form1 gf = new Form1();
                            if (File.Exists("game.dll"))
                            {
                                gf.dllMode = true;
                                DrearyGameDll gameDll = new DrearyGameDll("game.dll");
                                gf.gameDll = gameDll;
                                gf.throwFatalInsteadOfMsg = nofagsmode;
                            }
                            gameForm = gf;
                            break;
                        case "-nofatals":
                            nofagsmode = true;
                            break;
                        case "-h":
                            Console.WriteLine("Dreary Command Line Arguments\n" +
                                "\n" +
                                "\t-nosplash : Disables the splash screen\n" +
                                "\t-nofatals : Disables the Fatal Error messages, better for debug\n" +
                                "\t-progtest : Starts program testing mode\n" +
                                "\t-h        : Shows this help message");
                            Application.Exit();
                            break;
                        case "-progtest":
                            ProgTest.Run();
                            break;
                    }
                }
            }
            Application.Run(gameForm);
        }
    }
}
