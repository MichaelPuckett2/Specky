using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specky.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToDelimitedString(this IEnumerable<string> stringEnumeration, string delimiter)
        {
            stringEnumeration = stringEnumeration.ToList();

            var stringBuilder = new StringBuilder();

            var enumerator = stringEnumeration.GetEnumerator();
            enumerator.MoveNext();
            stringBuilder.Append(enumerator.Current);

            if (enumerator.MoveNext())
                stringBuilder.Append($"{delimiter}{enumerator.Current}");

            return stringBuilder.ToString();
        }
    }
}
