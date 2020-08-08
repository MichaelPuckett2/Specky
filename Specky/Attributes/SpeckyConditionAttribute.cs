using Specky.Enums;

namespace Specky.Attributes
{
    /// <summary>
    /// Customizable attribute to inject a Speck based on a condition.
    /// </summary>
    abstract public class SpeckyConditionAttribute : SpeckAttribute
    {
        public SpeckyConditionAttribute(Lifetime deliveryMode = default, string configuration = "") : base(deliveryMode, configuration) { }
        abstract public bool TestCondition();
    }
}
