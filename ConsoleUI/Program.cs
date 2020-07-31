using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AlgorithmLibrary;

namespace ConsoleUI
{
    class Program
    {

        private static string mapName = @"maps/map.txt";
        private const int SLEEP_TIME = 300;
        private static Maps AlgorithmMap;
        private static Maps myMap;
        private static Algorithm algorithm;

        private static int stepCount = -1;

        private const int MAX_XP = 100;
        private static int currentXP = MAX_XP;
        static void Main(string[] args)
        {
            try
            {
                LoadGame(args);

                WriteResultToFile();

                ShowAlgorithmResult();

                Console.WriteLine();

                Console.WriteLine("The program is done!");
                Console.WriteLine("The results are written to moves.txt.");
            }
            catch (ArgumentException e) when (e.Message == "The character died in the way!" ||
            e.Message == "There are no way to the Q!" || e.Message.Contains("There is something unknown"))
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Change the map and try again!");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                MyDebug.WriteExceptionInFile(e);
                Console.WriteLine("The error is in the log folder.");
                Console.WriteLine("Fix the problem and try again.");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ooops! Something went wrong!");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("The error is in the log folder.");
                MyDebug.WriteExceptionInFile(ex);
            }
            if (!System.Diagnostics.Debugger.IsAttached)
                Console.ReadKey();
        }

        private static void LoadGame(string[] args)
        {
            if (args.Length == 1)
            {
                mapName = $"maps\\{args[0]}";
            }
            AlgorithmMap = new Maps(mapName);
            myMap = new Maps(mapName);
            algorithm = new Algorithm(AlgorithmMap);
        }
        private static void ShowAlgorithmResult()
        {
            foreach (Point point in algorithm.Result)
            {
                Console.Clear();

                for (int y = 0; y < myMap.Map.GetLength(0); y++)
                {
                    for (int x = 0; x < myMap.Map.GetLength(1); x++)
                    {
                        if (point.X == x && point.Y == y)
                        {
                            Console.Write('P');
                            CheckObject(point);
                        }
                        else
                            Console.Write(myMap.Map[y, x]);
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                }
                ShowInfo();

                Thread.Sleep(SLEEP_TIME);
                    
            }
        }
        private static void WriteResultToFile()
        {
            string fileName = @"moves.txt";
            
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                sw.WriteLine(algorithm.Directions.Length);
                sw.WriteLine(algorithm.Directions);
            }
        }
        private static void ShowInfo()
        {
            stepCount++;
            Console.WriteLine($"Number of steps - {stepCount}");
            Console.WriteLine($"Current XP - {currentXP}");
        }
        private static void CheckObject(Point point)
        {
            char tempObject = myMap.ReturnObject(point);
            if(tempObject == '.')
            {
                return;
            }
            else if (Maps.KEYS.Contains(tempObject) || Maps.DOORS.Contains(tempObject))
            {
                myMap.DeleteObject(point);
            }
            else if(Maps.MEDKIT == tempObject)
            {
                UseMedkit(point);
            }
            else if (Maps.FIRE_POWER.Contains(tempObject))
            {
                GetDamage((int)Char.GetNumericValue(tempObject));
            }
            
        }

        private static void GetDamage(int firePower)
        {
            int damage = 20;
            currentXP -= damage * firePower;
        }

        private static void UseMedkit(Point position)
        {
            currentXP = MAX_XP;
            myMap.DeleteObject(position);
        }
    }
}
