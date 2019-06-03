using Specky.Enums;
using System;

namespace Specky.Logging
{
    internal static class Log
    {
        public static void Print(string message, PrintType printType, Exception innerException = null)
        {
            if (printType.HasFlag(PrintType.DebugWindow))
            {
                System.Diagnostics.Debug.Print($"** SPECKY :: {message}");
            }

            if (printType.HasFlag(PrintType.LogFile))
            {
                throw new NotImplementedException($"{nameof(Log)}.{nameof(Print)} does not yet support {nameof(PrintType.LogFile)}");
            }

            if (printType.HasFlag(PrintType.ThrowException))
            {
                throw innerException == null
                    ? throw new Exception(message)
                    : new Exception(message, innerException);
            }
        }
    }
}
