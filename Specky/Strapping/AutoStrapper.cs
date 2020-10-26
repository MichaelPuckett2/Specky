using Specky.Attributes;
using Specky.Extensions;
using Specky.Ioc;
using Specky.Models;
using System.Linq;
using System.Reflection;

namespace Specky.Strapping
{
    public class AutoStrapper : IAutoStrapper
    {
        private readonly IContainer container;

        public AutoStrapper(IContainer container)
        {
            this.container = container;
        }
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
                        var speck = container.SetSpeck(type);
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
