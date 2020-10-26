using Specky.Enums;
using Specky.Models;
using System;
using System.Linq;

namespace Specky.Extensions
{
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
