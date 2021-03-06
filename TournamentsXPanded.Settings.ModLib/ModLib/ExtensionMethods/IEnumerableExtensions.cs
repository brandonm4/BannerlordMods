﻿using System;
using System.Collections.Generic;

namespace TXP.ModLib
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Applies the action to all elements in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable != null)
            {
                foreach (var item in enumerable)
                    action?.Invoke(item);
                return enumerable;
            }
            else
                return null;
        }
    }
}
