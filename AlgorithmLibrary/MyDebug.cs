using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlgorithmLibrary
{
    public class MyDebug
    {
        public static void WriteExceptionInFile(Exception ex)
        {
            string fileName = @"log\error0.txt";
            int index = 0;

            var directory = new DirectoryInfo("log");
            if (directory.Exists == false)
            {
                Directory.CreateDirectory("log");

            }
            if (File.Exists(fileName))
            {
                fileName = directory.GetFiles().OrderByDescending(f => f.FullName).First().ToString();
                List<string> files = new List<string>(Directory.GetFiles("log"));
                for (int i = files.Count - 1; i >= 0; i--)
                {
                    if (files[i].Contains("error") != true)
                    {
                        files.RemoveAt(i);
                    }

                }
                files.Sort(StringComparer.InvariantCulture);
                fileName = Path.GetFileName(files.Last());
                fileName = fileName.Substring(0, fileName.IndexOf('.'));
                index = Int32.Parse(fileName.Substring(5)) + 1;
                fileName = $@"log\error{index}.txt";
            }
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("File created: {0}", DateTime.Now.ToString());
                sw.WriteLine();
                sw.WriteLine(ex);
            }
        }

        public static void WriteStringInDebugTxt(string _string, bool append = false)
        {
            string fileName = @"log\debug.txt";
            using (StreamWriter sw = new StreamWriter(fileName, append))
            {
                sw.WriteLine(_string);
            }
        }


        public static void WriteStringInTheConsole(string _string)
        {
            System.Diagnostics.Trace.WriteLine(_string);
        }
    }
}
