using System;
using System.Runtime.CompilerServices;

namespace Specky.Exceptions
{
    public abstract class SpeckException : Exception
    {
        public SpeckException(string message) : base(message) { }
    }

    public class NotAnInterfaceSpeckException : SpeckException
    {
        internal NotAnInterfaceSpeckException(Type type, [CallerMemberName] string caller = "") : base($"{type.Name} is not an interface; however an interface was expected @{caller}.")
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Type Type { get; }
    }

    public class SpeckNotFoundException : SpeckException
    {
        internal SpeckNotFoundException(Type type, [CallerMemberName] string caller = "") : base($"Speck not found for {type.Name} @{caller}.")
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Type Type { get; }
    }

    public class SpeckNullException : SpeckException
    {
        internal SpeckNullException([CallerMemberName] string caller = "") : base($"Speck null @{caller}.") { }
    }

    public class NoValidConstructorFound : SpeckException
    {
        internal NoValidConstructorFound(Type type, [CallerMemberName] string caller = "") : base($"Specky did not find a valid constrcutor to instantiate the type {type.Name}. @{caller}.")
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Type Type { get; }
    }
}
