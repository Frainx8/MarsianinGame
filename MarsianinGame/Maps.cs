using System;
using System.Collections.Generic;
using System.IO;

namespace MarsianinGame
{
    public class Maps
    {
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

        public char ReturnObject(Point point)
        {
            return Map[point.Y, point.X];
        }

        public void DeleteObject(Point position)
        {
            ChangeObject(position, '.');
        }

        public void ChangeObject(Point position, char _object)
        {
            Map[position.Y, position.X] = _object;
        }

        private char[,] ReadTxt(string nameOfMap) // Функция для чтения .txt файла
        {
            if (File.Exists(nameOfMap))
            {
                char[,] map; // Создаю двумерный массив карты, где будут лежать все игровые объекты
                using (StreamReader myFile = new StreamReader(nameOfMap))
                {
                    // Пытаюсь получить размер карты
                    int[] sizeOfMap = Array.ConvertAll(myFile.ReadLine().Split(' '), int.Parse);
                    if(sizeOfMap.Length > 2)
                    {
                        throw new System.ArgumentException("There are excess numbers in the first string", "nameOfMap");
                    }
                    map = new char[sizeOfMap[0], sizeOfMap[1]]; // y and x
                    // Заполняю карту из .txt документа
                    for (int y = 0; y < sizeOfMap[0]; y++)
                    {
                        char[] charasters = Array.ConvertAll(myFile.ReadLine().Split(' '), char.Parse); // строка, которая будет разбита на символы
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

        public Point ReturnAnElementPosition(char element)
        {
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                for (int x = 0; x < Map.GetLength(0); x++)
                {
                    if (Map[y, x] == element)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(-1, -1);
        }

        public bool IsThereElement(char element)
        {
            if (ReturnAnElementPosition(element).Equals(new Point(-1, -1)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<Point> ReturnNeighbours(Point point)
        {

            List<Point> neigbours = new List<Point>();

            if (point.X != 0)
            {
                Point leftPoint = new Point(point, -1, Point.Key.X);
                if(ReturnObject(leftPoint) != 'X')
                    neigbours.Add(leftPoint);
            }
            if (point.X != Map.GetLength(0) - 1)
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
            if (point.Y != Map.GetLength(1) - 1)
            {
                Point belowPoint = new Point(point, 1, Point.Key.Y);
                if (ReturnObject(belowPoint) != 'X')
                    neigbours.Add(belowPoint);
            }
            return neigbours;
        }
    }
}
