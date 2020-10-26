using Specky.Injection;
using Specky.Ioc;
using Specky.Strapping;

namespace Specky
{
    public static class Manager
    {
        static Manager()
        {
            Container = new Container(new SpeckActivator());
            AutoStrapper = new AutoStrapper(Container);
        }
        public static IContainer Container { get; } 
        public static IAutoStrapper AutoStrapper { get; }
    }
}
