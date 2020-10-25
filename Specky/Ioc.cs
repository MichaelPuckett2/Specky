using Specky.Enums;
using Specky.Exceptions;
using Specky.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Specky
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

    public static class SpeckExtensions
    {
        /// <summary>
        /// Gives the Speck a DeliveryMode of Singleton.
        /// </summary>
        /// <param name="speck">The Speck being Set.</param>
        /// <returns>The Speck being Set.</returns>
        public static Speck AsSingleton(this Speck speck)
        {
            speck.DeliveryMode = DeliveryMode.Singleton;
            return speck;
        }

        /// <summary>
        /// Gives the Speck a DeliveryMode of Scoped.
        /// </summary>
        /// <param name="speck">The Speck being Set.</param>
        /// <returns>The Speck being Set.</returns>
        public static Speck AsScoped(this Speck speck)
        {
            speck.DeliveryMode = DeliveryMode.Scoped;
            return speck;
        }

        /// <summary>
        /// Gives the Speck a DeliveryMode of Transient.
        /// </summary>
        /// <param name="speck">The Speck being Set.</param>
        /// <returns>The Speck being Set.</returns>
        public static Speck AsTransient(this Speck speck)
        {
            speck.DeliveryMode = DeliveryMode.Transient;
            return speck;
        }

        /// <summary>
        /// Assigns a specific interface (or set of interfaces) to the Speck.
        /// By default the Speck will utilize all of it's given interfaces but by making it specific you can ignore interfaces you do not wish the Speck to be viewed as.
        /// </summary>
        /// <typeparam name="T">A specific interface the Speck will inject against.</typeparam>
        /// <param name="speck">The Speck being assigned the interface.</param>
        /// <returns>The Speck being assigned the interface.</returns>
        public static Speck As<T>(this Speck speck)
        {
            if (!speck.Interfaces.Contains(typeof(T))) speck.AddInterface<T>();
            return speck;
        }

        /// <summary>
        /// Assigns a specific interface (or set of interfaces) to the Speck.
        /// By default the Speck will utilize all of it's given interfaces but by making it specific you can ignore interfaces you do not wish the Speck to be viewed as.
        /// </summary>
        /// <typeparam name="T">A specific interface the Speck will inject against.</typeparam>
        /// <param name="speck">The Speck being assigned the interface.</param>
        /// <returns>The Speck being assigned the interface.</returns>
        public static Speck As(this Speck speck, Type type)
        {
            if (!speck.Interfaces.Contains(type)) speck.AddInterface(type);
            return speck;
        }

    }
}
