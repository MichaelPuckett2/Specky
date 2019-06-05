using System;

namespace Specky.Attributes
{
    /// <summary>
    /// Injects a speck which targets the SpeckyConfigurationAttribute for injection into similar types.
    /// Can also be used on constructor parameters to define specific configurations during injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class SpeckyConfigurationAttribute : SpeckAttribute
    {
        /// <summary>
        /// Initializes the SpeckyConfigurationAttribute with a configuration name.
        /// </summary>
        /// <param name="configurationName">Configuration name.</param>
        public SpeckyConfigurationAttribute(string configurationName) : base(Enums.DeliveryMode.DataSet)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                throw new ArgumentException($"{nameof(configurationName)} cannot be null, empty, or white space.", nameof(configurationName));
            }

            ConfigurationName = configurationName;
        }

        /// <summary>
        /// The name of the configuration.
        /// </summary>
        public string ConfigurationName { get; }
    }
}
