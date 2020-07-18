using System;

namespace MarsianinGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Maps myMap = new Maps(@"maps/map3.txt");
            Algorithm algorithm = new Algorithm(myMap);
            for (int y = 0; y < myMap.Map.GetLength(1); y++)
            {
                for (int x = 0; x < myMap.Map.GetLength(0); x++)
                {
                    Console.Write(myMap.Map[y, x]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            
        }
    }
}
