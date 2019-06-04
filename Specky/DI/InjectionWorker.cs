using Specky.Attributes;
using Specky.Enums;
using Specky.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Specky.DI
{
    internal class InjectionWorker
    {
        private readonly IEnumerable<Assembly> StrappingAssemblies;
        internal InjectionWorker(IEnumerable<Assembly> strappingAssemblies) => StrappingAssemblies = strappingAssemblies;

        internal void Start()
        {
            var tuples = new List<(Type Type, SpeckAttribute Attribute)>();
            foreach (var assembly in StrappingAssemblies)
                tuples.AddRange(assembly.TypesWithAttribute<SpeckAttribute>());

            tuples
                .Log("Injecting Specks.", PrintType.DebugWindow)
                .ForEach((tuple) =>
                {
                    var speckName = tuple.Attribute is SpeckNameAttribute speckNameAttribute
                                  ? speckNameAttribute.SpeckName
                                  : "";

                    SpeckyContainer.Instance.InjectSpeck(new InjectionModel(tuple.Type, tuple.Attribute.DeliveryMode, default, speckName));
                });

            tuples
                .Log("Testing Specks", PrintType.DebugWindow)
                .ForEach((tuple) =>
                {
                    foreach (var constructor in tuple.Type.GetConstructors())
                        foreach (var parameter in constructor.GetParameters())
                            if (!SpeckyContainer.Instance.HasSpeck(parameter.ParameterType))
                                throw new Exception($"Specky will not be able to initialize {tuple.Type.Name} with parameter {parameter.ParameterType.Name}");
                });
        }
    }
}
