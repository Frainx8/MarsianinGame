using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmLibrary
{
    public class Permutations
    {
        public static List<string[]> Permute(IList<string> set)
        {
            List<string[]> result = new List<string[]>();

            Action<int> permute = null;
            permute = start =>
            {
                if (start == set.Count)
                {
                    result.Add(set.ToArray());
                }
                else
                {
                    List<string> swaps = new List<string>();
                    for (int i = start; i < set.Count; i++)
                    {
                        if (swaps.Contains(set[i])) continue; // skip if we already done swap with this item
                        swaps.Add(set[i]);

                        Swap(set, start, i);
                        permute(start + 1);
                        Swap(set, start, i);
                    }
                }
            };

            permute(0);

            return result;
        }

        private static void Swap(IList<string> set, int index1, int index2)
        {
            string temp = set[index1];
            set[index1] = set[index2];
            set[index2] = temp;
        }

    }
}
