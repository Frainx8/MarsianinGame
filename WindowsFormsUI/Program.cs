using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AlgorithmLibrary;

namespace WindowsFormsUI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 
        static string consoleExePath = $@"{CommonStuff.consoleFolder}\solution.exe";
        private static string mapName = $@"{CommonStuff.mapsFolderDefaultName}\map.txt";
        
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length > 2)
            {
                MessageBox.Show($"There are too much arguments!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (args.Length == 2)
            {
                string console = args[0].ToLower();
                int mapIndex = 1;
                if(console != "-console")
                {
                    console = args[1].ToLower();
                    mapIndex = 0;
                    if (console != "-console")
                    {
                        MessageBox.Show($"There are two arguments, but no one has -console!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                mapName = args[mapIndex];
                //The current directory for console is the parent's process directory!
                Process.Start(consoleExePath, mapName);

            }
            else if(args.Length == 1 && args[0].ToLower() == "-console")
            {
                Process.Start(consoleExePath);
            }
            else
            {
                if (args.Length == 1)
                {
                    mapName = $"{CommonStuff.mapsFolderDefaultName}\\{args[0]}";
                }
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (!File.Exists(mapName))
                {
                    MessageBox.Show($"There is no {mapName}!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Run(new MainForm());
                }
                
                else
                {
                    Application.Run(new MainForm(mapName));
                }
            }
            
        }        
    }
}
