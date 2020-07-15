using System;
using System.IO;

namespace MarsianinGame
{
    class Maps
    {
        public Maps(string nameOfMap)
        {
            Map = ReadTxt(nameOfMap);
        }

        public char[,] Map { get; private set; }

        public char ReturnObject(Point point)
        {
            return Map[point.Y, point.X];
        }

        public void DeleteObject(Point position)
        {
            Map[position.Y, position.X] = '.';
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

        public Point ReturnStartPoint()
        {
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                for (int x = 0; x < Map.GetLength(0); x++)
                {
                    if (Map[y, x] == 'S')
                    {
                        return new Point(x, y);
                    }
                }
            }
            throw new System.ArgumentException("There is no 'S' in the map!", "Map");
        }

        
    }
}
