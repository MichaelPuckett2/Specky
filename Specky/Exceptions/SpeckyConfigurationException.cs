using Specky.Attributes;
using System;

namespace Specky.Exceptions
{
    public class SpeckyConfigurationException : Exception
    {
        public SpeckyConfigurationException(string message) : base(message) { }
    }

    public class SpeckyFactoryMethodNotFoundException : Exception
    {
        public SpeckyFactoryMethodNotFoundException(string factoryMethod, Type type) 
            : base($"Method: {factoryMethod} was not found on {nameof(SpeckyFactoryAttribute)} | {type.Name}") { }
    }
}
