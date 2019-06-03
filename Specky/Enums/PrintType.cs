using System;

namespace Specky.Enums
{
    /// <summary>
    /// The flagged type used to print debug messages in Specky.
    /// </summary>
    [Flags]
    public enum PrintType
    {
        /// <summary>
        /// Prints Specky debug statements to the debug window.
        /// </summary>
        DebugWindow = 1,
        /// <summary>
        /// Not yet implemented.
        /// </summary>
        LogFile = 2,
        /// <summary>
        /// Throws an exception with the provided debug statement.
        /// </summary>
        ThrowException = 4
    }
}
