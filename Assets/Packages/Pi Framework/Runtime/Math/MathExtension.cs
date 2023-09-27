using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Math
{
    public static class MathExtension
    {
        //Shuffle any (I)List with an extension method
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            T value;
            while (n > 1)
            {
                int k = Random.Range(0, n);
                n--;
                value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}