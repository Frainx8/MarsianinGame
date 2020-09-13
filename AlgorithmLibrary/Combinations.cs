using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmLibrary
{
    public static class Combinations
    {
        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).GetCombinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }

        public static Point[][] MyCombinations(Point[] arr, int len)
        {
            List<Point[]> result = new List<Point[]>();
            Point[] combination = new Point[len];
            if (len == arr.Length)
            {
                result.Add(arr);
            }
            else
            {
                for (int i = 0; i <= arr.Length - len; i++)
                {
                    combination[combination.Length - len] = arr[i];

                    Point[] copyOfCombination = combination.ToArray();

                    result = MyCombinations(arr, len - 1, i + 1, copyOfCombination, result);
                }
            }

#if false
            foreach (var item in result)
            {
                foreach (Point point in item)
                {
                    Console.Write(point + "; ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
#endif

            Point[][] _result = new Point[result.Count()][];
            for (int i = 0; i < result.Count(); i++)
            {
                for (int j = 0; j < len; j++)
                {
                    _result[i] = new Point[len];
                    _result[i][j] = result[i][j];

                }
            }
            return _result;
        }

        public static List<Point[]> MyCombinations(Point[] arr, int len, int startPosition,
             Point[] combination, List<Point[]> result)
        {
            if (len == 0)
            {
#if false
                Console.WriteLine("In function");
                foreach (var item in combination)
                {
                    Console.Write(item + "; ");
                }
                Console.WriteLine();
#endif
                result.Add(combination);
                return result;
            }
            for (int i = startPosition; i <= arr.Length - len; i++)
            {
#if false
                Console.WriteLine("Before");
                foreach (var item in result)
                {
                    foreach (var item2 in item)
                    {
                        Console.Write(item2 + "; ");
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
#endif
                combination[combination.Length - len] = arr[i];
                Point[] copyOfCombination = combination.ToArray();
                result = MyCombinations(arr, len - 1, i + 1, copyOfCombination, result);
#if false
                Console.WriteLine("After");
                foreach(var item in result)
                {
                    foreach (var item2 in item)
                    {
                        Console.Write(item2 + "; ");
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
#endif
            }
            return result;
        }
    }
}
