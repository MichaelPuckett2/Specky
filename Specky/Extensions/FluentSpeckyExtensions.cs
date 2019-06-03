using Specky.Enums;
using System;
using System.Collections.Generic;
using static Specky.Logging.Log;

namespace Specky.Extensions
{
    public static class FluentSpeckyExtensions
    {
        public static TEnumerable WithEach<TEnumerable, T>(this TEnumerable items, Action<T> action) where TEnumerable : IEnumerable<T>
        {
            foreach (var item in items) action.Invoke(item);
            return items;
        }

        public static IEnumerable<T> Single<T>(this T item)
        {
            yield return item;
        }

        public static T With<T>(this T t, Action<T> action)
        {
            action.Invoke(t);
            return t;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) action.Invoke(item);
            return enumerable;
        }

        public static T Log<T>(this T t, string message, PrintType printType)
        {
            Print(message, printType);
            return t;
        }
    }
}
