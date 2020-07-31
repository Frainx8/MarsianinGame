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
        static string consoleExePath = @"console\solution.exe";
        private static string mapName = @"maps\map.txt";
        private const int SLEEP_TIME = 200;
        private static Maps AlgorithmMap;
        private static Maps myMap;
        private static Algorithm algorithm;
        [STAThread]
        static void Main(string[] args)
        {
            
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

                AlgorithmMap = new Maps(mapName);
                myMap = new Maps(mapName);
                algorithm = new Algorithm(AlgorithmMap);

                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            
        }
    }
}
