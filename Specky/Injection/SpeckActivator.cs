using Specky.Exceptions;
using Specky.Models;
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

            if (speck.Instance != null) return speck.Instance;

            if (TryConstructInstance(speck, specks, out object instance))
            {
                return instance;
            }
            else
            {
                throw new NoValidConstructorFound(speck.Type);
            }
        }

        private bool TryConstructInstance(Speck speck, IEnumerable<Speck> specks, out object instance)
        {
            var contructorArgs = GetConstructorArgs(speck.Type);
            instance = null;

            foreach (var contructorArg in contructorArgs)
            {
                if (!TrySetParameterSpecks(specks, contructorArg, out IEnumerable<Speck> parameterSpecks))
                    continue;

                if (TryGetInstance(speck, specks, parameterSpecks, out object innerInstance))
                {
                    if (speck.DeliveryMode == Enums.DeliveryMode.Singleton) speck.Instance = innerInstance;
                    instance = innerInstance;
                    break;
                }
            }

            return instance != null;
        }

        private bool TryGetInstance(Speck speck, IEnumerable<Speck> specks, IEnumerable<Speck> parameterSpecks, out object instance)
        {
            var instances = parameterSpecks.Select(x => InstantiateSpeck(x.Type, specks)).ToArray();
            try
            {
                instance = instances.Any()
                    ? Activator.CreateInstance(speck.Type, instances)
                    : Activator.CreateInstance(speck.Type, true);
            }
            catch 
            { 
                instance = null; 
            }

            return instance != null;
        }

        private bool TrySetParameterSpecks(IEnumerable<Speck> specks, ConstructorArgs contructorArg, out IEnumerable<Speck> parameterSpecks)
        {
            var parameterSpecksList = new List<Speck>();
            var isParametersValid = true;
            foreach (var parameter in contructorArg.Parameters)
            {
                var paramSpeck = GetSpeck(parameter.ParameterType, specks);
                if (paramSpeck == null)
                {
                    isParametersValid = false;
                    break;
                }
                else
                {
                    parameterSpecksList.Add(paramSpeck);
                }
            }
            parameterSpecks = parameterSpecksList.AsReadOnly();
            return isParametersValid;
        }

        private Speck GetSpeck(Type type, IEnumerable<Speck> specks)
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

        private IEnumerable<ConstructorArgs> GetConstructorArgs(Type type)
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
}
