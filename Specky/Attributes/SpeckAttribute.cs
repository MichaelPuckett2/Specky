using Specky.Enums;
using System;

namespace Specky.Attributes
{
    /// <summary>
    /// Injects a type as a Speck dependency.
    /// Can also be used on SpeckyFactory to represent the injeciton method and type of Speck captured.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SpeckAttribute : Attribute
    {
        public SpeckAttribute(Lifetime deliveryMode = default, string configuration = "")
        {
            DeliveryMode = deliveryMode;
            Configuration = configuration;
        }
        public Lifetime DeliveryMode { get; }
        public string Configuration { get; }
    }
}
