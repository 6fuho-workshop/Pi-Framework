using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.MathUtils
{
    public static class MathExtensions
    {
        private static readonly System.Random _random = new();

        /// <summary>
        /// Shuffles the elements of the list in-place using UnityEngine.Random for randomness.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = _random.Next(0, n--);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}