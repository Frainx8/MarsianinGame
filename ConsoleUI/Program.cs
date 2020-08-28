using System;
using System.Linq;
using System.Threading;
using AlgorithmLibrary;

namespace ConsoleUI
{
    class Program
    {
        private static string projectName = "MarsianinGame";
        private static string logFolderFullPath;
        private static string mapsFolder = @"..\maps\";
        private static string logFolderName = "log";
        private static string mapName = $@"{mapsFolder}map.txt";
        private static string movesName = @"..\moves.txt";
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

                algorithm.WriteResultToFile(movesName);

                bool isWant;

                isWant = CheckIfWantSeeTheResutl();

                if(isWant)
                {
                    ShowAlgorithmResult();
                }

                Console.WriteLine();

                Console.WriteLine("The program is done!");
                if(CheckForMovesTxt())
                {
                    Console.WriteLine($"The results are written to {movesName}");
                }
                else
                {
                    Console.WriteLine($"WARNING! File moves.txt wasn't created!");
                    Console.WriteLine($"Run the program as an administrator!");
                }
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
                MyDebug.WriteExceptionInFile(e, projectName, logFolderName);
                Console.WriteLine($"The error is in the {logFolderFullPath}.");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ooops! Something went wrong!");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine($"The error is in the {logFolderFullPath}.");
                MyDebug.WriteExceptionInFile(ex, projectName, logFolderName);
            }
            Console.ReadKey();
        }

        private static bool CheckIfWantSeeTheResutl()
        {
            Console.WriteLine("Do you want to see the result? Y, enter - yes, N - no.");

            while (true)
            {
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.Y || input.Key == ConsoleKey.Enter)
                {
                    return true;
                }
                else if (input.Key == ConsoleKey.N)
                {
                    return false;
                }
            }
            
        }

        private static void LoadGame(string[] args)
        {
            logFolderFullPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}" +
                $"\\{projectName}\\{logFolderName}";
            if (args.Length == 1)
            {
                mapName = @$"{mapsFolder}{args[0]}";
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
                int firePower = (int)Char.GetNumericValue(tempObject);
                GetDamage(firePower);
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

        /// <summary>
        /// Returns true if moves.txt exist.
        /// </summary>
        /// <returns></returns>
        private static bool CheckForMovesTxt()
        {
            if (System.IO.File.Exists(movesName))
            {
                return true;
            }
            else
                return false;
        }
    }
}
