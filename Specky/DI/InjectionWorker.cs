using Specky.Attributes;
using Specky.Enums;
using Specky.Exceptions;
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
        private readonly string Configuration;

        internal InjectionWorker(IEnumerable<Assembly> strappingAssemblies, string configurationName)
        {
            StrappingAssemblies = strappingAssemblies;
            SpeckyContainer.Instance.SetConfiguration(configurationName);
            Configuration = configurationName;
        }

        internal void Start()
        {
            var tuples = (from assembly in StrappingAssemblies
                          from tuple in assembly.TypesWithAttribute<SpeckAttribute>()
                          select tuple).ToList().AsReadOnly();

            tuples
                .Log("Injecting Specks.", PrintType.DebugWindow)
                .ForEach(InjectSpeck);

            tuples
                .Log("Testing Specks", PrintType.DebugWindow)
                .ForEach(TestConstructors);
        }

        private void InjectSpeck((Type Type, SpeckAttribute Attribute) tuple)
        {
            var (type, attribute) = tuple;

            switch (attribute)
            {
                case SpeckyFactoryAttribute speckyFactoryAttribute:
                    InjectFactorySpeck(type, attribute, speckyFactoryAttribute);
                    break;
                case SpeckyConfigurationAttribute configurationAttribute:
                    InjectConfigurationSpeck(type, configurationAttribute);
                    break;

                case SpeckNameAttribute nameAttribute:
                    InjectNameSpeck(type, attribute, nameAttribute);
                    break;

                case SpeckyConditionAttribute conditionAttribute:
                    InjectConditionSpeck(type, attribute, conditionAttribute);
                    break;

                case SpeckAttribute speckAttribute:
                default:
                    InjectDefaultSpeck(type, attribute);
                    break;
            }
        }

        private void InjectFactorySpeck(Type type, SpeckAttribute attribute, SpeckyFactoryAttribute speckyFactoryAttribute)
        {
            InjectDefaultSpeck(type, attribute);
            MethodInfo methodInfo;

            if (TryGetMethodFromName(type, speckyFactoryAttribute, out MethodInfo methodInfoFromName))
            {
                methodInfo = methodInfoFromName;
            }
            else if (TryGetMethodFromAttribute(type, out MethodInfo methodInfoFromAttribute))
            {
                methodInfo = methodInfoFromAttribute;
            }
            else
            {
                throw new SpeckyFactoryMethodNotFoundException(type);
            }

            var speckType = methodInfo.ReturnType;
            var getParameters = new Func<IEnumerable<Type>>(() => methodInfo.GetParameters().Select(x => x.ParameterType));
            var getFactorySpeck = new Func<IEnumerable<Type>, (object Speck, Lifetime deliveryMode, string speckName, string configuration)>((types) =>
            {
                var specks = new List<object>();
                foreach (var t in types) specks.Add(SpeckyContainer.Instance.GetSpeck(t));

                object speck;
                Lifetime deliveryMode;
                string speckName;
                string configuration;

                speck = methodInfo.Invoke(SpeckyContainer.Instance.GetSpeck(type), specks.ToArray());
                switch (methodInfo.GetCustomAttribute<SpeckAttribute>())
                {
                    case SpeckyFactoryAttribute speckyFactory:
                        deliveryMode = speckyFactory.DeliveryMode;
                        speckName = speckyFactory.SpeckName;
                        configuration = speckyFactory.Configuration;
                        break;

                    case SpeckNameAttribute speckNameAttribute:
                        deliveryMode = speckNameAttribute.DeliveryMode;
                        speckName = speckNameAttribute.SpeckName;
                        configuration = speckNameAttribute.Configuration;
                        break;

                    case SpeckAttribute speckAttribute:
                        deliveryMode = speckAttribute.DeliveryMode;
                        speckName = default;
                        configuration = speckAttribute.Configuration;
                        break;

                    default:
                        deliveryMode = Lifetime.SingleInstance;
                        speckName = default;
                        configuration = default;
                        break;
                }

                return (speck, deliveryMode, speckName, configuration);
            });

            SpeckyContainer.Instance.InjectSpeck(new InjectFactoryModel(getParameters, getFactorySpeck, speckType, attribute.DeliveryMode, attribute.Configuration));
        }

        private static bool TryGetMethodFromName(Type type, SpeckyFactoryAttribute speckyFactoryAttribute, out MethodInfo methodInfo)
        {
            try
            {
                methodInfo = type.GetMethod(speckyFactoryAttribute.FactoryMethodName);
            }
            catch
            {
                methodInfo = default;
                return false;
            }

            return true;
        }

        private bool TryGetMethodFromAttribute(Type type, out MethodInfo methodInfo)
        {
            try
            {
                methodInfo = type.GetMethods().Where(x => x.GetCustomAttribute<SpeckAttribute>() != null).FirstOrDefault();
            }
            catch
            {
                methodInfo = default;
                return false;
            }

            return methodInfo != null;
        }

        private static void InjectDefaultSpeck(Type type, SpeckAttribute attribute)
        {
            SpeckyContainer.Instance.InjectSpeck(new InjectionModel(type, attribute.DeliveryMode, default, default, attribute.Configuration));
        }

        private static void InjectConditionSpeck(Type type, SpeckAttribute attribute, SpeckyConditionAttribute conditionAttribute)
        {
            if (conditionAttribute.TestCondition())
            {
                SpeckyContainer.Instance.InjectSpeck(new InjectionModel(type, attribute.DeliveryMode, default, default, conditionAttribute.Configuration));
            }
        }

        private static void InjectNameSpeck(Type type, SpeckAttribute attribute, SpeckNameAttribute nameAttribute)
        {
            SpeckyContainer.Instance.InjectSpeck(new InjectionModel(type, attribute.DeliveryMode, default, nameAttribute.SpeckName, nameAttribute.Configuration));
        }

        private static void InjectConfigurationSpeck(Type type, SpeckyConfigurationAttribute configurationAttribute)
        {
            var parameterTypes = type.GetProperties().Where(p => p.GetCustomAttribute<SpeckyConfigurationParametersAttribute>() != null).ToList().AsReadOnly();
            if (!parameterTypes.Any())
            {
                throw new SpeckyConfigurationException($"{nameof(SpeckyConfigurationAttribute)} must contain a {nameof(SpeckyConfigurationParametersAttribute)}. This is what Specky uses to inject the configuration for an expected type.");
            }
            SpeckyContainer.Instance.InjectSpeck(new InjectConfigurationModel(type, parameterTypes.FirstOrDefault().PropertyType, configurationAttribute.ConfigurationName, configurationAttribute.Configuration));
        }

        private static void TestConstructors((Type Type, SpeckAttribute Attribute) tuple)
        {
            var (type, attribute) = tuple;
            var constructors = type.GetConstructors().ToList().AsReadOnly();

            if (attribute is SpeckyConfigurationAttribute)
            {
                if (constructors.Any(x => x.GetParameters().Any()))
                {
                    throw new SpeckyConfigurationException($"{nameof(SpeckyConfigurationAttribute)} cannot contain parameterized constructors.");
                }

                var hasValidParameters = false;
                foreach (var configProperty in type.GetProperties())
                {
                    if (configProperty.GetCustomAttribute<SpeckyConfigurationParametersAttribute>() != null)
                    {
                        hasValidParameters = true;
                        break;
                    }
                }

                if (!hasValidParameters)
                {
                    throw new SpeckyConfigurationException($"{nameof(SpeckyConfigurationAttribute)} must contain a {nameof(SpeckyConfigurationParametersAttribute)}. This is what Specky uses to inject the configuration for an expected type.");
                }

                return;
            }

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
                throw new Exception($"Specky has examined all possible constructors for {type.Name}.\nThere are no registered types to warrant initialization of any overloaded constructor.\nParameters missing associated Specks:\n{parameters}\nMake sure the SpeckAttribute is added to the class that should be injected.");
            }
        }
    }
}
