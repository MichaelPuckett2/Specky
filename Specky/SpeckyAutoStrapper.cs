using Specky.DI;
using Specky.Enums;
using Specky.Logging;
using System.Collections.Generic;
using System.Reflection;

namespace Specky
{
    public static class SpeckyAutoStrapper
    {
        private static bool StartCalled;
        private static readonly object lockObject = new object();
        /// <summary>
        /// Starts the strapping and injection process.
        /// Note: It is important that this is performed first in your application and in the main application threading context.
        /// </summary>
        /// <param name="assemblies">Optional assemblies used to search for dependencies.</param>
        public static void Start(IEnumerable<Assembly> assemblies = null)
        {
            lock (lockObject)
            {
                if (StartCalled)
                {
                    Log.Print($"{nameof(SpeckyAutoStrapper)}.{nameof(Start)} called but strapping is already started. Returning from call...", PrintType.DebugWindow);
                    return;
                }

                StartCalled = true;

                var allAssemblies = assemblies ?? new List<Assembly>() { Assembly.GetCallingAssembly() };

                Log.Print("Strapping...", PrintType.DebugWindow);

                new InjectionWorker(allAssemblies).Start();

                Log.Print("Strapped.", PrintType.DebugWindow);
            }
        }
    }
}
