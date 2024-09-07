using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Utilities
{
    public static class Shuffler
    {
        public static System.Random r = new System.Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while(n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    
        public static void SortTransformsByDistance(this IList<Transform> list, Vector3 target)
        {
            List<Transform> sortedList = new List<Transform>();
            List<float> distances = new List<float>();
        
            foreach(Transform item in list)
            {
                float distance = Vector3.Distance(item.position, target);
                int i = 0;
                for(; i < distances.Count; i++)
                {
                    if(distance < distances[i])
                    {
                        break;
                    }
                }
                distances.Insert(i, distance);
                sortedList.Insert(i, item);
            }
        }
    }
}