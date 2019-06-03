using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Specky.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Retrieves types that have attribute of type T.
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="assembly">The assembly to search.</param>
        /// <returns>IEnumerable&lt;Type, T&gt; of types that have the T attribute.</returns>
        public static IEnumerable<(Type Type, T Attribute)> TypesWithAttribute<T>(this Assembly assembly, Predicate<T> when = null) where T : Attribute
        {
            if (when == null) when = (obj) => true;
            return from type in assembly.GetTypes()
                   let attribute = type.GetCustomAttribute<T>(true)
                   where attribute != null
                   where when.Invoke(attribute)
                   select (type, attribute);
        }
    }
}
