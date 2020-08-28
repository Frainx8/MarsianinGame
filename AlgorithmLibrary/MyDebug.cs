using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlgorithmLibrary
{
    public class MyDebug
    {
        public static void WriteExceptionInFile(Exception ex, string projectName, string folderName)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullPath = $"{dir}\\{projectName}\\{folderName}";
            var directory = new DirectoryInfo(fullPath);
            if (directory.Exists == false)
            {
                Directory.CreateDirectory(fullPath);

            }
            string fileName = $@"{fullPath}\error0.txt";
            int index = 0;


            if (File.Exists(fileName))
            {
                fileName = directory.GetFiles().OrderByDescending(f => f.FullName).First().ToString();
                List<string> files = new List<string>(Directory.GetFiles(fullPath));
                for (int i = files.Count - 1; i >= 0; i--)
                {
                    if (!files[i].Contains("error"))
                    {
                        files.RemoveAt(i);
                    }

                }
                files.Sort(StringComparer.InvariantCulture);
                fileName = Path.GetFileName(files.Last());
                fileName = fileName.Substring(0, fileName.IndexOf('.'));
                index = Int32.Parse(fileName.Substring(5)) + 1;
                fileName = $@"{fullPath}\error{index}.txt";
            }
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("File created: {0}", DateTime.Now.ToString());
                sw.WriteLine();
                sw.WriteLine(ex);
            }
        }

        public static void WriteStringInDebugTxt(string _string, bool append = false, string folderName = "log")
        {
            string fileName = $@"{folderName}\debug.txt";
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
