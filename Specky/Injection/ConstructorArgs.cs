using System.Reflection;

namespace Specky.Injection
{
    public class ConstructorArgs
    {
        public ConstructorInfo ConstructorInfo { get; internal set; }
        public ParameterInfo[] Parameters { get; internal set; }
    }
}
