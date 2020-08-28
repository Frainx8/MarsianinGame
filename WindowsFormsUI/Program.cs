using System;
using System.Collections.Generic;
using System.Diagnostics;
using AlgorithmLibrary;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsUI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 
        static string consoleFolder = "Ñonsole";
        static string consoleExePath = $@"{consoleFolder}\solution.exe";
        private static string mapName = @"maps\map.txt";
        
        [STAThread]
        static void Main(string[] args)
        {
            if(!CheckForFolder(consoleFolder))
            {
                Directory.CreateDirectory(consoleFolder);
            }
            if (args.Length == 2)
            {
                int index = Array.IndexOf(args, "-console");
                if(index == -1)
                {
                    index = Array.IndexOf(args, "-Console");
                }
                else if (index == -1)
                {
                    throw new Exception("There two arguments, but no one has -console!");
                }
                Trace.WriteLine(index);
                if (index == 0)
                {
                    mapName = args[1];
                }
                else
                {
                    mapName = args[0];
                }
                Process.Start(consoleExePath, mapName);
            }
            else if(args.Contains("-console"))
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
                Application.Run(new MainForm(mapName));
            }
            
        }

        private static bool CheckForFolder(string folderName)
        {
            var directory = new DirectoryInfo(folderName);
            if (directory.Exists)
            {
                return true;
            }
            else
                return false;
        }
    }
}
