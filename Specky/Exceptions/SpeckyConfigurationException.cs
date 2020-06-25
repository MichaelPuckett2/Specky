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
        public SpeckyFactoryMethodNotFoundException(Type type) 
            : base($"No factory method found for {nameof(SpeckyFactoryAttribute)} | {type.Name}") { }
    }
}
