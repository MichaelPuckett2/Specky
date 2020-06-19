using System;

namespace Specky.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SpeckyFactoryAttribute : SpeckAttribute
    {
        public SpeckyFactoryAttribute(string factoryMethodName)
        {
            FactoryMethodName = factoryMethodName;
        }

        public string FactoryMethodName { get; }
    }
}
