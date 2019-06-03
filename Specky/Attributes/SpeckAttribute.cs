using Specky.Enums;
using System;

namespace Specky.Attributes
{
    /// <summary>
    /// Injects a class as a Speck dependency
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SpeckAttribute : Attribute
    {
        public SpeckAttribute() : this(default) { }
        public SpeckAttribute(DeliveryMode deliveryMode) => DeliveryMode = deliveryMode;
        public DeliveryMode DeliveryMode { get; }
    }
}
