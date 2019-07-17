using Specky.Enums;

namespace Specky.Attributes
{
    /// <summary>
    /// Customizable attribute to inject a Speck based on a condition.
    /// </summary>
    abstract public class SpeckConditionAttribute : SpeckAttribute
    {
        public SpeckConditionAttribute(DeliveryMode deliveryMode = default) : base(deliveryMode) { }
        abstract public bool TestCondition();
    }
}
