using System.Collections.Generic;
using System.Linq;

namespace AlgorithmLibrary
{
    public static class Combinations
    {
        public static IEnumerable<IEnumerable<T>> FindCombinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).FindCombinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }
}
