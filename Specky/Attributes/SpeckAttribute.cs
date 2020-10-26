using Specky.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Specky.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SpeckAttribute : Attribute
    {
        public SpeckAttribute(DeliveryMode deliveryMode = default, Type[] interfaces = null, string[] profiles = null)
        {
            DeliveryMode = deliveryMode;
            Interfaces = interfaces?.ToList().AsReadOnly() ?? Enumerable.Empty<Type>();
            Profiles = profiles?.ToList().AsReadOnly() ?? Enumerable.Empty<string>();
        }

        public DeliveryMode DeliveryMode { get; }
        public IEnumerable<Type> Interfaces { get; }
        public IEnumerable<string> Profiles { get; }
    }
}
