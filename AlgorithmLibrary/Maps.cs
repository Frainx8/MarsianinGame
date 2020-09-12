using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AlgorithmLibrary
{
    /// <summary>
    /// Tools for a map.
    /// </summary>
    /// <remarks>Use WritePointInConsole for debugging</remarks>
    public class Maps
    {
        public static readonly char[] DOORS = { 'A', 'B', 'C', 'E', 'D' };
        public static readonly char[] KEYS = { 'a', 'b', 'c', 'e', 'd' };
        public static readonly char[] FIRE_POWER = { '1', '2', '3', '4', '5' };
        public const char MEDKIT = 'H';
        public Point Q { get; private set; }
        public Point S { get; private set; }
        public char[,] Map { get; private set; }

        public Maps(string nameOfMap)
        {
            Map = ReadTxt(nameOfMap);
            Q = ReturnAnElementPositionOnMap('Q');
            S = ReturnAnElementPositionOnMap('S');
            if (Q.Equals(Point.nullPoint))
            {
                throw new System.ArgumentException("There is no exit!", "Map");
            }
            if (S.Equals(Point.nullPoint))
            {
                throw new System.ArgumentException("There is no enter!", "Map");
            }
        }

        public Maps(Maps anotherMap)
        {
            Map = CopyMap(anotherMap);

            Q = anotherMap.Q;
            S = anotherMap.S;
        }

        private char[,] CopyMap(Maps anotherMap)
        {
            char[,] newMap = new char[anotherMap.Map.GetLength(0), anotherMap.Map.GetLength(1)];
            for (int y = 0; y < anotherMap.Map.GetLength(0); y++)
            {
                for (int x = 0; x < anotherMap.Map.GetLength(1); x++)
                {
                    newMap[y, x] = anotherMap.Map[y, x];
                }
            }
            return newMap;
        }


        /// <summary>
        /// Returns contained object in the point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public char ReturnObject(Point point)
        {
            return Map[point.Y, point.X];
        }

        public char ReturnObject(int x, int y)
        {
            return Map[y, x];
        }

        /// <summary>
        /// Deleting an object in the map, replacing it with a dot.
        /// </summary>
        /// <param name="position"></param>
        public void DeleteObject(Point position)
        {
            ChangeObject(position, '.');
        }

        /// <summary>
        /// Changing an object in the map
        /// </summary>
        /// <param name="position"></param>
        /// <param name="_object"></param>
        public void ChangeObject(Point position, char _object)
        {
            Map[position.Y, position.X] = _object;
        }

        /// <summary>
        /// Using nameOfTheMap.txt to fill the Map.
        /// </summary>
        /// <param name="nameOfMap"></param>
        /// <returns>Returns a double array of chars that map is.</returns>
        private char[,] ReadTxt(string nameOfMap) // Функция для чтения .txt файла
        {
            if (File.Exists(nameOfMap))
            {
#if false
                Console.WriteLine("I've started read the map!");
#endif
                char[,] map; // Создаю двумерный массив карты, где будут лежать все игровые объекты
                using (StreamReader myFile = new StreamReader(nameOfMap))
                {
                    // Пытаюсь получить размер карты
                    int[] sizeOfMap = null;
                    string line = myFile.ReadLine();
                    if (line.Last() == ' ')
                    {
                        line = line.Remove(line.Length - 1);
                    }
                    sizeOfMap = Array.ConvertAll(line.Split(' '), int.Parse);
                    map = new char[sizeOfMap[0], sizeOfMap[1]]; // y and x
                    // Заполняю карту из .txt документа
                    for (int y = 0; y < sizeOfMap[0]; y++)
                    {
                        char[] charasters = null;
                        try
                        {
                            line = myFile.ReadLine();
                            if(String.IsNullOrWhiteSpace(line))
                            {
                                throw new ArgumentNullException();
                            }
                            else if(line.Last() == ' ')
                            {
                                line = line.Remove(line.Length - 1);
                            }
                            // строка, которая будет разбита на символы
                            charasters = Array.ConvertAll(line.Split(' '), char.Parse);
                        }
                        catch (ArgumentNullException e)
                        {
                            throw new ArgumentException($"It look like the map dosn't have {y + 1} row");
                        }

                        if(charasters.Length < sizeOfMap[1])
                        {
                            throw new ArgumentException($"The {y + 1} row is too short!");
                        }

                        for (int x = 0; x < sizeOfMap[1]; x++)
                        {
                            map[y, x] = charasters[x];
                        }

                        
                    }
                }

#if false
                Console.WriteLine("I've finished read the map!");
#endif

                return map;
            }
            else
            {
                throw new System.ArgumentException($"There is no such file: {nameOfMap}");
            }
        }

        /// <summary>
        /// Returns first founded element in the map.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>Returns position of the element.</returns>
        public Point ReturnAnElementPositionOnMap(char element)
        {
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    if (Map[y, x] == element)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return Point.nullPoint;
        }

        /// <summary>
        /// Returns all founded elements in the map.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>Return an array of positions of the elements.</returns>
        public Point[] ReturnElementsPositionsOnMap(char element)
        {
  
            List<Point> result = new List<Point>();
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    if (Map[y, x] == element)
                    {
                        result.Add(new Point(x, y));
                    }
                }
            }

            if(result.Any() == true)
            {
                return result.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// Checks if there is an element in the map.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>Returns true is there is the element, otherwise false.</returns>
        public bool IsThereElementOnMap(char element)
        {
            if (ReturnAnElementPositionOnMap(element).Equals(Point.nullPoint))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsThereFireOnMap()
        {
            for(int i = 1; i < 6; i++)
            {
                if (!ReturnAnElementPositionOnMap((char)i).Equals(Point.nullPoint))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the nearest neigbors of the point.
        /// </summary>
        /// <param name="point">Main point</param>
        /// <returns>Returns an array of points.</returns>
        public Point[] ReturnNeighbours(Point point)
        {

            List<Point> neigbours = new List<Point>();

            if (point.X != 0)
            {
                Point leftPoint = new Point(point, -1, Point.Key.X);
                char _point = ReturnObject(leftPoint);
                if (_point != 'X' && _point != '5')
                    neigbours.Add(leftPoint);
            }
            if (point.X != Map.GetLength(1) - 1)
            {
                Point rightPoint = new Point(point, 1, Point.Key.X);
                char _point = ReturnObject(rightPoint);
                if (_point != 'X' && _point != '5')
                    neigbours.Add(rightPoint);
            }
            if (point.Y != 0)
            {
                Point upperPoint = new Point(point, -1, Point.Key.Y);
                char _point = ReturnObject(upperPoint);
                if (_point != 'X' && _point != '5')
                    neigbours.Add(upperPoint);
            }
            if (point.Y != Map.GetLength(0) - 1)
            {
                Point belowPoint = new Point(point, 1, Point.Key.Y);
                char _point = ReturnObject(belowPoint);
                if (_point != 'X' && _point != '5')
                    neigbours.Add(belowPoint);
            }
            return neigbours.ToArray();
        }
    }
}
