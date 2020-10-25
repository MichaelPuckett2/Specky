using Specky.Injection;
using System.Collections.Generic;
using System.Linq;

namespace Specky.Ioc
{
    public class Container : IContainer
    {
        private readonly ISpeckActivator speckActivator;

        internal HashSet<Speck> Specks { get; } = new HashSet<Speck>();

        internal Container(ISpeckActivator speckActivator)
        {
            this.speckActivator = speckActivator;
        }

        public T Get<T>() => speckActivator.InstantiateSpeck<T>(Specks);

        public Speck SetSpeck<T>() => SetSpeck(typeof(T));
        public Speck SetSpeck(System.Type type)
        {
            var speck = new Speck { Type = type };
            Specks.Add(speck);
            return speck;
        }
    }
}
