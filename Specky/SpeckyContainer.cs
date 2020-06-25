using Specky.Attributes;
using Specky.DI;
using Specky.Dtos;
using Specky.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Specky
{
    public sealed class SpeckyContainer
    {
        SpeckyContainer() { }
        static public SpeckyContainer Instance { get; } = new SpeckyContainer();
        internal HashSet<InjectionModel> InjectionModels { get; } = new HashSet<InjectionModel>();
        public string DefaultConfigurationName { get; private set; }
        public IEnumerable<SpeckDto> InjectedSpecks => InjectionModels.Select(x => new SpeckDto(x));
        public void InjectSpeck(Type type, string speckName = "") => InjectSpeck(new InjectionModel(type, default, default, speckName, default));

        internal void InjectSpeck(InjectionModel injectionModel)
        {
            lock (this)
            {
                if (!string.IsNullOrWhiteSpace(injectionModel.SpeckName))
                {
                    var hasExistingSpeckName = InjectionModels.Any(x => x.SpeckName == injectionModel.SpeckName);
                    if (hasExistingSpeckName) throw new Exception($"A Speck already exists with a unique name of {injectionModel.SpeckName}\nOnly one unique name is allowed per Speck using the SpeckNameAttribute");
                }
                InjectionModels.Add(injectionModel);
            }
        }

        public T GetSpeck<T>(string configurationName = "") => (T)GetSpeck(typeof(T), configurationName);

        public object GetSpeck(Type type, string configurationName = "")
        {
            if (string.IsNullOrWhiteSpace(configurationName)) configurationName = DefaultConfigurationName;

            if (TryGetInstantiatedSpeck(type, configurationName, out object speck)) return speck;

            if (TryGetFactorySpeck(type, out object factorySpeck)) return factorySpeck;

            if (TryGetConfigurationParameters(type, out object parameters, configurationName)) return parameters;

            GetExistingModel(type, configurationName, out InjectionModel uninstantiatedModel);

            switch (uninstantiatedModel.DeliveryMode)
            {
                case DeliveryMode.PerRequest:
                case DeliveryMode.Scoped:
                case DeliveryMode.Transient:
                case DeliveryMode.DataSet:
                    speck = SpeckActivator.InstantiateSpeck(type, uninstantiatedModel, HasSpeck, GetSpeck, HasParameterConfiguration, configurationName);
                    break;

                case DeliveryMode.SingleInstance:
                case DeliveryMode.Singleton:
                default:
                    speck = SpeckActivator.InstantiateSpeck(type, uninstantiatedModel, HasSpeck, GetSpeck, HasParameterConfiguration, configurationName);
                    var (speckType, deliveryMode, _, speckName, configuration) = uninstantiatedModel;
                    var instantiatedModel = new InjectionModel(speckType, deliveryMode, speck, speckName, configuration);
                    lock (this)
                    {
                        InjectionModels.Remove(uninstantiatedModel);
                        InjectionModels.Add(instantiatedModel);
                    }
                    break;
            }

            return speck;
        }

        private bool TryGetFactorySpeck(Type type, out object factorySpeck)
        {
            IEnumerable<InjectFactoryModel> factoryInjectionModels;

            lock (this)
            {
                factoryInjectionModels = InjectionModels
                                        .Where(x => x is InjectFactoryModel model)
                                        .Select(x => (InjectFactoryModel)x)
                                        .ToList()
                                        .AsReadOnly();
            }

            var factoryInjectionModel = factoryInjectionModels.Where(x => x.Type == type).FirstOrDefault();
            if (factoryInjectionModel == null)
            {
                factorySpeck = null;
                return false;
            }

            (object tupledFactorySpeck, DeliveryMode deliveryMode, string speckName, string configuration) = factoryInjectionModel.GetFactorySpeck.Invoke(factoryInjectionModel.GetParameters.Invoke());
            if (deliveryMode == DeliveryMode.SingleInstance)
            {
                InjectSpeck(new InjectionModel(tupledFactorySpeck.GetType(), DeliveryMode.SingleInstance, tupledFactorySpeck, speckName, configuration));
            }
            factorySpeck = tupledFactorySpeck;
            return factorySpeck != null;
        }

        public Task<object> GetSpeckAsync(Type type)
            => Task.Run(() => GetSpeck(type));

        private bool TryGetInstantiatedSpeck(Type type, string configuration, out object speck)
        {
            lock (this)
            {
                var configedSpeck = InjectionModels
                       .Where(x => (x.Type == type || type.IsAssignableFrom(x.Type)) && (x.Instance != null) && (x.DeliveryMode == DeliveryMode.SingleInstance || x.DeliveryMode == DeliveryMode.Singleton))
                       .Where(x => x.Configuration == configuration)
                       .Select(x => x.Instance)
                       .FirstOrDefault();

                speck = configedSpeck ?? InjectionModels
                       .Where(x => (x.Type == type || type.IsAssignableFrom(x.Type)) && (x.Instance != null) && (x.DeliveryMode == DeliveryMode.SingleInstance || x.DeliveryMode == DeliveryMode.Singleton))
                       .Where(x => string.IsNullOrWhiteSpace(configuration))
                       .Select(x => x.Instance)
                       .FirstOrDefault();
            }

            return speck != null;
        }

        public object GetSpeck(string typeName)
        {
            IReadOnlyList<InjectionModel> models;
            lock (this)
            {
                models = InjectionModels.Where(x => x.Type.Name == typeName || x.SpeckName == typeName).ToList();
            }
            if (models.Count > 1)
            {
                throw new Exception($"More than one Speck found with name of {typeName}.\nIt is recommended that all types have unique names.\nIf types must have the same name then define a unique name for each Speck with the SpeckName(speckName) attribute.");
            }
            var model = models.FirstOrDefault();
            if (model == null)
            {
                throw new Exception($"Specky has no registered types with name '{typeName}'.\nIf this is a unique name try using the SpeckName(speckName) attribute.");
            }
            return GetSpeck(model.Type);
        }

        public Task<object> GetSpeckAsync(string typeName)
            => Task.Run(() => GetSpeck(typeName));

        private void GetExistingModel(Type type, string configuration, out InjectionModel model)
        {
            if (type.IsInterface)
            {
                lock (this)
                {
                    var configedModel = InjectionModels.FirstOrDefault(x => IsAssignable(type, x.Type) && x.Configuration == configuration);
                    model = configedModel ?? InjectionModels.FirstOrDefault(x => IsAssignable(type, x.Type) && string.IsNullOrWhiteSpace(x.Configuration))
                         ?? throw new Exception($"Type: {type.Name} not injected.");
                }
            }
            else
            {
                lock (this)
                {
                    var configedModel = InjectionModels.FirstOrDefault(x => x.Type == type && x.Configuration == configuration);
                    model = configedModel ?? InjectionModels.FirstOrDefault(x => x.Type == type && string.IsNullOrWhiteSpace(x.Configuration))
                         ?? throw new Exception($"Type: {type.Name} not injected.");
                }
            }
        }

        internal bool HasSpeck(Type parameterType)
        {
            lock (this)
            {
                return parameterType.IsInterface
                     ? InjectionModels.Any(x => IsAssignable(parameterType, x.Type))
                     : InjectionModels.Any(x => x.Type == parameterType);
            }
        }

        internal bool TryGetConfigurationParameters(Type parameterType, out object parametersInstance, string configurationName = "")
        {
            IEnumerable<InjectConfigurationModel> configurationModels;
            lock (this)
            {
                configurationModels = InjectionModels
                                     .Where(x => x is InjectConfigurationModel model && model.ParameterType == parameterType)
                                     .Select(x => (InjectConfigurationModel)x)
                                     .ToList()
                                     .AsReadOnly();
            }
            if (!configurationModels.Any())
            {
                parametersInstance = default;
                return false;
            }

            var configurationModel = configurationModels.FirstOrDefault(x => !string.IsNullOrWhiteSpace(configurationName) && x.ConfigurationName == configurationName)
                                  ?? configurationModels.FirstOrDefault();

            var config = Activator.CreateInstance(configurationModel.Type);

            if (config == null)
            {
                parametersInstance = default;
                return false;
            }
            var propertyInfo = config.GetType().GetProperties().Where(p => p.PropertyType == configurationModel.ParameterType).FirstOrDefault();
            parametersInstance = propertyInfo?.GetValue(config);
            return propertyInfo != null;
        }

        internal bool HasParameterConfiguration(ParameterInfo parameterInfo)
        {
            IEnumerable<InjectConfigurationModel> configurationModels;
            lock (this)
            {
                configurationModels = InjectionModels
                                     .Where(x => x is InjectConfigurationModel model && model.ParameterType == parameterInfo.ParameterType)
                                     .Select(x => (InjectConfigurationModel)x)
                                     .ToList()
                                     .AsReadOnly();
            }
            if (!configurationModels.Any()) return false;
            var configurationAttribute = parameterInfo.GetCustomAttribute<SpeckyConfigurationAttribute>();
            if (configurationAttribute == null) return true;
            return configurationModels.Any(x => x.ConfigurationName == configurationAttribute.ConfigurationName);
        }

        internal void SetConfiguration(string configurationName)
        {
            DefaultConfigurationName = configurationName;
        }

        public static bool IsAssignable(Type requestedType, Type injectedType)
        {
            if (requestedType.IsAssignableFrom(injectedType))
            {
                return true;
            }

            if ((requestedType.IsGenericType && injectedType.IsGenericType) && (requestedType.Assembly == injectedType.Assembly))
            {
                var interfaces = injectedType.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (requestedType.Name == @interface.Name) return true;
                }
            }

            return false;
        }

        ~SpeckyContainer()
        {
            foreach (var injectionModel in InjectionModels)
                if (injectionModel.Instance != null && injectionModel.Instance is IDisposable disposable)
                    try { disposable?.Dispose(); }
                    catch { }
        }
    }
}
