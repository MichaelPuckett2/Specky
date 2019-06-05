using System;

namespace Specky.Exceptions
{
    public class SpeckyConfigurationException : Exception
    {
        public SpeckyConfigurationException(string message) : base(message) { }
    }
}
