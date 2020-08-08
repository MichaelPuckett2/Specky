namespace Specky.Enums
{
    /// <summary>
    /// Used to set the lifetime of each Speck instantiated per injection.
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// Instantiated for every injection.
        /// </summary>
        Transient,
        /// <summary>
        /// Instantiated once and injected as the same instance per user session for stateful applications like ASP.NET Core, Blazor, Web API...
        /// </summary>
        Scoped,
        /// <summary>
        /// Instantiated once and injected as a single instance for the life of the application.
        /// </summary>
        Singleton        
    }
}
