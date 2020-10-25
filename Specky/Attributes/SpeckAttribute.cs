using Specky.Enums;
using System;
using System.Collections.Generic;

namespace Specky.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SpeckAttribute : Attribute
    {
        public SpeckAttribute(DeliveryMode deliveryMode = default, Type[] interfaces = null)
        {
            DeliveryMode = deliveryMode;
            Interfaces = interfaces;
        }

        public DeliveryMode DeliveryMode { get; }
        public IEnumerable<Type> Interfaces { get; }
    }
}
