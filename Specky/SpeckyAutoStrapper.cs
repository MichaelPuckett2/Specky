using Specky.DI;
using Specky.Logging;
using Specky.Enums;
using System.Reflection;
using System.Collections.Generic;

namespace Specky
{
    public static class SpeckyAutoStrapper
    {
        private static bool StartCalled;
        /// <summary>
        /// Starts the strapping and injection process.
        /// Note: It is important that this is performed first in your application and in the main application threading context.
        /// </summary>
        /// <param name="profile">The profile used to start strapping.</param>
        public static void Start(IEnumerable<Assembly> assemblies = null)
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
