namespace Specky.Enums
{
    /// <summary>
    /// Dictates the scope of the Speck to be delivered.
    /// SingleInstance returns the same Speck throughout the lifetime of the application and PerRequest will return a new Speck each time it's requested.
    /// </summary>
    public enum DeliveryMode
    {
        /// <summary>
        /// A single Speck to be returned once during the lifetime of the application.
        /// </summary>
        SingleInstance,
        /// <summary>
        /// A new Speck returned on each request.
        /// </summary>
        PerRequest,
        /// <summary>
        /// An enumeration of Specks or DataSet containing Speck'd values.
        /// </summary>
        DataSet,
        /// <summary>
        /// For AspNetCore: The same as PerRequest.
        /// Registers as AddTransient with IServiceProvider.
        /// </summary>
        Transient,
        /// <summary>
        /// For AspNetCore:  The same as PerRequest.
        /// Registers as AddScoped with IServiceProvider.
        /// </summary>
        Scoped,
        /// <summary>
        /// For AspNetCore:  The same as SingleInstance.
        /// Registers as AddSingleton with IServiceProvider
        /// </summary>
        Singleton        
    }
}
