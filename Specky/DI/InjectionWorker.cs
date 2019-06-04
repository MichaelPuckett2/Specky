using Specky.Attributes;
using Specky.Enums;
using Specky.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
                    var constructors = tuple.Type.GetConstructors().ToList();
                    var failedCount = 0;
                    var parameters = new StringBuilder();
                    foreach (var constructor in constructors)
                    {
                        foreach (var parameter in constructor.GetParameters())
                        {
                            if (!SpeckyContainer.Instance.HasSpeck(parameter.ParameterType))
                            {
                                failedCount++;
                                parameters.AppendLine(parameter.ParameterType.Name);
                            }
                        }
                    }
                    if (failedCount == constructors.Count)
                    {
                        throw new Exception($"Specky has examined all possible constructors for {tuple.Type.Name}.\nThere are no registered types to warrant initialization of any overloaded constructor.\nParameters expected:\n{parameters.ToString()}");
                    }
                });
        }
    }
}
