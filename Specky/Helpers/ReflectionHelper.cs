using System.Reflection;

namespace Specky.Helpers
{
    public static class ReflectionHelper
    {
        public static Assembly GetAssembly<T>() => Assembly.GetAssembly(typeof(T));
    }
}
