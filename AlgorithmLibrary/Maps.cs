﻿using System;
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
        
        public Maps(string nameOfMap)
        {
            Map = ReadTxt(nameOfMap);
            if (IsThereElement('Q') == false)
            {
                throw new System.ArgumentException("There is no exit!", "Map");
            }
            if (IsThereElement('S') == false)
            {
                throw new System.ArgumentException("There is no enter!", "Map");
            }
        }

        public char[,] Map { get; private set; }

        /// <summary>
        /// Returns contained object in the point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public char ReturnObject(Point point)
        {
            return Map[point.Y, point.X];
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
                char[,] map; // Создаю двумерный массив карты, где будут лежать все игровые объекты
                using (StreamReader myFile = new StreamReader(nameOfMap))
                {
                    // Пытаюсь получить размер карты
                    int[] sizeOfMap = null;
                    try
                    {
                        sizeOfMap = Array.ConvertAll(myFile.ReadLine().Split(' '), int.Parse);
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine($"It look like you have excess spacebar in the first line!");
                        throw;
                    }
                    map = new char[sizeOfMap[0], sizeOfMap[1]]; // y and x
                    // Заполняю карту из .txt документа
                    for (int y = 0; y < sizeOfMap[0]; y++)
                    {
                        char[] charasters = null;
                        try
                        {
                            string line = myFile.ReadLine();
                            if(line == null)
                            {
                                throw new ArgumentNullException();
                            }
                            // строка, которая будет разбита на символы
                            charasters = Array.ConvertAll(line.Split(' '), char.Parse);
                        }
                        catch (ArgumentNullException e)
                        {
                            Console.WriteLine($"It look like the map dosn't have {y + 1} row");
                            throw;
                        }
                        catch (System.FormatException e)
                        {
                            Console.WriteLine($"It look like you have excess spacebar in the {y+1} row");
                            throw;
                        }

                        for (int x = 0; x < sizeOfMap[1]; x++)
                        {
                            map[y, x] = charasters[x];
                        }

                        
                    }
                }
                
                return map;
            }
            else
            {
                throw new System.ArgumentException("There is no such file", "nameOfMap");
            }
        }

        /// <summary>
        /// Returns first founded element in the map.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>Returns position of the element.</returns>
        public Point ReturnAnElementPosition(char element)
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
        public Point[] ReturnElementsPositions(char element)
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
        public bool IsThereElement(char element)
        {
            if (ReturnAnElementPosition(element).Equals(Point.nullPoint))
            {
                return false;
            }
            else
            {
                return true;
            }
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
                if(ReturnObject(leftPoint) != 'X')
                    neigbours.Add(leftPoint);
            }
            if (point.X != Map.GetLength(1) - 1)
            {
                Point rightPoint = new Point(point, 1, Point.Key.X);
                if (ReturnObject(rightPoint) != 'X')
                    neigbours.Add(rightPoint);
            }
            if (point.Y != 0)
            {
                Point upperPoint = new Point(point, -1, Point.Key.Y);
                if (ReturnObject(upperPoint) != 'X')
                    neigbours.Add(upperPoint);
            }
            if (point.Y != Map.GetLength(0) - 1)
            {
                Point belowPoint = new Point(point, 1, Point.Key.Y);
                if (ReturnObject(belowPoint) != 'X')
                    neigbours.Add(belowPoint);
            }
            return neigbours.ToArray();
        }

        /// <summary>
        /// Used for debuging a point in the console.
        /// </summary>
        /// <param name="point"></param>
        public void WritePointInConsole(Point point)
        {
            Console.WriteLine(point.X + " -" + point.Y);
        }
    }
}