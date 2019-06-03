using System;

namespace Specky.Extensions
{
    public static class BooleanExtensions
    {
        /// <summary>
        /// Invokes seperate actions when boolean is true or false.
        /// </summary>
        /// <param name="boolean"></param>
        /// <param name="True">Action to invoke when boolean is true.</param>
        /// <param name="False">Action to invoke when boolean is false.</param>
        public static bool Pulse(this bool boolean, Action True = null, Action False = null)
        {
            if (boolean) True?.Invoke();
            else False?.Invoke();
            return boolean;
        }

        /// <summary>
        /// Invokes a predicate when boolean is true.
        /// </summary>
        /// <param name="boolean"></param>
        /// <param name="predicate">Predicate to invoke when boolean is true.</param>
        /// <returns>Returns the result of the invoked predicate.</returns>
        public static bool PulseOnTrue(this bool boolean, Predicate<bool> predicate)
        {
            if (boolean) return predicate.Invoke(boolean); else return boolean;
        }

        /// <summary>
        /// Invokes a predicate when boolean is false.
        /// </summary>
        /// <param name="boolean"></param>
        /// <param name="predicate">Predicate to invoke when boolean is false.</param>
        /// <returns>Returns the result of the invoked predicate.</returns>
        public static bool PulseOnFalse(this bool boolean, Predicate<bool> predicate)
        {
            if (!boolean) return predicate.Invoke(boolean); else return boolean;
        }

        /// <summary>
        /// Invokes seperate predicates when boolean is true or false.
        /// </summary>
        /// <param name="boolean"></param>
        /// <param name="truePredicate">Predicate to invoke when boolean is true.</param>
        /// <param name="falsePredicate">Predicate to invoke when boolean is false.</param>
        /// <returns>Returns the result of the invoked predicate.</returns>
        public static bool Pulse(this bool boolean, Predicate<bool> truePredicate = null, Predicate<bool> falsePredicate = null)
        {
            if (boolean) return boolean.PulseOnTrue(truePredicate);
            else return boolean.PulseOnFalse(falsePredicate);
        }
    }
}
