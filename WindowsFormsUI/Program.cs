using Microsoft.VisualBasic.FileIO;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace WindowsFormsUI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 
        static string consoleFolder = "Console";
        static string consoleExePath = $@"{consoleFolder}\solution.exe";
        private static string mapName = @"maps\map.txt";
        
        [STAThread]
        static void Main(string[] args)
        {
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
                        throw new Exception("There two arguments, but no one has -console!");
                    }
                }
                else
                {
                    if (mapIndex == 0)
                    {
                        mapName = args[1];
                    }
                    else
                    {
                        mapName = args[0];
                    }
                    Process.Start(consoleExePath, mapName);
                }

            }
            else if(args.Length == 1 && args[0].ToLower() == "-console")
            {
                Process.Start(consoleExePath);
            }
            else
            {
                if (args.Length == 1)
                {
                    mapName = $"maps/{args[0]}";
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

        private static bool CheckForFolder(string folderName)
        {
            var directory = new DirectoryInfo($@".\{folderName}");
            directory.Refresh();
            if (directory.Exists)
            {
                return true;
            }
            else
                return false;
        }

        
    }
}
