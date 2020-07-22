using System;
using System.Threading;

namespace MarsianinGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Maps myMap = new Maps(@"maps/map3.txt");
            Algorithm algorithm = new Algorithm(myMap);
            foreach(Point point in algorithm.Result)
            {
                for (int y = 0; y < myMap.Map.GetLength(1); y++)
                {
                    for (int x = 0; x < myMap.Map.GetLength(0); x++)
                    {
                        if(point.X == x && point.Y ==y)
                        {
                            Console.Write('P');
                        }
                        else
                            Console.Write(myMap.Map[y, x]);
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                }
                Thread.Sleep(500);
                Console.Clear();
            }
            
        }
    }
}
