using Specky.Attributes;
using Specky.Injection;
using Specky.Ioc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Specky
{
    public static class Manager
    {
        public static IContainer Container { get; } = new Container(new SpeckActivator());

        public static IAutoStrapper AutoStrapper { get; } = new AutoStrapper();
    }

    public interface IAutoStrapper
    {
        void Start();
    }

    public class AutoStrapper : IAutoStrapper
    {
        public void Start()
        {
            Assembly[] assemblies = new Assembly[] { Assembly.GetCallingAssembly() };

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var speckAttributes = type.GetCustomAttributes().Where(x => x is SpeckAttribute).Select(x => (SpeckAttribute)x);

                    foreach (var speckAttribute in speckAttributes)
                    {
                        var speck = Manager.Container.SetSpeck(type);
                        SetDeliveryMode(speckAttribute, speck);
                        SetInterfaces(speckAttribute, speck);
                    }
                }
            }
        }

        private static void SetDeliveryMode(SpeckAttribute speckAttribute, Speck speck)
        {
            switch (speckAttribute.DeliveryMode)
            {
                case Enums.DeliveryMode.Transient:
                    speck.AsTransient();
                    break;
                case Enums.DeliveryMode.Scoped:
                    speck.AsScoped();
                    break;
                case Enums.DeliveryMode.Singleton:
                default:
                    speck.AsSingleton();
                    break;
            }
        }

        private static void SetInterfaces(SpeckAttribute speckAttribute, Speck speck)
        {
            if (speckAttribute.Interfaces == null) return;
            foreach (var intfce in speckAttribute.Interfaces)
            {
                speck.As(intfce);
            }
        }
    }
}
