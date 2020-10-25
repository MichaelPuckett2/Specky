using Specky.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Specky.Injection
{
    public interface ISpeckActivator
    {
        T InstantiateSpeck<T>(IEnumerable<Speck> specks);
        object InstantiateSpeck(Type type, IEnumerable<Speck> specks);
    }

    public class SpeckActivator : ISpeckActivator
    {
        public static BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        public T InstantiateSpeck<T>(IEnumerable<Speck> specks) => (T)InstantiateSpeck(typeof(T), specks);

        public object InstantiateSpeck(Type type, IEnumerable<Speck> specks)
        {
            var speck = GetSpeck(type, specks);

            if (speck == null) throw new SpeckNotFoundException(type);

            if (speck.Instance != null)
            {
                return speck.Instance;
            }

            var contructorArgs = GetConstructorArgs(speck.Type);

            foreach (var contructorArg in contructorArgs)
            {
                var parameterSpecks = new List<Speck>();
                var isParametersValid = true;
                foreach (var parameter in contructorArg.Parameters)
                {
                    var paramSpeck = specks.FirstOrDefault(x => x.Type == parameter.ParameterType);
                    if (paramSpeck == null)
                    {
                        isParametersValid = false;
                        break;
                    }
                    else
                    {
                        parameterSpecks.Add(paramSpeck);
                    }
                }
                if (!isParametersValid) continue;

                var instances = parameterSpecks.Select(x => InstantiateSpeck(x.Type, specks)).ToList().AsReadOnly();
                object instance;
                if (!instances.Any())
                {
                    instance = Activator.CreateInstance(speck.Type, true);
                }
                else
                {
                    instance = Activator.CreateInstance(speck.Type, true, instances);
                }
                if (speck.DeliveryMode == Enums.DeliveryMode.Singleton) speck.Instance = instance;
                return instance;
            }

            throw new NoValidConstructorFound(speck.Type);
        }

        private static Speck GetSpeck(Type type, IEnumerable<Speck> specks)
        {
            return specks.FirstOrDefault(x =>
            {
                Type[] interfaces;

                if (!x.Interfaces.Any())
                {
                    interfaces = x.Type.GetInterfaces();
                }
                else
                {
                    interfaces = x.Interfaces.ToArray();
                }

                return x.Type == type || interfaces.Any(intfce => intfce == type);
            });
        }

        private static IEnumerable<ConstructorArgs> GetConstructorArgs(Type type)
        {
            return type
                .GetConstructors(BindingFlags)
                .Select(x => new ConstructorArgs
                {
                    ConstructorInfo = x,
                    Parameters = x.GetParameters()
                })
                .OrderBy(x => x.Parameters.Length)
                .ToList()
                .AsReadOnly();
        }
    }

    public class ConstructorArgs
    {
        public ConstructorInfo ConstructorInfo { get; internal set; }
        public ParameterInfo[] Parameters { get; internal set; }
    }
}
