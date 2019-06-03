using System.Reflection;

namespace Specky
{
    internal static class Constants
    {
        internal static BindingFlags BindingFlags = BindingFlags.Instance
                                                  | BindingFlags.Public
                                                  | BindingFlags.NonPublic;
    }
}
