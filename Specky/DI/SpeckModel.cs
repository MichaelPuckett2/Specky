using Specky.Enums;
using System;
using System.Collections.Generic;

namespace Specky.DI
{
    /// <summary>
    /// Immutable DVO representing Speck and injection.
    /// </summary>
    internal class InjectionModel
    {
        public InjectionModel(Type type) : this(type, default) { }
        public InjectionModel(Type type, DeliveryMode deliveryMode) : this(type, deliveryMode, default) { }
        public InjectionModel(Type type, DeliveryMode deliveryMode, object instance) : this(type, deliveryMode, instance, default, default) { }
        public InjectionModel(Type type, DeliveryMode deliveryMode, object instance, string speckName, string configuration)
        {
            Type = type;
            DeliveryMode = deliveryMode;
            Instance = instance;
            SpeckName = speckName;
            Configuration = configuration;
        }

        public void Deconstruct(out Type type, out DeliveryMode deliveryMode, out object instance, out string speckName, out string configuration)
        {
            type = Type;
            deliveryMode = DeliveryMode;
            instance = Instance;
            speckName = SpeckName;
            configuration = Configuration;
        }

        public Type Type { get; }
        public object Instance { get; }
        public DeliveryMode DeliveryMode { get; }
        public string SpeckName { get; }
        public string Configuration { get; }
    }

    internal class InjectConfigurationModel : InjectionModel
    {
        public InjectConfigurationModel(Type type, Type parameterType, string configurationName, string configuration) : base(type, DeliveryMode.DataSet, configuration)
        {
            ParameterType = parameterType;
            ConfigurationName = configurationName;
        }

        public Type ParameterType { get; }
        public string ConfigurationName { get; }
    }

    internal class InjectFactoryModel : InjectionModel
    {
        public InjectFactoryModel(Func<IEnumerable<Type>> getParameters, Func<IEnumerable<Type>, (object Speck, DeliveryMode deliveryMode, string speckName, string configuration)> getFactorySpeck, Type type, DeliveryMode deliveryMode, string configuration) 
            : base(type, deliveryMode, default, default, configuration)
        {
            GetParameters = getParameters;
            GetFactorySpeck = getFactorySpeck;
        }

        public Func<IEnumerable<Type>> GetParameters { get; }
        public Func<IEnumerable<Type>, (object Speck, DeliveryMode deliveryMode, string speckName, string configuration)> GetFactorySpeck { get; }
    }
}