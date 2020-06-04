using Specky.Enums;

namespace Specky.Attributes
{
    /// <summary>
    /// Injects a Type as a Speck dependency with an optional name for referencing.
    /// </summary>
    public class SpeckNameAttribute : SpeckAttribute
    {
        public SpeckNameAttribute(string speckName, DeliveryMode deliveryMode = default, string configuration = "") : base(deliveryMode, configuration)
        {
            SpeckName = speckName;
        }

        public string SpeckName { get; }
    }
}
