using Specky.Enums;
using System;

namespace Specky.Attributes
{
    /// <summary>
    /// Injects a type as a Speck dependency
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SpeckAttribute : Attribute
    {
        public SpeckAttribute(DeliveryMode deliveryMode = default, string configuration = "")
        {
            DeliveryMode = deliveryMode;
            Configuration = configuration;
        }
        public DeliveryMode DeliveryMode { get; }
        public string Configuration { get; }
    }
}
