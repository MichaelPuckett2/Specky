using Specky.Enums;
using System;

namespace Specky.Dtos
{
    /// <summary>
    /// The Speck and related configurations.
    /// </summary>
    public class SpeckDto
    {
        internal SpeckDto(
            Type type,
            Lifetime lifeTIme,
            Role role,
            string configuration,
            string name,
            object instance)
        {
            Type = type;
            Lifetime = lifeTIme;
            Role = role;
            Configuration = configuration;
            Name = name;
            Instance = instance;
        }

        /// <summary>
        /// The Type of Speck.
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// The lifetime of the Speck.
        /// </summary>
        public Lifetime Lifetime { get; }
        /// <summary>
        /// The role the Speck is configured to run as.
        /// </summary>
        public Role Role { get; }
        /// <summary>
        /// The configuration required to inject the Speck.
        /// </summary>
        public string Configuration { get; }
        /// <summary>
        /// The name used to reference the Speck.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The instantiated Speck; if available.
        /// </summary>
        public object Instance { get; }
    }
}
