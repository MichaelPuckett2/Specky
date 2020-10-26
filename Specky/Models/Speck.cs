using Specky.Enums;
using Specky.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Specky.Models
{
    public class Speck
    {
        private readonly List<Type> interfaces = new List<Type>();

        /// <summary>
        /// The Type this Speck will instantiate.
        /// </summary>
        public Type Type { get; internal set; }

        /// <summary>
        /// The DeliveryMode of the Speck when instantiated.
        /// </summary>
        public DeliveryMode DeliveryMode { get; internal set; }

        /// <summary>
        /// The interfaces the Speck will instantiate against.
        /// If left empty the Speck will automatically instantiate against any of it's given interfaces.
        /// </summary>
        public IEnumerable<Type> Interfaces => interfaces.ToList().AsReadOnly();

        /// <summary>
        /// The instance of the Speck is one has been instantiated and the DeliveryMode allows holding of the instance; othwersise the instance is null;
        /// </summary>
        public object Instance { get; internal set; }

        internal void AddInterface<T>() => AddInterface(typeof(T));
        internal void AddInterface(Type type)
        {
            if (type.IsInterface)
            {
                interfaces.Add(type);
            }
            else
            {
                throw new NotAnInterfaceSpeckException(type);
            }
        }
    }
}
