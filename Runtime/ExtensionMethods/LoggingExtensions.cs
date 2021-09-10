using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DrakesGames.Extensions
{
    public static class LoggingExtensions
    {
        public static void LogElementsToConsole<T>(this List<T> list)
        {
            StringBuilder stringBuilder = new StringBuilder("Elements of ");
            stringBuilder.Append(nameof(list) + ": ");
            foreach (var element in list)
            {
                stringBuilder.Append(element.ToString() + ", ");
            }
            Debug.Log(stringBuilder.ToString());
        }

        public static void LogElementsToConsole<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            StringBuilder stringBuilder = new StringBuilder("Elements of ");
            stringBuilder.Append(nameof(dictionary) + ": \n");
            foreach (var element in dictionary)
            {
                stringBuilder.Append("[" + element.Key + ", " + element.Value + "] \n");
            }
            
            Debug.Log(stringBuilder.ToString());
        }
    }

}
