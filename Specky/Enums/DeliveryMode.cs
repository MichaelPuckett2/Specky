namespace Specky.Enums
{
    public enum DeliveryMode
    { 
        /// <summary>
        /// A single instance for the lifetime of the application.
        /// </summary>
        Singleton,
        /// <summary>
        /// A new instance for every request.
        /// </summary>
        Transient,
        /// <summary>
        /// A new instance per user session.
        /// </summary>
        Scoped
    }
}
