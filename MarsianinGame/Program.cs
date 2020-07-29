using System;
using System.Threading;

namespace Algorithm
{
    class Program
    {
        private const string MAP_NAME = @"maps/map3.txt";
        private const int SLEEP_TIME = 200;
        private static Maps myMap = new Maps(MAP_NAME);
        private static Algorithm algorithm = new Algorithm(myMap);
        static void Main(string[] args)
        {
            ShowAlgorithmResult();

            Console.ReadKey();
        }

        private static void ShowAlgorithmResult()
        {
            foreach (Point point in algorithm.Result)
            {
                for (int y = 0; y < myMap.Map.GetLength(1); y++)
                {
                    for (int x = 0; x < myMap.Map.GetLength(0); x++)
                    {
                        if (point.X == x && point.Y == y)
                        {
                            Console.Write('P');
                        }
                        else
                            Console.Write(myMap.Map[y, x]);
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                }
                Thread.Sleep(SLEEP_TIME);
                Console.Clear();
            }
        }
    }
}
